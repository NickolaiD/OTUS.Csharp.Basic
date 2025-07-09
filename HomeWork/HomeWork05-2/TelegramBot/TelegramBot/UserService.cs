namespace TelegramBot
{
    internal class UserService : IUserService
    {
        private List<ToDoUser> toDoUsers;
        public UserService()
        {
            toDoUsers = new List<ToDoUser>();
        }
        public ToDoUser? GetUser(long telegramUserId)
        {
            foreach (var user in toDoUsers)
            {
                if (user.TelegramUserId == telegramUserId)
                    return user;
            }
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var user = new ToDoUser(telegramUserName, telegramUserId);
            toDoUsers.Add(user);

            return user;
        }
    }
}
