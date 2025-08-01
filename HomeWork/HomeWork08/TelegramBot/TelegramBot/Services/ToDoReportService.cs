using Otus.ToDoList.ConsoleBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly InMemoryToDoRepository _toDoRepository;
        public ToDoReportService(InMemoryToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStatsAsync(Guid userId, CancellationToken ct)
        {
            var toDoItemList = await _toDoRepository.GetAllByUserIdAsync(userId, ct);
            var total = await Task.Run(() => toDoItemList.Count);
            var completed = await Task.Run(() => toDoItemList.Where(x => x.State == ToDoItemState.Completed).Count());
            var active = await Task.Run(() => toDoItemList.Where(x => x.State == ToDoItemState.Active).Count());

            return (total, completed, active, DateTime.Now);
            
        }
    }
}
