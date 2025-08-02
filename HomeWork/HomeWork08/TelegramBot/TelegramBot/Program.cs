using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot.Infrastructure.DataAccess;
using TelegramBot.Services;

namespace TelegramBot
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Введите максимально допустимое количество задач");
            var taskCountLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

            Console.WriteLine("Введите максимально допустимую длину задачи");
            var taskLengthLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);
            string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User) ?? "";
            var botClient = new TelegramBotClient(token);

            var ping = await botClient.GetMe();

            Console.WriteLine($"Bot is alive - {ping.FirstName} {ping.Username}");

            var toDoRepository = new InMemoryToDoRepository();
            var cts = new CancellationTokenSource();
            var handler = new UpdateHandler(new UserService(),
                                            botClient,
                                            new ToDoService(taskCountLimit, taskLengthLimit, toDoRepository),
                                            new ToDoReportService(toDoRepository));

            try
            {
                handler.SubscribeOnUpdateStarted(UpdateStarted);
                handler.SubscribeOnUpdateCompleted(UpdateCompleted);
                botClient.StartReceiving(handler, cancellationToken:cts.Token);

                var keyPressTask = Task.Run(() => KeyPress(botClient, cts.Token));

                await Task.WhenAny(keyPressTask);
                if (keyPressTask.IsCompleted)
                {
                    cts.Cancel();
                    Console.WriteLine("\nЗавершение работы бота...");
                }
            }
            catch (OperationCanceledException e)
            {
                ShowError("Бот остановлен");
            }
            catch (Exception ex)
            {
                ShowError($"Произошла непредвиденная ошибка:{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
            }
            finally
            {
                handler.UnSubscribeOnUpdateStarted(UpdateStarted);
                handler.UnSubscribeOnUpdateCompleted(UpdateCompleted);
            }
        }

        private async static Task KeyPress(TelegramBotClient bot, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.A)
                    {
                        return;
                    }
                    else
                    {
                        // Получаем информацию о боте при нажатии любой другой клавиши
                        var me = await bot.GetMe();
                        Console.WriteLine($"\nИнформация о боте: @{me.Username}, ID: {me.Id}");
                        Console.WriteLine("Нажмите клавишу A для выхода");
                    }
                await Task.Delay(100);
            }
        }
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void UpdateStarted(string message)
        {
            Console.WriteLine($"Началась обработка сообщения {message}");
        }

        private static void UpdateCompleted(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения {message}");
        }
    }
}
