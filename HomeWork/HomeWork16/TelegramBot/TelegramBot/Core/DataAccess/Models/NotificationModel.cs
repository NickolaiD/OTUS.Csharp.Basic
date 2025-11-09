using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Core.DataAccess.Models
{
    
        [Table("notification")]
        internal class NotificationModel
    {
            [PrimaryKey, Column("id")]
            public Guid Id { get; set; }

            [Column("user_id"), NotNull]
            public Guid UserId { get; set; }

            [Column("type"), NotNull]
            public string Type { get; set; }
        
            [Column("text"), NotNull]
            public string Text { get; set; }

            [Column("scheduled_at"), NotNull]
            public DateTime ScheduledAt { get; set; }

            [Column("is_notified"), NotNull]
            public bool IsNotified { get; set; }

            [Column("notified_at")]
            public DateTime? NotifiedAt { get; set; }
        }
}
