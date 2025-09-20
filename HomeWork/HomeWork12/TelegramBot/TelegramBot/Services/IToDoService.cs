using TelegramBot.Entities;

namespace TelegramBot.Services
{
    internal interface IToDoService
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
        //Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
        Task<ToDoItem> AddAsync(ToDoUser user, string name, DateTime deadline, ToDoList? list, CancellationToken ct);
        Task MarkCompletedAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> FindAsync(ToDoUser user, string namePrefix, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> GetByUserIdAndListAsync(Guid userId, Guid? listId, CancellationToken ct);
        Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct);
    }
}
