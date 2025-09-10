using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Entities.Index
{
    internal class IndexItem
    {
        public Guid ToDoItemId { get; init; }
        public Guid UserId { get; init; }
        public IndexItem() { }

        public IndexItem(Guid toDoItemId, Guid userId) 
        {
            ToDoItemId = toDoItemId;
            UserId = userId;
        }

    }
}

