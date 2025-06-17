namespace StackExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var s = new Stack("a", "b", "c");

                // size = 3, Top = 'c'
                Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

                var deleted = s.Pop();

                // Извлек верхний элемент 'c' Size = 2
                Console.WriteLine($"Извлек верхний элемент '{deleted}' Size = {s.Size}");
                s.Add("d");

                // size = 3, Top = 'd'
                Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

                s.Pop();
                s.Pop();
                s.Pop();
                // size = 0, Top = null
                Console.WriteLine($"size = {s.Size}, Top = {(s.Top == null ? "null" : s.Top)}");
                //s.Pop();

                //***************

                var s1 = new Stack("a", "b", "c");
                s1.Merge(new Stack("1", "2", "3"));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            



        }
    }
}
