using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class ToDoUser
    {
        private Guid _userId;
        private string _telegramUserName;
        private DateTime _registeredAt;

        public ToDoUser(string telegramUserName)
        {
            _userId = Guid.NewGuid();
            _registeredAt = DateTime.UtcNow;
            _telegramUserName = telegramUserName;

        }

    }
}
