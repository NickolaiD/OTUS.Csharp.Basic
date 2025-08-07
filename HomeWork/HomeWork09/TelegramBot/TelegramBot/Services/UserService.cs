using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService()
        {
            userRepository = new InMemoryUserRepository();
        }
        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken ct)
        {
            return await Task.Run(() => userRepository.GetUserByTelegramUserIdAsync(telegramUserId, ct));
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            var user = new ToDoUser(telegramUserName, telegramUserId);
            await userRepository.AddAsync(user, ct);
            return user;
        }
    }
}
