
namespace StackExample
{
    public static class StackExtensions
    {
        public static void Merge (this Stack stack1, Stack stack2)
        {
            for (int i = 0; i < stack2.Size; i++) 
            {
                stack1.Add(stack2.Pop());
            }
        }
    }
}
