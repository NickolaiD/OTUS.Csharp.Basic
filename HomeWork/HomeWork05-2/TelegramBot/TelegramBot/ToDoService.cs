using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBot
{
    internal class ToDoService : IToDoService
    {
        private List<ToDoItem> _toDoItemList;
        private readonly int _taskCountLimit;
        private readonly int _taskLengthLimit;
        private readonly ITelegramBotClient _botClient;
        private readonly Update _update;
        public ToDoService(ITelegramBotClient botClient, Update update, int taskCountLimit, int taskLengthLimit)
        {
            _toDoItemList = new List<ToDoItem>();
            _botClient = botClient;
            _update = update;

            _taskCountLimit = taskCountLimit;
            _taskLengthLimit = taskLengthLimit;
        }

        public ToDoItem Add(ToDoUser user, string toDoItemName)
        {
            ValidateString(toDoItemName);

            if (_toDoItemList.Count >= _taskCountLimit)
                throw new TaskCountLimitException(_taskCountLimit);

            if (toDoItemName.Length > _taskLengthLimit)
                throw new TaskLengthLimitException(toDoItemName.Length, _taskLengthLimit);

            foreach (var toDoItem in _toDoItemList)
            {
                if (toDoItem.Name == toDoItemName)
                    throw new DuplicateTaskException(toDoItemName);
            }

            var newToDoItem = new ToDoItem(user, toDoItemName);
            _toDoItemList.Add(newToDoItem);

            return newToDoItem;

        }

        public void Delete(Guid id)
        {
            foreach (var toDoItem in _toDoItemList)
            {
                if (id == toDoItem.Id)
                {
                    _toDoItemList.Remove(toDoItem);
                    return;
                }
            }
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            var userToDoItemList = new List<ToDoItem>();
            foreach (var toDoItem in _toDoItemList)
                if ((toDoItem.User.UserId == userId) && (toDoItem.State == ToDoItemState.Active))
                    userToDoItemList.Add(toDoItem);

            return userToDoItemList;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var userToDoItemList = new List<ToDoItem>();
            foreach (var toDoItem in _toDoItemList)
                if (toDoItem.User.UserId == userId)
                    userToDoItemList.Add(toDoItem);

            return userToDoItemList;            
        }

        public void MarkCompleted(Guid id)
        {
            foreach (var toDoItem in _toDoItemList)
            {
                if ((toDoItem.State == ToDoItemState.Active) && (toDoItem.Id == id))
                {
                    toDoItem.State = ToDoItemState.Completed;
                    toDoItem.StateChangedAt = DateTime.Now;
                    return;
                }
            }
        }

        private void ValidateString(string? str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                foreach (var item in str)
                {
                    if (!char.IsWhiteSpace(item))
                    {
                        return;
                    }
                }
            }

            throw new ArgumentException();
        }

    }

}
