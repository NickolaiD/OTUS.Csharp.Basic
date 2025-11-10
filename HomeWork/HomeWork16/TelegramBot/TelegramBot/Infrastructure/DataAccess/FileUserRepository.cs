using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class FileUserRepository : IUserRepository
    {
        private readonly string _directoryName;
        public FileUserRepository(string baseDirectoryName)
        {
            _directoryName = Path.Combine(baseDirectoryName, "ToDoUsers");
            if (!Directory.Exists(_directoryName))
            {
                Directory.CreateDirectory(_directoryName);
            }
        }
        public async Task AddAsync(ToDoUser user, CancellationToken ct)
        {
            if (await GetUserByTelegramUserIdAsync(user.TelegramUserId, ct) == null)
            {
                string fileName = Path.Combine(_directoryName, $"{user.UserId}.json");
                using var createStream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(createStream, user, cancellationToken: ct);
            }
        }

        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
        {
            var toDoUsers = await GetAllUsersAsync(ct);
            return await Task.Run(() => toDoUsers.Where(x => x.UserId == userId).FirstOrDefault());
        }

        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct)
        {
            var toDoUsers = await GetAllUsersAsync(ct);
            return await Task.Run(() => toDoUsers.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault(), ct);
        }

        public async Task<IReadOnlyList<ToDoUser>> GetAllUsersAsync(CancellationToken ct)
        {
            var userList = new List<ToDoUser>();
            
            if (Directory.Exists(_directoryName))
            {
                var userFiles = Directory.EnumerateFiles(_directoryName);
                foreach (var file in userFiles)
                {
                    using var reader = File.OpenRead(file);
                    var toDoUser = await JsonSerializer.DeserializeAsync<ToDoUser>(reader, cancellationToken: ct);
                    if (toDoUser != null)
                    {
                        userList.Add(toDoUser);
                    }
                }
            }
            return userList;
        }

        public Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
        {
            //класс использовался в более ранних ДЗ, сейчас метод не нужен.
            throw new NotImplementedException();
        }
    }
}
