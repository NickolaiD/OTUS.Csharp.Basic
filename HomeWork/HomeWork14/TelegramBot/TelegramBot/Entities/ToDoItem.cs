namespace TelegramBot.Entities
{
    internal class ToDoItem
    {
        public Guid Id { get; set; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTimeOffset? StateChangedAt { get; set; }
        public DateTime Deadline { get; set; }
        public ToDoList? List { get; set; }
    }
}
