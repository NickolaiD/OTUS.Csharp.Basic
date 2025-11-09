using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess.Models;
using TelegramBot.Entities;
using TelegramBot.Infrastructure.DataAccess;

namespace TelegramBot.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDataContextFactory<DataConnection> _dataContextFactory;
        public NotificationService(IDataContextFactory<DataConnection> factory) 
        {
            _dataContextFactory = factory;
        }
        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();

            var resultList = new List<Notification>();
            var notificationList = await Task.Run(() => dbContext.GetTable<NotificationModel>().Where(x => x.IsNotified == false && x.ScheduledAt <= scheduledBefore).ToList());

            foreach (var notification in notificationList)
            {
                resultList.Add(ModelMapper.MapFromModel(notification));
            }

            return resultList;
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await dbContext.GetTable<NotificationModel>().Where(x => x.Id == notificationId).Set(x => x.IsNotified, true).Set(x => x.NotifiedAt, DateTime.Now).UpdateAsync(ct);
        }

        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            int notificationCount = await dbContext.GetTable<NotificationModel>().Where(x => x.UserId == userId && x.Type == type).CountAsync(ct);
            if (notificationCount > 0)
                return false;

            var notificationModel = new NotificationModel()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Text = text,
                ScheduledAt = scheduledAt,
                IsNotified = false
            };

            await dbContext.InsertAsync<NotificationModel>(notificationModel, token: ct);
            return true;
        }
    }
}
