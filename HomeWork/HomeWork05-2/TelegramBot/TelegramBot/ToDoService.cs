using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class ToDoService : IToDoService
    {
        private List<ToDoItem> _toDoItemList;
        private int _taskCountLimit;
        private int _taskLengthLimit;
        private ITelegramBotClient _botClient;
        private Update _update;
        public ToDoService(ITelegramBotClient botClient, Update update)
        {
            _toDoItemList = new List<ToDoItem>();
            _botClient = botClient;
            _update = update;

            _taskCountLimit = 5;
            _taskLengthLimit = 5;
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

            _botClient.SendMessage(_update.Message.Chat, "Задача добавлена");

            return newToDoItem;

        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {

            if (_toDoItemList.Count > 0)
            {
                int counter = 1;
                foreach (var toDoItem in _toDoItemList)
                {
                    _botClient.SendMessage(_update.Message.Chat, $"{counter} - {toDoItem.Name} - {toDoItem.State} - {toDoItem.CreatedAt} - {toDoItem.Id}");
                    counter++;
                }
            }
            else
                _botClient.SendMessage(_update.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}");
        }

        public void MarkCompleted(Guid id)
        {
            throw new NotImplementedException();
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
