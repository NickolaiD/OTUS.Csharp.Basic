using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Entities.Index;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private readonly string _directoryName;
        private readonly string _indexFileName;
        public FileToDoRepository(string baseDirectoryName)
        {
            _directoryName = Path.Combine(baseDirectoryName, "ToDoItems");
            _indexFileName = Path.Combine(baseDirectoryName, "Index.json");

            if (!Directory.Exists(_directoryName))
            {
                Directory.CreateDirectory(_directoryName);
            }
            
            if (!File.Exists(_indexFileName)) 
            {
                var fileIndex = new List<IndexItem>();

                var userDirectoryName = Path.Combine(baseDirectoryName, "ToDoItems");
                var userFolders = Directory.EnumerateDirectories(userDirectoryName);
                foreach (var folder in userFolders)
                {
                    var itemFiles = Directory.EnumerateFiles(folder);
                    foreach (var file in itemFiles)
                    {
                        using var reader = File.OpenRead(file);
                        var item = JsonSerializer.Deserialize<ToDoItem>(reader);
                        if (item != null)
                        {
                            fileIndex.Add(new IndexItem(item.Id, item.User.UserId));
                        }
                    }
                }

                using var writerIndex = File.Create(_indexFileName);
                JsonSerializer.Serialize(writerIndex, fileIndex);
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

            var fileIndex = await GetFileIndexList(ct);
            
            fileIndex.Add(new IndexItem(item.Id, item.User.UserId));

            using var writerIndex = File.Create(_indexFileName);
            await JsonSerializer.SerializeAsync(writerIndex, fileIndex, cancellationToken: ct);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {

            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).Count();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var fileIndex = await GetFileIndexList(ct);

            var fileItem = fileIndex.Where(x => x.ToDoItemId == id).FirstOrDefault();
            if (fileItem != null)
            {
                var fileNameToDel = Path.Combine(_directoryName, fileItem.UserId.ToString(), $"{fileItem.ToDoItemId}.json");
                if (File.Exists(fileNameToDel))
                {
                    File.Delete(fileNameToDel);
                }
                
                fileIndex.RemoveAll(x => x.ToDoItemId == id);
                using var writerIndex = File.Create(_indexFileName);
                await JsonSerializer.SerializeAsync(writerIndex, fileIndex, cancellationToken: ct);
            }
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return toDoItemList.Where(x => x.User.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0;
        }

        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
        {
            var fileIndex = await GetFileIndexList(ct);
            var fileItem = fileIndex.Where(x => x.ToDoItemId == id).FirstOrDefault();
            ToDoItem? toDoItem = null;

            if (fileItem != null)
            {
                var fileName = Path.Combine(_directoryName, fileItem.UserId.ToString(), $"{fileItem.ToDoItemId}.json");
                if (File.Exists(fileName))
                {
                    using var reader = File.OpenRead(fileName);
                    toDoItem = await JsonSerializer.DeserializeAsync<ToDoItem>(reader, cancellationToken: ct);
                }
            }
            return toDoItem;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            var toDoItemList = new List<ToDoItem>();
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
            var fileName = Path.Combine(_directoryName, item.User.UserId.ToString(), $"{item.Id}.json");
            ToDoItem? toDoItem = null;
            if (File.Exists(fileName))
            {
                using (var reader = File.OpenRead(fileName))
                {
                    toDoItem = JsonSerializer.Deserialize<ToDoItem>(reader);
                }

                if (toDoItem != null)
                {
                    toDoItem.State = ToDoItemState.Completed;
                    toDoItem.StateChangedAt = DateTime.Now;

                    using var writer = File.Create(fileName);
                    JsonSerializer.Serialize(writer, toDoItem);
                }
            }

        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            return toDoItemList.Where(x => x.User.UserId == userId).Where(predicate).ToList();
        }
        public async Task<List<IndexItem>> GetFileIndexList(CancellationToken ct)
        {
            List<IndexItem>? fileIndex;

            if (new FileInfo(_indexFileName).Length != 0)
            {
                using var readerIndex = File.OpenRead(_indexFileName);
                fileIndex = await JsonSerializer.DeserializeAsync<List<IndexItem>>(readerIndex, cancellationToken: ct);
                if (fileIndex == null)
                {
                    return new List<IndexItem>();
                }
            }
            else
            {
                return new List<IndexItem>();
            }
            return fileIndex;
        }
        public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct)
        {
            var toDoItemList = await GetAllByUserIdAsync(userId, ct);
            if (listId == null)
            {
                return toDoItemList.Where(x => x.User.UserId == userId && x.List == null).ToList();
            }
            else
            {
                return toDoItemList.Where(x => x.User.UserId == userId && x.List != null).Where(x => x.List.Id == listId).ToList();
            }
        }

    }
}
