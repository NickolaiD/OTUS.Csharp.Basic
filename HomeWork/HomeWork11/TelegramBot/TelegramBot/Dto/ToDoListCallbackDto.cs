using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Dto
{
    internal class ToDoListCallbackDto : CallbackDto
    {
        public Guid ToDoListId { get; private set; }
        public static new ToDoListCallbackDto FromString(string input)
        {
            var stringArray = input.Split('|');
            if (Guid.TryParse(stringArray[1], out var guid))
            {
                return new ToDoListCallbackDto { Action = stringArray[0], ToDoListId = guid };
            }
            else
            {
                return new ToDoListCallbackDto { Action = stringArray[0], ToDoListId = Guid.Empty };
            }
        }
        public override string ToString() 
        {
            return $"{base.ToString()}|{ToDoListId}";
        }
    }
}
