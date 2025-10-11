using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using TelegramBot.Entities;

namespace TelegramBot.Core.DataAccess.Models
{
    [Table("todo_list")]
    internal class ToDoListModel
    {
        [PrimaryKey, Column("id")]
        public Guid Id { get; set; }

        [Column("name"), NotNull]
        public string Name { get; set; }

        [Column("user_id"), NotNull] 
        public ToDoUser User { get; set; }

        [Column("created_at"), NotNull]
        public DateTime CreatedAt { get; set; }

        [Association(ThisKey = nameof(Id), OtherKey = nameof(ToDoItemModel.List))]
        public List<ToDoItemModel> ToDoItems { get; set; } = [];
    }
}
