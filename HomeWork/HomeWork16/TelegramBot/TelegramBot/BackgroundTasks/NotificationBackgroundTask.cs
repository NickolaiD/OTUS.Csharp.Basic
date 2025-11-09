using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot.Helpers;
using TelegramBot.Scenarios;
using TelegramBot.Services;

namespace TelegramBot.BackgroundTasks
{
    internal class NotificationBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        public NotificationBackgroundTask(INotificationService notificationService, IUserService userService, ITelegramBotClient bot) : base(TimeSpan.FromMinutes(1), nameof(NotificationBackgroundTask))
        {
            _userService = userService;
            _notificationService = notificationService;
            _bot = bot;
        }

        protected override async Task Execute(CancellationToken ct)
        {
            var notificationList = await _notificationService.GetScheduledNotification(DateTime.UtcNow, ct);
            foreach(var notification in notificationList) 
                {
                    var toDoUser = await _userService.GetUserByIdAsync(notification.User.UserId, ct);
                    if (toDoUser != null)
                    {
                        await _bot.SendMessage(toDoUser.ChatId, notification.Text, cancellationToken: ct, replyMarkup: BotHelper.GetKeyboardButtons(true));
                        await _notificationService.MarkNotified(notification.Id, ct);
                    }
                }
        }
    }
}
