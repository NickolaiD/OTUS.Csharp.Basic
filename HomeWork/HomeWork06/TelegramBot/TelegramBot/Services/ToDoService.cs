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

        public ToDoItem Add(ToDoUser user, string toDoItemName)
        {
            ValidateString(toDoItemName);

            if (_toDoRepository.CountActive(user.UserId) >= _taskCountLimit)
                throw new TaskCountLimitException(_taskCountLimit);

            if (toDoItemName.Length > _taskLengthLimit)
                throw new TaskLengthLimitException(toDoItemName.Length, _taskLengthLimit);

            if (_toDoRepository.ExistsByName(user.UserId, toDoItemName))
            { 
                throw new DuplicateTaskException(toDoItemName);
            }

            var newToDoItem = new ToDoItem(user, toDoItemName);

            _toDoRepository.Add(newToDoItem);
            return newToDoItem;
        }

        public void Delete(Guid id)
        {
            _toDoRepository.Delete(id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId);
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId);
        }

        public void MarkCompleted(Guid id)
        {
            var toDoItem = _toDoRepository.Get(id);
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

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return _toDoRepository.Find(user.UserId, x => x.Name.StartsWith(namePrefix));
        }
    }

}
