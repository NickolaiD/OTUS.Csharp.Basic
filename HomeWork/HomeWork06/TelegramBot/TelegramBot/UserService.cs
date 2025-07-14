namespace TelegramBot
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService()
        {
            userRepository = new InMemoryUserRepository();
        }
        public ToDoUser? GetUser(long telegramUserId)
        {
            return userRepository.GetUserByTelegramUserId(telegramUserId);
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var user = new ToDoUser(telegramUserName, telegramUserId);
            userRepository.Add(user);
            return user;
        }
    }
}
