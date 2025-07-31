using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Threading;
using TelegramBot.Entities;
using TelegramBot.Exceptions;
using TelegramBot.Services;

namespace TelegramBot
{
    public delegate void MessageEventHandler(string  message);
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _botClient;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;
        private event MessageEventHandler OnHandleUpdateStarted;
        private event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(IUserService userService, ITelegramBotClient botClient, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _botClient = botClient;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            PublishOnUpdateStarted(update.Message.Text);
            
            try
            {
                
                await ExecuteCommand(update, ct);
            }

            catch (ArgumentException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
            }

            catch (TaskCountLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
            }

            catch (TaskLengthLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
            }

            catch (DuplicateTaskException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
            }

            PublishOnUpdateCompleted(update.Message.Text);
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
                    await _botClient.SendMessage(botUpdate.Message.Chat, "Доступны команды /start, /help, /info", ct);
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

            switch (command)
            {
                case "/start":
                    await CommandStart(botUpdate, ct);
                    break;

                case "/help":
                    await CommandHelp(botUpdate, ct);
                    break;

                case "/info":
                    await CommandInfo(botUpdate, ct);
                    break;

                case "/addtask":
                    await CommandAddTask(parameter, botUpdate, ct);
                    break;

                case "/completetask":
                    await CommandCompleteTask(parameter, botUpdate, ct);
                    break;

                case "/showtasks":
                      await CommandShowTasks(parameter, botUpdate, ct);
                    break;

                case "/showalltasks":
                    await CommandShowAllTasks(botUpdate, ct);
                    break;

                case "/removetask":
                    await CommandRemoveTask(parameter, botUpdate, ct);
                    break;
                
                case "/report":
                    await CommandReport(botUpdate, ct);
                    break;
                case "/find":
                    await CommandShowTasks(parameter, botUpdate, ct);
                    break;

                case "/exit":
                    await CommandExit(botUpdate, ct);
                    return;

                default:
                    await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser)}", ct);
                    break;
            }
            return;
        }

        private async Task CommandStart(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            if (_toDoUser == null) 
                _toDoUser = await _userService.RegisterUserAsync(botUpdate.Message.From.Id, botUpdate.Message.From.Username, ct);

            await _botClient.SendMessage(botUpdate.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}! Чем могу помочь?", ct);
        }

        private async Task CommandHelp(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, @$"
{GetFullOutput("Справочная информация:", _toDoUser)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/addtask - добавить задачу в список
/showtasks - показать список активных задач
/showalltasks - показать список всех задач
/find - показать список задач по фильтру
/report - статистика по задачам
/completetask - завершить активную задачу
/removetask - удалить задачу из списка
/exit - завершение работы"
, ct
);
        }

        private async Task CommandInfo(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _toDoUser), ct);
        }

        private async Task CommandAddTask(string parameter, Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _toDoService.AddAsync(_toDoUser, parameter, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, "Задача добавлена", ct);
        }

        private async Task CommandCompleteTask(string parameter, Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var userToDoItemList = await _toDoService.GetAllByUserIdAsync(_toDoUser.UserId, ct);

            if (userToDoItemList.Count == 0)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}", ct);
                return;
            }
            
            foreach (var toDoItem in userToDoItemList)
            {
                if (toDoItem.Id.ToString() == parameter)
                {
                    await _toDoService.MarkCompletedAsync(toDoItem.Id, ct);
                    await _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput($"Задача завершена - {toDoItem.Name} - {toDoItem.Id}", _toDoUser), ct);
                    return;
                }
            }
            await _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput($"Задача с Id {parameter} не найдена", _toDoUser), ct);
        }

        private async Task CommandShowTasks(string parameter, Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            IReadOnlyList<ToDoItem> userToDoItemList;
            if (parameter == string.Empty)
            {
                userToDoItemList = await _toDoService.GetAllByUserIdAsync(_toDoUser.UserId, ct);
            }
            else
            {
                userToDoItemList = await _toDoService.FindAsync(_toDoUser, parameter, ct);
            }
            
            if (userToDoItemList.Count == 0)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}", ct);
                return;
            }

            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.CreatedAt} - {toDoItem.Id}", ct);
                counter++;
            }
        }

        private async Task CommandShowAllTasks(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var userToDoItemList = await _toDoService.GetAllByUserIdAsync(_toDoUser.UserId, ct);
            if (userToDoItemList.Count == 0)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}", ct);
                return;
            }
            
            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.State} - {toDoItem.CreatedAt} - {toDoItem.Id}", ct);
                counter++;
            }
        }

        private async Task CommandRemoveTask(string taskNo, Update botUpdate, CancellationToken ct)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var userToDoItemList = await _toDoService.GetAllByUserIdAsync(toDoUser.UserId, ct);
            
            if (userToDoItemList.Count == 0)
            {
                await _botClient.SendMessage(botUpdate.Message.Chat, "Список задач пуст", ct);
                return;
            }
            
            var taskNoInt = ParseAndValidateInt(taskNo, 1, userToDoItemList.Count);

            await _toDoService.DeleteAsync(userToDoItemList[taskNoInt - 1].Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, $"Задача с номером {taskNoInt} удалена", ct);
        }

        private async Task CommandReport(Update botUpdate, CancellationToken ct)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            var stats = await Task.Run(() => _toDoReportService.GetUserStatsAsync(toDoUser.UserId, ct));
            await _botClient.SendMessage(botUpdate.Message.Chat, 
                $"Статистика по задачам на {stats.generatedAt}. Всего: {stats.total}; Завершенных: {stats.completed}; Активных: {stats.active}", ct);
        }

        private async Task CommandExit(Update botUpdate, CancellationToken ct)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, ct);
            await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser)}", ct);
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

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
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

    }
}
