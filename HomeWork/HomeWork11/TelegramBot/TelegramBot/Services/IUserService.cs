using TelegramBot.Entities;

namespace TelegramBot.Services
{
    interface IUserService
    {
        Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct);
        Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken ct);
    }
}
