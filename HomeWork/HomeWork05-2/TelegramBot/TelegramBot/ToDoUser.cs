﻿namespace TelegramBot
{
    internal class ToDoUser
    {
        public Guid UserId { get; }
        public string TelegramUserName { get;}
        public DateTime RegisteredAt { get; }
        public long TelegramUserId { get; }
        public ToDoUser(string telegramUserName, long telegramUserId)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.UtcNow;
            TelegramUserName = telegramUserName;
            TelegramUserId = telegramUserId;
        }

    }
}
