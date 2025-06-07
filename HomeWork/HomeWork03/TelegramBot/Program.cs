using System.Diagnostics;

namespace TelegramBot
{
    internal class Program
    {
        private static string _userName = "";
        private static List<string> _taskList = new List<string>();
        static void Main()
        {
            string userCommand = "";
            bool doContinue, firstRun = true;

            do
            {
                if (firstRun)
                {
                    Console.WriteLine(@"Добро пожаловать! Доступные команды: /start, /help, /info, /echo, /addtask, /showtasks, /removetask, /exit");
                    firstRun = false;
                }
                else
                {
                    Console.WriteLine();
                    Console.Write($"Введите команду: ");
                    userCommand = Console.ReadLine() ?? "";
                }

                doContinue = ExecuteCommand(userCommand);
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
                    Console.Write("Введите свое имя: ");
                    _userName = Console.ReadLine() ?? "";
                    
                    if (_userName != String.Empty)
                        Console.WriteLine($"Привет, {_userName}! Чем могу помочь?");
                    
                    break;

                case "/help":
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
                    break;

                case "/info":
                    Console.WriteLine(GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", _userName));
                    break;

                case "/echo":
                    if (_userName == string.Empty)
                        Console.WriteLine($"{GetFullOutput("Для использования команды /echo сначала выполните /start и введите имя", _userName)}");
                    else
                        Console.WriteLine($"{GetFullOutput($"Введенный текст: {parameter}", _userName)}");
                    break;
                
                case "/addtask":
                    Console.WriteLine($"{GetFullOutput("Введите название задачи:", _userName)}");
                    var taskName = Console.ReadLine() ?? "";
                    if (taskName != String.Empty)
                        _taskList.Add(taskName);
                    Console.WriteLine("Задача добавлена");
                    break;
                
                case "/showtasks":
                    ShowTasks();
                    break;
                
                case "/removetask":
                    ShowTasks();

                    if (_taskList.Count == 0)
                        return true;

                    Console.WriteLine($"{GetFullOutput("Введите номер задачи для удаления:", _userName)}");
                    var taskNo = Console.ReadLine() ?? "";
                    if (int.TryParse(taskNo, out int taskNoInt)) {
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
                    break;

                case "/exit":
                    Console.WriteLine($"{GetFullOutput("Завершение работы.", _userName)}");
                    Console.Read();
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

        private static void ShowTasks()
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
    }
}
