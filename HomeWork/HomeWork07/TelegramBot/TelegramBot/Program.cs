using Otus.ToDoList.ConsoleBot;
using TelegramBot.Infrastructure.DataAccess;
using TelegramBot.Services;

namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Введите максимально допустимое количество задач");
                var taskCountLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.WriteLine("Введите максимально допустимую длину задачи");
                var taskLengthLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

                var botClient = new ConsoleBotClient();
                var toDoRepository = new InMemoryToDoRepository();
                var handler = new UpdateHandler(new UserService(),
                                                botClient,
                                                new ToDoService(taskCountLimit, taskLengthLimit, toDoRepository),
                                                new ToDoReportService(toDoRepository));
            
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            {
                ShowError($"Произошла непредвиденная ошибка:{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
            }
        }
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
