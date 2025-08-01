using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _toDoItemList;
        public InMemoryToDoRepository()
        {
            _toDoItemList = new List<ToDoItem>();
        }
        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
           await Task.Run(() => _toDoItemList.Add(item));
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).Count());
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var toDoItem = await GetAsync(id, ct);
            if (toDoItem != null)
            {
                await Task.Run(() => _toDoItemList.Remove(toDoItem));
            }
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0);
        }
       
        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.Id == id).FirstOrDefault());
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList());
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId).ToList());
        }

        public void Update(ToDoItem item)
        {
                item.State = ToDoItemState.Completed;
                item.StateChangedAt = DateTime.Now;
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId).Where(predicate).ToList());
        }
    }
}
