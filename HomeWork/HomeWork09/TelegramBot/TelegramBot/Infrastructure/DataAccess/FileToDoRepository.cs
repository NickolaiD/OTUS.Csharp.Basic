using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private readonly string _directoryName;
        public FileToDoRepository(string baseDirectoryName)
        {
            if (string.IsNullOrEmpty(baseDirectoryName))
            {
                throw new ArgumentNullException("Задан пустой путь для хранения файлов");
            }
            _directoryName = Path.Combine(baseDirectoryName, "ToDoItems");
            if (!Directory.Exists(_directoryName))
            {
                Directory.CreateDirectory(_directoryName);
            }
        }
        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
             string userDirectory = Path.Combine(_directoryName, item.User.UserId.ToString());
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            string fileName = Path.Combine(userDirectory, $"{item.Id}.json");
            using var createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, item, cancellationToken: ct);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {

            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return await Task.Run(() => toDoItemList.Where(x => x.State == ToDoItemState.Active).Count());
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            //var toDoItem = await GetAsync(id, ct);
            //if (toDoItem != null)
            //{
            //    await Task.Run(() => _toDoItemList.Remove(toDoItem));
            //}
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return await Task.Run(() => toDoItemList.Where(x => x.Name == name && x.State == ToDoItemState.Active).Count() > 0);
        }

        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
        {
            //return await Task.Run(() => _toDoItemList.Where(x => x.Id == id).FirstOrDefault());
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return await Task.Run(() => toDoItemList.Where(x => x.State == ToDoItemState.Active).ToList());
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            List<ToDoItem> toDoItemList = new List<ToDoItem>();
            string userDirectory = Path.Combine(_directoryName, userId.ToString());
            if (Directory.Exists(userDirectory))
            {
                var userFiles = Directory.EnumerateFiles(userDirectory);
                foreach (var file in userFiles)
                {
                    using var reader = File.OpenRead(file);
                    var item = await JsonSerializer.DeserializeAsync<ToDoItem>(reader, cancellationToken: ct);
                    if (item != null)
                    {
                        toDoItemList.Add(item);
                    }
                }
            }
            return toDoItemList;
        }
        public void Update(ToDoItem item)
        {
            //item.State = ToDoItemState.Completed;
            //item.StateChangedAt = DateTime.Now;
            throw new NotImplementedException();
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return await Task.Run(() => toDoItemList.Where(predicate).ToList());
        }

    }
}
