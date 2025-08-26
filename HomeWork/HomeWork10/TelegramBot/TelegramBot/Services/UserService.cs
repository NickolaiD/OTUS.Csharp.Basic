using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService()
        {
            //userRepository = new InMemoryUserRepository();
            _userRepository = new FileUserRepository(BotHelper.BASE_DIR);
        }
        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken ct)
        {
            return await Task.Run(() => _userRepository.GetUserByTelegramUserIdAsync(telegramUserId, ct));
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            var user = new ToDoUser(telegramUserName, telegramUserId);
            await _userRepository.AddAsync(user, ct);
            return user;
        }
    }
}
