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
        private List<ToDoItem> _toDoItemList;
        public InMemoryToDoRepository()
        {
            _toDoItemList = new List<ToDoItem>();
        }
        public void Add(ToDoItem item)
        {
            _toDoItemList.Add(item);
        }

        public int CountActive(Guid userId)
        {
            return _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).Count();
        }

        public void Delete(Guid id)
        {
            var toDoItem = Get(id);
            if (toDoItem != null)
            {
                _toDoItemList.Remove(toDoItem);
            }
        }

        public bool ExistsByName(Guid userId, string name)
        {
            return _toDoItemList.Where(x => x.User.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0;
        }
       
        public ToDoItem? Get(Guid id)
        {
            return _toDoItemList.Where(x => x.Id == id).FirstOrDefault();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoItemList.Where(x => x.User.UserId == userId).ToList();
        }

        public void Update(ToDoItem item)
        {
                item.State = ToDoItemState.Completed;
                item.StateChangedAt = DateTime.Now;
        }
        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return _toDoItemList.Where(predicate).ToList();
        }
    }
}
