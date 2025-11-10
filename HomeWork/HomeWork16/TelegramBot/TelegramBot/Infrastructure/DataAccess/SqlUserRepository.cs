


using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Core.DataAccess;
using TelegramBot.Core.DataAccess.Models;
using TelegramBot.Entities;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class SqlUserRepository : IUserRepository
    {
        private IDataContextFactory<DataConnection> _dataContextFactory;
        public SqlUserRepository(IDataContextFactory<DataConnection> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }
        public async Task AddAsync(ToDoUser user, CancellationToken ct)
        {
            if (await GetUserByTelegramUserIdAsync(user.TelegramUserId, ct) == null) { 
                using var dbContext = _dataContextFactory.CreateDataContext();
                await dbContext.InsertAsync<ToDoUserModel>(ModelMapper.MapToModel(user), token:ct);
            }
        }

        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoUserModel = await dbContext.GetTable<ToDoUserModel>().Where(x => x.UserId == userId).FirstOrDefaultAsync(ct);
            return toDoUserModel != null ? ModelMapper.MapFromModel(toDoUserModel) : null;
        }

        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoUserModel = await dbContext.GetTable<ToDoUserModel>().Where(x => x.TelegramUserId == telegramUserId).FirstOrDefaultAsync();
            return toDoUserModel != null ? ModelMapper.MapFromModel(toDoUserModel) : null;
        }

        public async Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDoUserModelList = await dbContext.GetTable<ToDoUserModel>().ToListAsync();

            var resultList = new List<ToDoUser>();
            foreach (var userModel in toDoUserModelList)
            {
                resultList.Add(ModelMapper.MapFromModel(userModel));
            }

            return resultList;




        }
    }
}
