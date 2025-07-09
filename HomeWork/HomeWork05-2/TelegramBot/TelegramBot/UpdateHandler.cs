using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _botClient;
        private readonly IToDoService _toDoService;

        public UpdateHandler(IUserService userService, ITelegramBotClient botClient, IToDoService toDoService)
        {
            _userService = userService;
            _botClient = botClient;
            _toDoService = toDoService;
        }
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {

            try
            {
                ExecuteCommand(update);
            }

            catch (ArgumentException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }

            catch (TaskCountLimitException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }

            catch (TaskLengthLimitException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }

            catch (DuplicateTaskException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }
        }

        private bool ExecuteCommand(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            var userCommand = botUpdate.Message.Text;
            if (userCommand == string.Empty)
                return true;

            if (_toDoUser == null)
            {
                if ((userCommand != "/start") && (userCommand != "/help") && (userCommand != "/info"))
                {
                    _botClient.SendMessage(botUpdate.Message.Chat, "Доступны команды /start, /help, /info");
                    return true;
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
                    CommandStart(botUpdate);
                    break;

                case "/help":
                    CommandHelp(botUpdate);
                    break;

                case "/info":
                    CommandInfo(botUpdate);
                    break;

                case "/addtask":
                    CommandAddTask(parameter, botUpdate);
                    break;

                case "/completetask":
                    CommandCompleteTask(parameter, botUpdate);
                    break;

                case "/showtasks":
                      CommandShowTasks(botUpdate);
                    break;

                case "/showalltasks":
                    CommandShowAllTasks(botUpdate);
                    break;

                case "/removetask":
                    CommandRemoveTask(parameter, botUpdate);
                    break;

                case "/exit":
                    CommandExit(botUpdate);
                    return false;

                default:
                    _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser)}");
                    break;
            }
            return true;
        }

        private void CommandStart(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            if (_toDoUser == null) 
                _toDoUser = _userService.RegisterUser(botUpdate.Message.From.Id, botUpdate.Message.From.Username);

            _botClient.SendMessage(botUpdate.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}! Чем могу помочь?");
        }

        private void CommandHelp(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            _botClient.SendMessage(botUpdate.Message.Chat, @$"
{GetFullOutput("Справочная информация:", _toDoUser)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/addtask - добавить задачу в список
/showtasks - показать список активных задач
/showalltasks - показать список всех задач
/completetask - завершить активную задачу
/removetask - удалить задачу из списка
/exit - завершение работы"
);
        }

        private void CommandInfo(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _toDoUser));
        }

        private void CommandAddTask(string parameter, Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            _toDoService.Add(_toDoUser, parameter);
            _botClient.SendMessage(botUpdate.Message.Chat, "Задача добавлена");
        }

        private void CommandCompleteTask(string parameter, Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);

            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }
            
            foreach (var toDoItem in userToDoItemList)
            {
                if (toDoItem.Id.ToString() == parameter)
                {
                    _toDoService.MarkCompleted(toDoItem.Id);
                    _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput($"Задача завершена - {toDoItem.Name} - {toDoItem.Id}", _toDoUser));
                    return;
                }
            }
            _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput($"Задача с Id {parameter} не найдена", _toDoUser));
        }

        private void CommandShowTasks(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }

            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                counter++;
            }
        }

        private void CommandShowAllTasks(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            var userToDoItemList = _toDoService.GetAllByUserId(_toDoUser.UserId);
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }
            
            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.State} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                counter++;
            }
        }

        private void CommandRemoveTask(string taskNo, Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);
            
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(botUpdate.Message.Chat, "Список задач пуст");
                return;
            }
            
            var taskNoInt = ParseAndValidateInt(taskNo, 1, userToDoItemList.Count);

            _toDoService.Delete(userToDoItemList[taskNoInt - 1].Id);
            _botClient.SendMessage(botUpdate.Message.Chat, $"Задача с номером {taskNoInt} удалена");
        }

        private void CommandExit(Update botUpdate)
        {
            var _toDoUser = _userService.GetUser(botUpdate.Message.From.Id);
            _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser)}");
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
    }
}
