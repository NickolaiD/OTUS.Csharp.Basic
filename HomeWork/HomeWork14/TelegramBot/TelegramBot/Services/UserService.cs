using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Helpers;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService()
        {
            _userRepository = new SqlUserRepository(new DataContextFactory());
        }
        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken ct)
        {
            return await Task.Run(() => _userRepository.GetUserByTelegramUserIdAsync(telegramUserId, ct));
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            var user = new ToDoUser() 
            { 
                UserId = Guid.NewGuid(), 
                TelegramUserName = telegramUserName, 
                TelegramUserId = telegramUserId,
                RegisteredAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, ct);
            return user;
        }
    }
}
