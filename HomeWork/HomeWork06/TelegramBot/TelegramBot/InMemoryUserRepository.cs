using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> toDoUsers;
        public InMemoryUserRepository()
        {
            toDoUsers = new List<ToDoUser>();
        }
        public void Add(ToDoUser user)
        {
            if (GetUserByTelegramUserId(user.TelegramUserId) == null )
              toDoUsers.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            return toDoUsers.Where(x => x.UserId == userId).FirstOrDefault();
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            return toDoUsers.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault();
        }
    }
}
