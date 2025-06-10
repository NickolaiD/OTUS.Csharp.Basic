using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}")
        {
            
        }
    }
}
