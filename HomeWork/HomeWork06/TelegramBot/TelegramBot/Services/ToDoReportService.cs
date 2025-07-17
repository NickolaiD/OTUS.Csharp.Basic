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
        InMemoryToDoRepository _toDoRepository;
        public ToDoReportService(InMemoryToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            var toDoItemList = _toDoRepository.GetAllByUserId(userId);
            var total = toDoItemList.Count;
            var completed = toDoItemList.Where(x => x.State == ToDoItemState.Completed).Count();
            var active = toDoItemList.Where(x => x.State == ToDoItemState.Active).Count();

            return (total, completed, active, DateTime.Now);
            
        }
    }
}
