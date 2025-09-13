using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Helpers
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> value, int batchSize, int batchNumber)
        {
            if (batchSize <= 0)
                throw new ArgumentException("Размер пачки должен быть больше 0");

            if (batchNumber <= 0)
                throw new ArgumentException("Номер пачки должен быть больше 0");

            return value.Skip(batchNumber * batchSize).Take(batchSize);
        }
    }
}
