using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.DataAccess.Models;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class SqlToDoRepository : IToDoListRepository
    {
        private IDataContextFactory<DataConnection> _dataContextFactory;

        public SqlToDoRepository(IDataContextFactory<DataConnection> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public Task Add(ToDoList list, CancellationToken ct)
        {
            //await Task.Run(() => _toDoItemList.Add(item));

            using var dbContext = _dataContextFactory.CreateDataContext();
            dbContext.InsertAsync(list);


            throw new NotImplementedException();
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            return await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.User.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0);
        }

        public Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
