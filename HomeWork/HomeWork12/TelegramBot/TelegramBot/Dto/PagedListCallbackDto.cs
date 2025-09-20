using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Dto
{
    internal class PagedListCallbackDto : ToDoListCallbackDto
    {
        public int Page { get; private set; }
        public static new PagedListCallbackDto FromString(string input)
        {
            var stringArray = input.Split('|');
            Guid? toDoItemId;

            if (Guid.TryParse(stringArray[1], out var guid))
            {
                toDoItemId = guid;
            }
            else
            {
                toDoItemId = null;
            }

            if (int.TryParse(stringArray[2], out var page))
            {
                return new PagedListCallbackDto { Action = stringArray[0], Page = page, ToDoListId = toDoItemId };
            }
            else
            {
                return new PagedListCallbackDto { Action = stringArray[0], Page = -1, ToDoListId = toDoItemId };
            }
        }
        public override string ToString() 
        {
            return $"{base.ToString()}|{Page}";
        }
    }
}
