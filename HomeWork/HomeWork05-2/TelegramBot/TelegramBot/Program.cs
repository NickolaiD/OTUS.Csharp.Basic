using Otus.ToDoList.ConsoleBot;
using System.Diagnostics;
using System.Reflection.Metadata;

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
        

        
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }





    }
}
