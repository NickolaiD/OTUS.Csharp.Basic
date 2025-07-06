using Otus.ToDoList.ConsoleBot;

namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            var handler = new UpdateHandler();
            var botClient = new ConsoleBotClient();
            
            try
            {
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
