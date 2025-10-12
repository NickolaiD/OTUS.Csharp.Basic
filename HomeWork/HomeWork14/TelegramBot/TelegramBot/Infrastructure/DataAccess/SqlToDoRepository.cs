using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Core.DataAccess;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class SqlToDoRepository : IToDoListRepository
    {
        private IDataContextFactory<DataConnection> _dataContext;

        public SqlToDoRepository(IDataContextFactory<DataConnection> dataContext)
        {
            _dataContext = dataContext;
            _dataContext.
        }

        public Task Add(ToDoList list, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            throw new NotImplementedException();
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
