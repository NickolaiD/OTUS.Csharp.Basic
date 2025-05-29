namespace TelegramBot
{
    internal class Program
    {
        static string userName = "";
        static void Main(string[] args)
        {
            string userCommand;
            bool doContinue, firstRun = true;

            do
            {
                if (firstRun)
                {
                    userCommand = "/help";
                    Console.WriteLine("Добро пожаловать!");
                    firstRun = false;
                }
                else
                {
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

            int spacePosition = userCommand.IndexOf(" ");
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
                    userName = Console.ReadLine() ?? "";
                    break;

                case "/help":
                    Console.WriteLine(@$"
{GetFullOutput("Справочная информация:", userName)}
/start - начало работы
/help - справочная информация
/info - информация о версии программы и дате её создания
/echo - программа возвращает введенный текст (например, /echo Hello)
/exit - завершение работы"
                    );
                    break;

                case "/info":
                    Console.WriteLine(GetFullOutput("Версия бота: 1.0, дата создания 20.05.2025", userName));
                    break;

                case "/echo":
                    if (userName == string.Empty)
                        Console.WriteLine($"{GetFullOutput("Для использования команды /echo сначала выполните /start и введите имя", userName)}");
                    else
                        Console.WriteLine($"{GetFullOutput($"Введенный текст: {parameter}", userName)}");
                    break;

                case "/exit":
                    Console.WriteLine($"{GetFullOutput("Завершение работы.", userName)}");
                    Console.Read();
                    return false;

                default:
                    Console.WriteLine($"{GetFullOutput($"Команда {command} не существует", userName)}");
                    break;
            }

            Console.WriteLine("----------------------\n");

            return true;
        }

        private static string GetFullOutput(string request, string userName)
        {
            if (userName == string.Empty)
                return request;
            else
                return $"{userName}, {request.ToLower()}";
        }
    }
}
