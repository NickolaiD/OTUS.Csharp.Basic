﻿namespace TelegramBot.Exceptions
{
    class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}")
        {
            
        }
    }
}
