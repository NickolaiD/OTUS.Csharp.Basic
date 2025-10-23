using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TelegramBot.Core.DataAccess.Models
{
    [Table("todo_user")]
    internal class ToDoUserModel
    {
            [PrimaryKey, Column("id")]
            public Guid UserId { get; set; }

            [Column("tg_user_name"), NotNull]
            public string TelegramUserName { get; set; }

            [Column("registered_at"), NotNull]
            public DateTime RegisteredAt { get; set; }
            
            [Column("tg_userid"), NotNull]
            public long TelegramUserId { get; set; }

            [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoListModel.UserId))]
            public List<ToDoListModel> ToDoLists { get; set; } = [];

            [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoItemModel.UserId))]
            public List<ToDoItemModel> ToDoItems { get; set; } = [];

    }
}
