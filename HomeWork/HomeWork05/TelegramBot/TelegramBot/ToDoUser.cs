﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class ToDoUser
    {
        public Guid UserId { get; }
        public string TelegramUserName { get;}
        public DateTime RegisteredAt { get; }

        public ToDoUser(string telegramUserName)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.UtcNow;
            TelegramUserName = telegramUserName;

        }

    }
}
