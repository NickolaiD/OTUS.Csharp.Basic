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
            if (int.TryParse(stringArray[2], out var page))
            {
                return new PagedListCallbackDto { Action = stringArray[0], Page = page };
            }
            else
            {
                return new PagedListCallbackDto { Action = stringArray[0], Page = -1 };
            }
        }
        public override string ToString() 
        {
            return $"{base.ToString()}|{Page}";
        }
    }
}
