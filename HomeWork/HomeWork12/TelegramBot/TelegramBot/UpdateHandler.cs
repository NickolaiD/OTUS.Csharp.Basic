using System.Data.Common;
using System.Reflection.Metadata;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Dto;
using TelegramBot.Entities;
using TelegramBot.Exceptions;
using TelegramBot.Scenarios;
using TelegramBot.Services;
using static TelegramBot.BotHelper;

namespace TelegramBot
{
    public delegate void MessageEventHandler(string  message);
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _botClient;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;
        private readonly IEnumerable<IScenario> _scenarios;
        private readonly IScenarioContextRepository _contextRepository;
        private readonly IToDoListService _toDoListService;
        private event MessageEventHandler OnHandleUpdateStarted;
        private event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(IUserService userService, 
                             ITelegramBotClient botClient,
                             IToDoService toDoService,
                             IToDoReportService toDoReportService,
                             IEnumerable<IScenario> scenarios,
                             IScenarioContextRepository contextRepository,
                             IToDoListService toDoListService)
        {
            _userService = userService;
            _botClient = botClient;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
            _scenarios = scenarios;
            _contextRepository = contextRepository;
            _toDoListService = toDoListService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await (update switch
                {
                    { Message: { } message } => OnMessage(update, ct),
                    { CallbackQuery: { } callbackQuery } => OnCallbackQuery(update, ct)
                });
                
            }

            catch (ArgumentException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }

            catch (TaskCountLimitException ex)
            {
                if (update.Message != null)
                    await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
            }

            catch (TaskLengthLimitException ex)
            {
                if (update.Message != null)
                    await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
            }

