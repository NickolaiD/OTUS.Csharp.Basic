using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    public class FileToDoListRepository : IToDoListRepository
    {
        private readonly string _directoryName;
        public FileToDoListRepository(string baseDirectoryName)
        {
            _directoryName = Path.Combine(baseDirectoryName, "ToDoLists");
            if (!Directory.Exists(_directoryName))
            {
                Directory.CreateDirectory(_directoryName);
            }
        }
        public async Task Add(ToDoList list, CancellationToken ct)
        {
            string fileName = Path.Combine(_directoryName, $"{list.Id}.json");
            using var createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, list, cancellationToken: ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            var toDoLists = await GetAllToDoListsAsync(ct);
            var fileItem = toDoLists.Where(x => x.Id == id).FirstOrDefault();

            if (fileItem != null)
            {
                var fileNameToDel = Path.Combine(_directoryName, fileItem.Id.ToString(), $"{fileItem.Id}.json");
                if (File.Exists(fileNameToDel))
                {
                    File.Delete(fileNameToDel);
                }
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var toDoLists = await GetAllToDoListsAsync(ct);
            return toDoLists.Where(x => x.Name == name && x.User.UserId == userId).Count() > 0;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            var toDoLists = await GetAllToDoListsAsync(ct);
            return toDoLists.Where(x => x.Id == id).FirstOrDefault();
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            var toDoLists = await GetAllToDoListsAsync(ct);
            return toDoLists.Where(x => x.User.UserId == userId).ToList();
        }

        public async Task<IReadOnlyList<ToDoList>> GetAllToDoListsAsync(CancellationToken ct)
        {
            var userList = new List<ToDoList>();

            if (Directory.Exists(_directoryName))
            {
                var toDoListFiles = Directory.EnumerateFiles(_directoryName);
                foreach (var file in toDoListFiles)
                {
                    using var reader = File.OpenRead(file);
                    var toDoList = await JsonSerializer.DeserializeAsync<ToDoList>(reader, cancellationToken: ct);
                    if (toDoList != null)
                    {
                        userList.Add(toDoList);
                    }
                }
            }
            return userList;
        }
    }
}
