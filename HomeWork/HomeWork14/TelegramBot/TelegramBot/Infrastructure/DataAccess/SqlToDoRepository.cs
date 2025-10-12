using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Core.DataAccess;
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
            throw new NotImplementedException();
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            //            using var dbContext = factory.CreateDataContext();
            //            Использовать ModelMapper
            //Не забудь добавлять LoadWith, чтобы загружать связанные сущности(eager loading)

            //  .LoadWith(i => i.User)
            //  .LoadWith(i => i.List)
            //  .LoadWith(i => i.List!.User)

            using var dbContext = _dataContextFactory.CreateDataContext();

            var t = new ToDoDataContext("dsgf");
            
            dbContext.
            t.ToDoItems

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
