namespace TelegramBot.Exceptions
{
    class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task) : base($"Задача {task} уже существует")
        {
            
        }
    }
}
