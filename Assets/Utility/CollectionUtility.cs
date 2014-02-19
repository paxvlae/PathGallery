using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Utility
{
    static class CollectionUtility
    {
        public static int IndexOfFirst<T>(this IList<T> collection, Predicate<T> condition)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (condition(collection[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
