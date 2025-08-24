using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Entities.Index
{
    internal class FileIndex
    {
        public List<IndexItem> Items { get; init; } 
        public FileIndex()
        {
            Items = new List<IndexItem>();
        }
        public void Add(IndexItem item)
        {
            Items.Add(item);
        }

        public void Delete(Guid toDoItemId)
        {
            Items.RemoveAll(x => x.ToDoItemId == toDoItemId);
        }
    }    
}
