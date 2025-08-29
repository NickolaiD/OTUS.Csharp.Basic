using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Dto
{
    public class CallbackDto
    {
        public string Action { get; protected set; }
        
        //На вход принимает строку ввида "{action}|{prop1}|{prop2}...".
        //Нужно создать CallbackDto с Action = action.
        //Нужно учесть что в строке может не быть |, тогда всю строку сохраняем в Action. -
        public static CallbackDto FromString(string input)
        {
            var stringArray = input.Split('|');
            if (stringArray.Length > 0)
            {
                return new CallbackDto { Action = stringArray[0] };
            }
            else 
            {
                return new CallbackDto { Action = input };
            }
        }

        public override string ToString() {  return Action; }
    }
}
