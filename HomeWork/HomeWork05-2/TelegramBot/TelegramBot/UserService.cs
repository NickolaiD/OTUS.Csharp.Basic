namespace TelegramBot
{
    internal class UserService : IUserService
    {
        public ToDoUser? GetUser(long telegramUserId)
        {
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            return new ToDoUser (telegramUserName, telegramUserId);
        }
    }
}
