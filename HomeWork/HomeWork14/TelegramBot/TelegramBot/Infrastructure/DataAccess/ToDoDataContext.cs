using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess.Models;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class ToDoDataContext : LinqToDB.Data.DataConnection
    {
        public ITable<ToDoItemModel> ToDoItems => this.GetTable<ToDoItemModel>();
        public ITable<ToDoListModel> ToDoLists => this.GetTable<ToDoListModel>();
        public ITable<ToDoUserModel> ToDoUsers => this.GetTable<ToDoUserModel>();
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString)
        { }

    }
}
