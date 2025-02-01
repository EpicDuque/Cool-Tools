using System.Collections.Generic;

namespace Cooltools.Utilities
{
    public static class ListExtensions
    {
        public static bool AddUnique<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
            {
                return false;
            }

            list.Add(item);
            return true;
        }
    }
}