namespace TelegramBot.Entities
{
    internal class ToDoList
    {
        public Guid Id { get; }
        public string Name { get; }
        public ToDoUser User { get; }
        public DateTime CreatedAt { get; }
        //public ToDoList(ToDoUser user, string name, DateTime deadline)
        //{
            //Id = Guid.NewGuid();
            //CreatedAt = DateTime.UtcNow;
            //User = user;
            //Name = name;
        //}
    }
}
