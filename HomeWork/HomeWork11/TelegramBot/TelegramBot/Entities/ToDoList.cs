namespace TelegramBot.Entities
{
    public class ToDoList
    {
        public Guid Id { get; init; }
        public string Name { get; }
        public ToDoUser User { get; }
        public DateTime CreatedAt { get; init; }
        public ToDoList(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            User = user;
            Name = name;
        }
    }
}
