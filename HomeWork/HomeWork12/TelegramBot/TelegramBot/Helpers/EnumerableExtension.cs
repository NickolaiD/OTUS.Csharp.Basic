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
            try
            {
                return value.Chunk(batchSize).ToArray()[batchNumber];
            }
            catch (Exception ex) 
            {
                //return Enumerable.Empty<T>();
                throw new ArgumentException("Неверные аргументы для текущей коллекции");
            }
        }
    }
}
