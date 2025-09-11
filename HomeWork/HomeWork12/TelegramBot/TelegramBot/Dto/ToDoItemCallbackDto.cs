using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Dto
{
    public class ToDoItemCallbackDto : CallbackDto
    {
        public Guid ToDoItemId { get; private set; }
        public static new ToDoItemCallbackDto FromString(string input)
        {
            var stringArray = input.Split('|');
            if (Guid.TryParse(stringArray[1], out var guid))
            {
                return new ToDoItemCallbackDto { Action = stringArray[0], ToDoItemId = guid };
            }
            else
            {
                return new ToDoItemCallbackDto { Action = stringArray[0], ToDoItemId = null };
            }
        }
        public override string ToString() 
        {
            return $"{base.ToString()}|{ToDoItemId}";
        }
    }
}
