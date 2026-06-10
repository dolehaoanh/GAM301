using System.Collections.Generic;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class StackExtensions
    {
        public static void Remove<T>(this Stack<T> stack, T item)
        {
            if (stack.Count == 0 || !stack.Contains(item))
            {
                return;
            }

            Stack<T> temp = new();

            while (stack.Count > 0)
            {
                T top = stack.Pop();
                if (EqualityComparer<T>.Default.Equals(top, item))
                {
                    break;
                }

                temp.Push(top);
            }

            while (temp.Count > 0)
            {
                stack.Push(temp.Pop());
            }
        }
    }
}
