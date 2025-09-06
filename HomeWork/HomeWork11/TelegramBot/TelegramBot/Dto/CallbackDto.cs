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
