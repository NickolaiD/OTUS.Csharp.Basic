using TelegramBot.Entities;

namespace TelegramBot.Services
{
    internal interface IUserService
    {
        Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, long ChatId, CancellationToken ct);
        Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken ct);
        Task<ToDoUser?> GetUserByIdAsync(Guid userId, CancellationToken ct);

    }
}
