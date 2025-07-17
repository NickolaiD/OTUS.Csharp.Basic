using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> _toDoUsers;
        public InMemoryUserRepository()
        {
            _toDoUsers = new List<ToDoUser>();
        }
        public void Add(ToDoUser user)
        {
            if (GetUserByTelegramUserId(user.TelegramUserId) == null )
              _toDoUsers.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            return _toDoUsers.Where(x => x.UserId == userId).FirstOrDefault();
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            return _toDoUsers.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault();
        }
    }
}
