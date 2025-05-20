using System.Runtime.CompilerServices;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var userName = "";
            while (true) {
                var userCommand = Console.ReadLine();

                switch (userCommand)
                {
                    case "/start":
                        Console.Write("Введите свое имя: ");
                        userName = Console.ReadLine();
                        break;

                    case "/help":
                        Console.WriteLine("Справочная информация:");
                        Console.WriteLine("/start - начало работы");
                        Console.WriteLine("/help - справочная информация");
                        Console.WriteLine("/info - информация о версии программы и дате её создания");
                        Console.WriteLine("/echo - программа возвращает введенный текст (например, /echo Hello)");
                        Console.WriteLine("/exit - завершение работы");
                        break;

                    case "/info":
                        ;
                        break;

                    case "/echo":
                        ;
                        break;

                    case "/exit": 
                        break;

                    default:
                        Console.WriteLine("Команда не существует");
                        break;
                }

            }
        }

        private string GetUserName(string userName)
        {
            return userName != "" ? userName : "Пользователь";
        }
    }
}
