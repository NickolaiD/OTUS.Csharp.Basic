using System.Diagnostics;
using System.Reflection.Metadata;

namespace TelegramBot
{
    internal class Program
    {
        private static string _userName = "";
        private static List<string> _taskList = new List<string>();
        private static int _taskCountLimit = 0;
        private static int _taskLengthLimit = 0;
        static void Main()
        {
            string userCommand = "";
            bool doContinue = true, firstRun = true;
            
            do
            {
                try
                {
                    if (firstRun)
                    {
                        Console.WriteLine("Введите максимально допустимое количество задач");
                        _taskCountLimit = ParseAndValidateInt(Console.ReadLine(), 0, 100);

                        Console.WriteLine("Введите максимально допустимую длину задачи");
                        _taskLengthLimit = ParseAndValidateInt(Console.ReadLine(), 0, 100);

                        Console.WriteLine(@"Добро пожаловать! Доступные команды: /start, /help, /info, /echo, /addtask, /showtasks, /removetask, /exit");
                        firstRun = false;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write("Введите команду: ");
                        userCommand = Console.ReadLine() ?? "";
                        ValidateString(userCommand);
                    }

                    doContinue = ExecuteCommand(userCommand);
                }
                
                catch (ArgumentException ex)
                {
                    ShowError(ex.Message);
                }

                catch (TaskCountLimitException ex)
                {
                    ShowError(ex.Message);
                }

                catch (TaskLengthLimitException ex)
                {
                    ShowError(ex.Message);
                }

                catch (DuplicateTaskException ex)
                {
                    ShowError(ex.Message);
                }

                catch (Exception ex)
                {
                    ShowError($"Произошла непредвиденная ошибка:{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
                }
            }
            while (doContinue);
        }

        private static bool ExecuteCommand(string userCommand)
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
                
                case "/showtasks":
                    CommandShowTasks();
                    break;
                
                case "/removetask":
                    CommandRemoveTask();
                    break;

                case "/exit":
                    CommandExit();
                    return false;

                default:
                    Console.WriteLine($"{GetFullOutput($"Команда {command} не существует", _userName)}");
                    break;
            }

            return true;
        }

        private static string GetFullOutput(string request, string userName)
        {
            if (userName == string.Empty)
                return request;
            else
                return $"{userName}, {request.ToLower()}";
        }
        
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static int ParseAndValidateInt(string? str, int min, int max)
        {
            if ((!int.TryParse(str, out int result)) || (result <= min) || (result > 100))
            {
                throw new ArgumentException();
            }
            return result;
        }

        private static void ValidateString(string? str)
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

        private static void CommandStart()
        {
            Console.Write("Введите свое имя: ");
            _userName = Console.ReadLine() ?? "";
            ValidateString(_userName);
            Console.WriteLine($"Привет, {_userName}! Чем могу помочь?");
        }

        private static void CommandHelp()
        {
            Console.WriteLine(@$"
{GetFullOutput("Справочная информация:", _userName)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/echo - программа возвращает введенный текст (например, /echo Hello)
/addtask - добавить задачу в список
/showtasks - показать список задач
/removetask - удалить задачу из списка
/exit - завершение работы"
);
        }

        private static void CommandInfo()
        {
            Console.WriteLine(GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _userName));
        }

        private static void CommandEcho(string parameter)
        {
            if (_userName == string.Empty)
                Console.WriteLine($"{GetFullOutput("Для использования команды /echo сначала выполните /start и введите имя", _userName)}");
            else
                Console.WriteLine($"{GetFullOutput($"Введенный текст: {parameter}", _userName)}");

        }

        private static void CommandAddTask()
        {
            if (_taskList.Count >= _taskCountLimit)
                throw new TaskCountLimitException(_taskCountLimit);

            Console.WriteLine($"{GetFullOutput("Введите название задачи:", _userName)}");
            var taskName = Console.ReadLine() ?? "";
            ValidateString(taskName);

            if (taskName.Length > _taskLengthLimit)
                throw new TaskLengthLimitException(taskName.Length, _taskLengthLimit);

            foreach (var task in _taskList)
            {
                if (task == taskName)
                    throw new DuplicateTaskException(taskName);
            }

            _taskList.Add(taskName);
            Console.WriteLine("Задача добавлена");
        }
        private static void CommandShowTasks()
        {
            if (_taskList.Count > 0)
            {
                int counter = 1;
                foreach (var task in _taskList)
                {
                    Console.WriteLine($"{counter} - {task}");
                    counter++;
                }
            }
            else
                Console.WriteLine($"{GetFullOutput("Список задач пуст", _userName)}");
        }
        
        private static void CommandRemoveTask()
        {
            CommandShowTasks();

            if (_taskList.Count == 0)
                return;

            Console.WriteLine($"{GetFullOutput("Введите номер задачи для удаления:", _userName)}");
            var taskNo = Console.ReadLine() ?? "";
            ValidateString(taskNo);

            if (int.TryParse(taskNo, out int taskNoInt))
            {
                if ((taskNoInt > 0) && (taskNoInt <= _taskList.Count))
                {
                    _taskList.RemoveAt(taskNoInt - 1);
                    Console.WriteLine($"Задача с номером {taskNoInt} удалена");
                }
                else
                    Console.WriteLine($"Элемент с номером {taskNoInt} не существует");

            }
            else
                Console.WriteLine("Введен некорректный номер задачи");

        }

        private static void CommandExit()
        {
            Console.WriteLine($"{GetFullOutput("Завершение работы.", _userName)}");
            Console.Read();
        }

    }
}
