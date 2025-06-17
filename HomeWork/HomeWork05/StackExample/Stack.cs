using System.Drawing;

namespace StackExample
{
    class Stack
    {
        private List<string> _stackList;
        public int Size { get => _stackList.Count; }
        public string? Top 
        { 
            get
            {
                string? result = null;
                if (_stackList.Count > 0)
                {
                    result = _stackList[_stackList.Count - 1];
                }
                return result;
            } 
        }

        public Stack(params string[] stackValues)
        {
            _stackList = new List<string>();
            foreach (string stackValue in stackValues)
            {
                Add(stackValue);
            }

        }

        public void Add(string stackValue)
        {
            _stackList.Add(stackValue);
        }

        public string Pop()
        {
            if (_stackList.Count == 0)
                throw new Exception("Стек пустой");

            var result = _stackList[_stackList.Count - 1];
            _stackList.RemoveAt(_stackList.Count - 1);
            return result;
        }
    }
}
