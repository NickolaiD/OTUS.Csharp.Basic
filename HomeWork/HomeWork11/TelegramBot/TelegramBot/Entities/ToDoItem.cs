namespace TelegramBot.Entities
{
    internal class ToDoItem
    {
        public Guid Id { get; init; }
        public ToDoUser User { get; init; }
        public string Name { get; init; }
        public DateTime CreatedAt { get; init; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }
        public DateTime Deadline { get; set; }
        public ToDoList? List { get; }

        public ToDoItem(ToDoUser user, string name, DateTime deadline, ToDoList? list)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            User = user;
            Name = name;
            Deadline = deadline;
            List = list;
        }
        public ToDoItem() { }
    }
}
