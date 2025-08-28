using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;
using TelegramBot.Exceptions;

namespace TelegramBot.Services
{
    public class ToDoListService : IToDoListService
    {
        private IToDoListRepository _toDoListRepository;
        public ToDoListService(IToDoListRepository toDoListRepository) 
        { 
            _toDoListRepository = toDoListRepository;
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            BotHelper.ValidateString(name);

            if (name.Length > 10)
                throw new ArgumentException($"Длина списка больше 10 символов");

            if (await _toDoListRepository.ExistsByName(user.UserId, name, ct))
            {
                throw new ArgumentException($"Список с таким названием уже существует");
            }

            var toDoList = new ToDoList(user, name);
            await _toDoListRepository.Add(toDoList, ct);
            return toDoList;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _toDoListRepository.Delete(id, ct);
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            return await _toDoListRepository.Get(id, ct);
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            return await _toDoListRepository.GetByUserId(userId, ct);
        }
    }
}
