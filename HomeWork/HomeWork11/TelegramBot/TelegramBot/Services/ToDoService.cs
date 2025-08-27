using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Exceptions;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    internal class ToDoService : IToDoService
    {
        private readonly int _taskCountLimit;
        private readonly int _taskLengthLimit;
        private readonly IToDoRepository _toDoRepository;
        public ToDoService(int taskCountLimit, int taskLengthLimit, IToDoRepository toDoRepository)
        {
            _taskCountLimit = taskCountLimit;
            _taskLengthLimit = taskLengthLimit;
            _toDoRepository = toDoRepository;
        }

        public async Task<ToDoItem> AddAsync(ToDoUser user, string toDoItemName, DateTime deadline, CancellationToken ct)
        {
            ValidateString(toDoItemName);

            if (await _toDoRepository.CountActiveAsync(user.UserId, ct) >= _taskCountLimit)
                throw new TaskCountLimitException(_taskCountLimit);

            if (toDoItemName.Length > _taskLengthLimit)
                throw new TaskLengthLimitException(toDoItemName.Length, _taskLengthLimit);

            if (await _toDoRepository.ExistsByNameAsync(user.UserId, toDoItemName, ct))
            { 
                throw new DuplicateTaskException(toDoItemName);
            }

            var newToDoItem = new ToDoItem(user, toDoItemName, deadline);

            await _toDoRepository.AddAsync(newToDoItem, ct);
            return newToDoItem;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            await _toDoRepository.DeleteAsync(id, ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _toDoRepository.GetActiveByUserIdAsync(userId, ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await Task.Run(() => _toDoRepository.GetAllByUserIdAsync(userId, ct));
        }

        public async Task MarkCompletedAsync(Guid id, CancellationToken ct)
        {
            var toDoItem = await _toDoRepository.GetAsync(id, ct);
            if (toDoItem != null)
            {
                 _toDoRepository.Update(toDoItem);
            }
        }

        private void ValidateString(string? str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var item in str)
                {
                    if (!char.IsWhiteSpace(item))
                    {
                        return;
                    }
                }
            }

            throw new ArgumentException("Передаваемый параметр пуст или содержит одни пробелы");
        }

        public async Task<IReadOnlyList<ToDoItem>> FindAsync(ToDoUser user, string namePrefix, CancellationToken ct)
        {
            return await Task.Run(() => _toDoRepository.FindAsync(user.UserId, x => x.Name.StartsWith(namePrefix), ct));
        }
    }

}
