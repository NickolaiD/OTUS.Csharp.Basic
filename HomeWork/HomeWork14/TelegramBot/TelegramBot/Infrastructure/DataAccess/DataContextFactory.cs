using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TelegramBot.Helpers.BotHelper;

namespace TelegramBot.Infrastructure.DataAccess
{
    internal class DataContextFactory : IDataContextFactory<DataConnection>
    {
        public DataConnection CreateDataContext()
        {
            return new ToDoDataContext(CONNECTION_STRING);
        }

        //static void test()
        //{
        //    var t = new DataContextFactory();
        //    t.CreateDataContext();
        //}
    }
}
