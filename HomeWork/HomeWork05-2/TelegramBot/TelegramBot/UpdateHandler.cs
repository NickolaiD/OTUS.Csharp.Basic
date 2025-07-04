using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBot
{
    class UpdateHandler : IUpdateHandler
    {
        private ToDoUser? _toDoUser;
        private List<ToDoItem> _toDoItemList;
        private int _taskCountLimit;
        private int _taskLengthLimit;

        private ITelegramBotClient _botClient;
        private Update _update;

        public UpdateHandler()
        {
            _toDoItemList = new List<ToDoItem>();
            _toDoUser = new ToDoUser("");
            _taskCountLimit = 5;
            _taskLengthLimit = 5;
        }
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            _botClient = botClient;
            _update = update;

            bool doContinue = true, firstRun = true;
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");
            /*
            if (firstRun)
            {
                Console.WriteLine("Введите максимально допустимое количество задач");
                _taskCountLimit = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.WriteLine("Введите максимально допустимую длину задачи");
                _taskLengthLimit = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.WriteLine(@"Добро пожаловать! Доступные команды: /start, /help, /info, /echo, /addtask, /showtasks, /showalltasks, /completetask /removetask, /exit");
                firstRun = false;
            }
            else
            {
                Console.WriteLine();
                Console.Write("Введите команду: ");
                userCommand = Console.ReadLine() ?? "";
                ValidateString(userCommand);
            }
            */
            doContinue = ExecuteCommand(update.Message.Text);

        }

        private bool ExecuteCommand(string userCommand)
        {
            if (userCommand == string.Empty)
                return true;

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

                case "/echo":
                    CommandEcho(parameter);
                    break;

                case "/addtask":
                    CommandAddTask();
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
                    CommandRemoveTask();
                    break;

                case "/exit":
                    CommandExit();
                    return false;

                default:
                    _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser.TelegramUserName)}");
                    break;
            }

            return true;
        }

        private void CommandStart()
        {
            _botClient.SendMessage(_update.Message.Chat, "Введите свое имя: ");
            var userName = Console.ReadLine() ?? "";
            ValidateString(userName);
            _toDoUser = new ToDoUser(userName);
            _botClient.SendMessage(_update.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}! Чем могу помочь?");
        }

        private void CommandHelp()
        {
            _botClient.SendMessage(_update.Message.Chat, @$"
{GetFullOutput("Справочная информация:", _toDoUser.TelegramUserName)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/echo - программа возвращает введенный текст (например, /echo Hello)
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
            _botClient.SendMessage(_update.Message.Chat, GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _toDoUser.TelegramUserName));
        }

        private void CommandEcho(string parameter)
        {
            if (_toDoUser.TelegramUserName == string.Empty)
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Для использования команды /echo сначала выполните /start и введите имя", _toDoUser.TelegramUserName)}");
            else
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput($"Введенный текст: {parameter}", _toDoUser.TelegramUserName)}");

        }

        private void CommandAddTask()
        {
            if (_toDoItemList.Count >= _taskCountLimit)
                throw new TaskCountLimitException(_taskCountLimit);

            _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Введите название задачи:", _toDoUser.TelegramUserName)}");
            var toDoItemName = Console.ReadLine() ?? "";
            ValidateString(toDoItemName);

            if (toDoItemName.Length > _taskLengthLimit)
                throw new TaskLengthLimitException(toDoItemName.Length, _taskLengthLimit);

            foreach (var toDoItem in _toDoItemList)
            {
                if (toDoItem.Name == toDoItemName)
                    throw new DuplicateTaskException(toDoItemName);
            }

            _toDoItemList.Add(new ToDoItem(_toDoUser, toDoItemName));
            _botClient.SendMessage(_update.Message.Chat, "Задача добавлена");
        }

        private void CommandCompleteTask(string parameter)
        {
            if (_toDoItemList.Count > 0)
            {
                foreach (var toDoItem in _toDoItemList)
                {
                    if ((toDoItem.State == ToDoItemState.Active) && (toDoItem.Id.ToString() == parameter))
                    {
                        toDoItem.State = ToDoItemState.Completed;
                        toDoItem.StateChangedAt = DateTime.Now;

                        _botClient.SendMessage(_update.Message.Chat, GetFullOutput($"Задача завершена - {toDoItem.Name} - {toDoItem.Id}", _toDoUser.TelegramUserName));
                        return;

                    }
                }
                _botClient.SendMessage(_update.Message.Chat, GetFullOutput($"Задача с Id {parameter} не найдена", _toDoUser.TelegramUserName));
            }
            else
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser.TelegramUserName)}");

        }
        private void CommandShowTasks()
        {
            if (_toDoItemList.Count > 0)
            {
                int counter = 1;
                foreach (var toDoItem in _toDoItemList)
                {
                    if (toDoItem.State == ToDoItemState.Active)
                    {
                        _botClient.SendMessage(_update.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                        counter++;
                    }
                }
                if (counter == 1)
                {
                    _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Активных задач нет", _toDoUser.TelegramUserName)}");
                }
            }
            else
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser.TelegramUserName)}");
        }

        private void CommandShowAllTasks()
        {
            if (_toDoItemList.Count > 0)
            {
                int counter = 1;
                foreach (var toDoItem in _toDoItemList)
                {
                    _botClient.SendMessage(_update.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.State} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                    counter++;
                }
            }
            else
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser.TelegramUserName)}");
        }

        private void CommandRemoveTask()
        {
            CommandShowTasks();

            if (_toDoItemList.Count == 0)
                return;

            _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Введите номер задачи для удаления:", _toDoUser.TelegramUserName)}");
            var taskNo = Console.ReadLine() ?? "";
            var taskNoInt = ParseAndValidateInt(taskNo, 1, _toDoItemList.Count);

            _toDoItemList.RemoveAt(taskNoInt - 1);
            _botClient.SendMessage(_update.Message.Chat, $"Задача с номером {taskNoInt} удалена");
        }

        private void CommandExit()
        {
            _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser.TelegramUserName)}");
            Console.Read();
        }

        private string GetFullOutput(string request, string userName)
        {
            if (userName == string.Empty)
                return request;
            else
                return $"{userName}, {request.ToLower()}";
        }
        private int ParseAndValidateInt(string? str, int min, int max)
        {
            if ((!int.TryParse(str, out int result)) || (result < min) || (result > max))
            {
                throw new ArgumentException();
            }
            return result;
        }

        private void ValidateString(string? str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                foreach (var item in str)
                {
                    if (!char.IsWhiteSpace(item))
                    {
                        return;
                    }
                }
            }

            throw new ArgumentException();
        }
    }
}
