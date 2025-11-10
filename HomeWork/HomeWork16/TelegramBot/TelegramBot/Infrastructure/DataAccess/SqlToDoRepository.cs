using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.DataAccess.Models;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class SqlToDoRepository : IToDoRepository
    {
        private IDataContextFactory<DataConnection> _dataContextFactory;

        public SqlToDoRepository(IDataContextFactory<DataConnection> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await dbContext.InsertAsync<ToDoItemModel>(ModelMapper.MapToModel(item), token: ct);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            return await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.UserId == userId && x.State == ToDoItemState.Active).Count());
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.Id == id).Delete());
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            return await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0);
        }
       
        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoItemModel = await dbContext.GetTable<ToDoItemModel>().Where(x => x.Id == id).FirstOrDefaultAsync();
            return toDoItemModel != null ? ModelMapper.MapFromModel(toDoItemModel) : null;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoItemModel = await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.UserId == userId && x.State == ToDoItemState.Active).ToList());

            var resultList = new List<ToDoItem>();

            foreach (var item in toDoItemModel)
            {
                resultList.Add(ModelMapper.MapFromModel(item));
            }

            return resultList;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoItemModel = await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.UserId == userId).ToList());

            var resultList = new List<ToDoItem>();

            foreach (var item in toDoItemModel)
            {
                resultList.Add(ModelMapper.MapFromModel(item));
            }

            return resultList;
        }

        public void Update(ToDoItem item)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            dbContext.GetTable<ToDoItemModel>().Where(x => x.Id == item.Id).Set(x => x.State, ToDoItemState.Completed).Set(x => x.StateChangedAt, DateTime.Now).Update();
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            //Использовалось в ранних версиях, сейчас метод не нужен
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoItemModel = await Task.Run(() => dbContext.GetTable<ToDoItemModel>().Where(x => x.UserId == userId && x.ListId == listId).ToList());

            var resultList = new List<ToDoItem>();

            foreach (var item in toDoItemModel)
            {
                resultList.Add(ModelMapper.MapFromModel(item));
            }

            return resultList;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveWithDeadline(Guid userId, DateTime from, DateTime to, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoItemModel = await dbContext.GetTable<ToDoItemModel>().Where(x => x.State == ToDoItemState.Active && x.Deadline >= from && x.Deadline < to).ToListAsync();

            var resultList = new List<ToDoItem>();
            foreach (var item in toDoItemModel)
            {
                resultList.Add(ModelMapper.MapFromModel(item));
            }

            return resultList;
        }
    }
}
