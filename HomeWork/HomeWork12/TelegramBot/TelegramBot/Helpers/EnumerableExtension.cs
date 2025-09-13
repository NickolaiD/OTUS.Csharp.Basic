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
                //return value.Chunk(batchSize).ToArray()[batchNumber];
                
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (batchSize <= 0)
                    throw new ArgumentException("Размер пачки должен быть больше 0", nameof(batchSize));

                if (batchNumber < 0)
                    throw new ArgumentException("Номер пачки не может быть отрицательным", nameof(batchNumber));


                return value
                    .Skip(batchNumber * batchSize)
                    .Take(batchSize);

            }
            catch (Exception ex) 
            {
                //return Enumerable.Empty<T>();
                throw new ArgumentException("Неверные аргументы для текущей коллекции");
            }
        }
    }
}
