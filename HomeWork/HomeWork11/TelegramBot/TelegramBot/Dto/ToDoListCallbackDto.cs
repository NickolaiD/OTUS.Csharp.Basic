using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Dto
{
    internal class ToDoListCallbackDto : CallbackDto
    {
        public Guid? ToDoListId { get; private set; }
        public static new ToDoListCallbackDto FromString(string input)
        {

            //На вход принимает строку ввида "{action}|{toDoListId}|{prop2}...".
            //Нужно создать ToDoListCallbackDto с Action = action и ToDoListId = toDoListId.

            var stringArray = input.Split('|');

            Guid.TryParse(stringArray[1], out var guid);
            return new ToDoListCallbackDto { Action = stringArray[0], ToDoListId = guid };
        }
        public override string ToString() 
        {
            return $"{base.ToString()}|{ToDoListId}";
        }
    }
}
