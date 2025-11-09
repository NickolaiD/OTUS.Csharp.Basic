using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Scenarios;
using TelegramBot.Services;

namespace TelegramBot.BackgroundTasks
{
    internal class DeadlineBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IToDoRepository _toDoRepository;

        public DeadlineBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(TimeSpan.FromHours(1), nameof(DeadlineBackgroundTask))
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _toDoRepository = toDoRepository;
        }

        protected override async Task Execute(CancellationToken ct)
        {
            var userList = await _userRepository.GetUsers(ct);
            foreach (var user in userList)
            {
                var deadlineTasks = await _toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.Date, ct);
                foreach (var task in deadlineTasks)
                {
                    await _notificationService.ScheduleNotification(user.UserId, $"Deadline_{task.Id}", $"Ой! Вы пропустили дедлайн по задаче {task.Name}", DateTime.UtcNow, ct);
                }
            }
        }
    }
}
