using Otus.ToDoList.ConsoleBot.Types;
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

            return null; // заглушка
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            return new ToDoUser (telegramUserName, telegramUserId);  // заглушка
            
        }
    }
}
