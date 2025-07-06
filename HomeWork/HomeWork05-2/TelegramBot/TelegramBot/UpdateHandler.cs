using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private ToDoUser? _toDoUser;
        private ITelegramBotClient _botClient;
        private Update _update;
        private IToDoService _toDoService;
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            _botClient = botClient;
            _update = update;
            
            if (_toDoService == null)
            {
                Console.WriteLine("Введите максимально допустимое количество задач");
                var _taskCountLimit = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.WriteLine("Введите максимально допустимую длину задачи");
                var _taskLengthLimit = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                _toDoService = new ToDoService(botClient, update, _taskCountLimit, _taskLengthLimit);
            }

            try
            {
                ExecuteCommand(update.Message.Text);
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

        private bool ExecuteCommand(string userCommand)
        {
            if (userCommand == string.Empty)
                return true;

            if (_toDoUser == null)
            {
                if ((userCommand != "/start") && (userCommand != "/help") && (userCommand != "/info"))
                {
                    _botClient.SendMessage(_update.Message.Chat, "Доступны команды /start, /help, /info");
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
                    CommandStart();
                    break;

                case "/help":
                    CommandHelp();
                    break;

                case "/info":
                    CommandInfo();
                    break;

                case "/addtask":
                    CommandAddTask(parameter);
                    break;

                case "/completetask":
                    CommandCompleteTask(parameter);
                    break;

                case "/showtasks":
                      CommandShowTasks();
                    break;

                case "/showalltasks":
                    CommandShowAllTasks();
                    break;

                case "/removetask":
                    CommandRemoveTask(parameter);
                    break;

                case "/exit":
                    CommandExit();
                    return false;

                default:
                    _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser)}");
                    break;
            }
            return true;
        }

        private void CommandStart()
        {
            IUserService userService = new UserService();
            _toDoUser = userService.GetUser(_update.Message.From.Id);
            if (_toDoUser == null) 
                _toDoUser = userService.RegisterUser(_update.Message.From.Id, _update.Message.From.Username);

            _botClient.SendMessage(_update.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}! Чем могу помочь?");
        }

        private void CommandHelp()
        {
            _botClient.SendMessage(_update.Message.Chat, @$"
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

        private void CommandInfo()
        {
            _botClient.SendMessage(_update.Message.Chat, GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _toDoUser));
        }

        private void CommandAddTask(string parameter)
        {
            _toDoService.Add(_toDoUser, parameter);
            _botClient.SendMessage(_update.Message.Chat, "Задача добавлена");
        }

        private void CommandCompleteTask(string parameter)
        {
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);

            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }
            
            foreach (var toDoItem in userToDoItemList)
            {
                if (toDoItem.Id.ToString() == parameter)
                {
                    _toDoService.MarkCompleted(toDoItem.Id);
                    _botClient.SendMessage(_update.Message.Chat, GetFullOutput($"Задача завершена - {toDoItem.Name} - {toDoItem.Id}", _toDoUser));
                    return;
                }
            }
            _botClient.SendMessage(_update.Message.Chat, GetFullOutput($"Задача с Id {parameter} не найдена", _toDoUser));
        }

        private void CommandShowTasks()
        {
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }

            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                _botClient.SendMessage(_update.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                counter++;
            }
        }

        private void CommandShowAllTasks()
        {
            var userToDoItemList = _toDoService.GetAllByUserId(_toDoUser.UserId);
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
                return;
            }
            
            int counter = 1;
            foreach (var toDoItem in userToDoItemList)
            {
                _botClient.SendMessage(_update.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.State} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                counter++;
            }
        }

        private void CommandRemoveTask(string taskNo)
        {
            var userToDoItemList = _toDoService.GetActiveByUserId(_toDoUser.UserId);
            
            if (userToDoItemList.Count == 0)
            {
                _botClient.SendMessage(_update.Message.Chat, "Список задач пуст");
                return;
            }
            
            var taskNoInt = ParseAndValidateInt(taskNo, 1, userToDoItemList.Count);

            _toDoService.Delete(userToDoItemList[taskNoInt - 1].Id);
            _botClient.SendMessage(_update.Message.Chat, $"Задача с номером {taskNoInt} удалена");
        }

        private void CommandExit()
        {
            _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser)}");
            Console.Read();
        }

        private string GetFullOutput(string request, ToDoUser? toDoUser)
        {
            if (toDoUser == null)
                return request;
            else
                return $"{toDoUser.TelegramUserName}, {request.ToLower()}";
        }
        private int ParseAndValidateInt(string? str, int min, int max)
        {
            if ((!int.TryParse(str, out int result)) || (result < min) || (result > max))
            {
                throw new ArgumentException();
            }
            return result;
        }
    }
}