            catch (DuplicateTaskException ex)
            {
                if (update.Message != null)
                  await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: ct, replyMarkup: GetKeyboardCancel());
            }
        }
        private async Task OnMessage(Update update, CancellationToken ct)
        {
            PublishOnUpdateStarted(update.Message.Text);
            var context = await _contextRepository.GetContext(update.Message.From.Id, ct);
            if (context != null)
            {
                if (update.Message.Text == "/cancel")
                {
                    await _contextRepository.ResetContext(update.Message.From.Id, ct);
                    await _botClient.SendMessage(update.Message.Chat, "Действие отменено", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                    PublishOnUpdateCompleted(update.Message.Text);
                    return;
                }
                await ProcessScenario(context, update, ct);
                PublishOnUpdateCompleted(update.Message.Text);
                return;
            }

            await ExecuteCommand(update, ct);
            PublishOnUpdateCompleted(update.Message.Text);
        }
        private async Task OnCallbackQuery(Update update, CancellationToken ct)
        {
            PublishOnUpdateStarted(update.CallbackQuery.Data);
            var context = await _contextRepository.GetContext(update.CallbackQuery.From.Id, ct);
            if (context != null)
            {
                await ProcessScenario(context, update, ct);
                PublishOnUpdateCompleted(update.CallbackQuery.Data);
                return;
            }

            var _toDoUser = await _userService.GetUserAsync(update.CallbackQuery.From.Id, ct);
            if (_toDoUser == null)
            {
                return; 
            }
            var callback = CallbackDto.FromString(update.CallbackQuery.Data);
            InlineKeyboardMarkup replyKeyboardMarkup;
            switch (callback.Action)
            {
                case "show":
                    var toDoListCallback = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    IReadOnlyList<ToDoItem> userToDoItemList;
                    userToDoItemList = await _toDoService.GetByUserIdAndListAsync(_toDoUser.UserId, toDoListCallback.ToDoListId, ct);

                    if (userToDoItemList.Count == 0)
                    {
                        await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                        return;
                    }

                    var listButtons = new List<InlineKeyboardButton[]>();
                    foreach (var toDoItem in userToDoItemList)
                    {
                        listButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(toDoItem.Name, $"showtask|{toDoItem.Id}") });
                    }
                    replyKeyboardMarkup = new InlineKeyboardMarkup(listButtons);
                    await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"Список задач", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
                    break;
                case "addlist":
                    await ProcessScenario(new ScenarioContext(ScenarioType.AddList, update.CallbackQuery.From.Id), update, ct);
                    break;
                case "deletelist":
                    await ProcessScenario(new ScenarioContext(ScenarioType.DeleteList, update.CallbackQuery.From.Id), update, ct);
                    break;
                case "showtask":
                    var toDoItemCallback = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                    if (toDoItemCallback.ToDoItemId != null)
                    {
                        var toDoItem = await _toDoService.Get((Guid)toDoItemCallback.ToDoItemId, ct);
                        if (toDoItem != null)
                        {
                            replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData("✅Выполнить", $"completetask|{toDoItem.Id}"),
                                    InlineKeyboardButton.WithCallbackData("❌Удалить", $"deletetask|{toDoItem.Id}")
                                }
                            });
                             
                            await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"{toDoItem.Name}:\n Срок выполнения {toDoItem.Deadline}\n Время создания {toDoItem.CreatedAt}", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
                        }
                    }
                    break;
                case "completetask":
                    toDoItemCallback = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                    var toDoItemComplete = await _toDoService.Get(toDoItemCallback.ToDoItemId.GetValueOrDefault(), ct);
                    await _toDoService.MarkCompletedAsync(toDoItemComplete.Id, ct);
                    await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"Задача завершена - {toDoItemComplete.Name}", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                    break;
                case "deletetask":
                    await ProcessScenario(new ScenarioContext(ScenarioType.DeleteTask, update.CallbackQuery.From.Id), update, ct);
                    break;
            }
            PublishOnUpdateCompleted(update.CallbackQuery.Data);
        }
        private async Task ExecuteCommand(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var userCommand = botUpdate.Message.Text;
            if (userCommand == string.Empty)
                return;

            if (_toDoUser == null)
            {
                if ((userCommand != "/start") && (userCommand != "/help") && (userCommand != "/info"))
                {
                    await _botClient.SendMessage(botUpdate.Message.Chat, "Доступны команды /start, /help, /info", cancellationToken: ct, replyMarkup:GetKeyboardButtons(false));
                    return;
                }
            }

            int spacePosition = userCommand.IndexOf(' ');
            string command, parameter;

            if (spacePosition == -1)
            {
                command = userCommand;
                parameter = "";
            }
            else
            {
                command = userCommand.Substring(0, spacePosition);
                parameter = userCommand.Substring(spacePosition + 1);
            }

            bool userRegistered = !(_toDoUser == null);
            switch (command)
            {
                case "/start":
                    await CommandStart(botUpdate, ct);
                    break;

                case "/help":
                    await CommandHelp(botUpdate, ct, userRegistered);
                    break;

                case "/info":
                    await CommandInfo(botUpdate, ct, userRegistered);
                    break;

                case "/addtask":
                    await CommandAddTask(parameter, botUpdate, ct);
                    break;

                case "/show":
                      await CommandShow(parameter, botUpdate, ct);
                    break;
                
                case "/report":
                    await CommandReport(botUpdate, ct);
                    break;
                
                case "/find":
                    await CommandShow(parameter, botUpdate, ct);
                    break;

                case "/exit":
                    await CommandExit(botUpdate, ct);
                    return;

                default:
                    await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser)}", cancellationToken: ct, replyMarkup:GetKeyboardButtons(userRegistered));
                    break;
            }
            return;
        }

        private async Task CommandStart(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            if (_toDoUser == null) 
                _toDoUser = await _userService.RegisterUserAsync(botUpdate.Message.From.Id, botUpdate.Message.From.Username, ct);

            await _botClient.SendMessage(botUpdate.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}! Чем могу помочь?", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
        }

        private async Task CommandHelp(Update botUpdate, CancellationToken ct, bool userRegistered)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, @$"
{GetFullOutput("Справочная информация:", _toDoUser)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/addtask - добавить задачу в список
/show - показать список активных задач
/find - показать список задач по фильтру
/report - статистика по задачам
/cancel - отменить действие
/exit - завершение работы"
, cancellationToken: ct, replyMarkup: GetKeyboardButtons(userRegistered)
);
        }

        private async Task CommandInfo(Update botUpdate, CancellationToken ct, bool userRegistered)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _toDoUser), cancellationToken: ct, replyMarkup: GetKeyboardButtons(userRegistered));
        }

        private async Task CommandAddTask(string parameter, Update botUpdate, CancellationToken ct)
        {
            await ProcessScenario(new ScenarioContext(ScenarioType.AddTask, botUpdate.Message.From.Id), botUpdate, ct);
        }
        private async Task CommandShow(string parameter, Update botUpdate, CancellationToken ct)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var toDoList = await _toDoListService.GetUserLists(toDoUser.UserId, ct);
            var listButtons = new List<InlineKeyboardButton>();
            foreach (var list in toDoList) 
            {
                listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"show|{list.Id}"));
            }
            
            var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📌Без списка", "show|null")
                },
                listButtons.ToArray(),
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🆕Добавить", "addlist"),
                    InlineKeyboardButton.WithCallbackData("❌Удалить", "deletelist")
                }
            });

            await _botClient.SendMessage(botUpdate.Message.Chat, $"Выберите список", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
          
        }
        private async Task CommandReport(Update botUpdate, CancellationToken ct)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var stats = await Task.Run(() => _toDoReportService.GetUserStatsAsync(toDoUser.UserId, ct));
            await _botClient.SendMessage(botUpdate.Message.Chat, 
                $"Статистика по задачам на {stats.generatedAt}. Всего: {stats.total}; Завершенных: {stats.completed}; Активных: {stats.active}", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
        }

        private async Task CommandExit(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser)}", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
            Console.Read();
        }

        private string GetFullOutput(string request, ToDoUser? toDoUser)
        {
            if (toDoUser == null)
                return request;
            else
                return $"{toDoUser.TelegramUserName}, {request.ToLower()}";
        }
        public static int ParseAndValidateInt(string? str, int min, int max)
        {
            if ((!int.TryParse(str, out int result)) || (result < min) || (result > max))
            {
                throw new ArgumentException();
            }
            return result;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }

        public void SubscribeOnUpdateStarted(MessageEventHandler handler) => OnHandleUpdateStarted += handler;
        public void SubscribeOnUpdateCompleted(MessageEventHandler handler) => OnHandleUpdateCompleted += handler;
        public void UnSubscribeOnUpdateStarted(MessageEventHandler handler) => OnHandleUpdateStarted -= handler;
        public void UnSubscribeOnUpdateCompleted(MessageEventHandler handler) => OnHandleUpdateCompleted -= handler;

        public void PublishOnUpdateStarted(string message) => OnHandleUpdateStarted.Invoke(message);
        public void PublishOnUpdateCompleted(string message) => OnHandleUpdateCompleted.Invoke(message);
        private IScenario GetScenario(ScenarioType scenario)
        {

            foreach (var item in _scenarios)
            {
                if (item.CanHandle(scenario))
                    return item;
            }
            throw new Exception("Сценарий не найден");
        }

        private async Task ProcessScenario(ScenarioContext context, Update update, CancellationToken ct)
        {
            var scenario = GetScenario(context.CurrentScenario);
            var result = await scenario.HandleMessageAsync(_botClient, context, update, ct);

            if (result == ScenarioResult.Completed)
            {
                await _contextRepository.ResetContext(context.UserId, ct);
            }
            else
            {
                await _contextRepository.ResetContext(context.UserId, ct);
                await _contextRepository.SetContext(context.UserId, context, ct);
            }
            
        }
    }
}
