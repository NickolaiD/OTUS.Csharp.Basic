using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Entities;

namespace TelegramBot.Core.DataAccess.Models
{
    [Table("todo_item")]
    internal class ToDoItemModel
    {
        [PrimaryKey, Column("id")]
        public Guid Id { get; set; }

        [Column("user_id"), NotNull]
        public Guid UserId { get; set; }

        [Column("name"), NotNull]
        public string Name { get; set; }

        [Column("created_at"), NotNull]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("state"), NotNull]
        public ToDoItemState State { get; set; }

        [Column("state_change_at")]
        public DateTimeOffset? StateChangedAt { get; set; }

        [Column("deadline"), NotNull]
        public DateTime Deadline { get; set; }

        [Column("list_id")]
        public Guid ListId { get; set; }
    }
}
