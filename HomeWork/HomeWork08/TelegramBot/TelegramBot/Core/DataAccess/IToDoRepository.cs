﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Entities;

namespace TelegramBot.Core.DataAccess
{
    interface IToDoRepository
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
        //Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
        Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct);
        Task AddAsync(ToDoItem item, CancellationToken ct);
        void Update(ToDoItem item);
        Task DeleteAsync(Guid id, CancellationToken ct);
        //Проверяет есть ли задача с таким именем у пользователя
        Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct);
        //Возвращает количество активных задач у пользователя
        Task<int> CountActiveAsync(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct);
    }
}
