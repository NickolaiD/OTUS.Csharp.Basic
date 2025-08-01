﻿using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
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
