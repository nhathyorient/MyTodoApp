using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos.Common.Extensions
{
    public static class ListExtension
    {
        public static List<T> ReplaceWhere<T>(this List<T> items, Func<T, bool> matchPredicate, T replaceByItem)
        {
            var replacedItems = new List<T>();

            for (int i = 0; i < items.Count; i++)
            {
                if (matchPredicate(items[i]))
                {
                    replacedItems[i] = replaceByItem;

                    replacedItems.Add(items[i]);
                }
            }

            return replacedItems;
        }

        public static bool IsEmpty<T>(this List<T> items)
        {
            return !items.Any();
        }
    }
}
