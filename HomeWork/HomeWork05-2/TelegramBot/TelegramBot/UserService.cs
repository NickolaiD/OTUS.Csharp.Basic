using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class UserService : IUserService
    {
        public ToDoUser? GetUser(long telegramUserId)
        {
            
            throw new NotImplementedException();
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            throw new NotImplementedException();
        }
    }
}
