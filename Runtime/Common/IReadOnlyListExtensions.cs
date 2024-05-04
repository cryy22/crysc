using System.Collections.Generic;
using System.Linq;

namespace Crysc.Common
{
    public static class ReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T element)
        {
            for (var i = 0; i < list.Count; i++)
                if (EqualityComparer<T>.Default.Equals(x: list.ElementAt(i), y: element))
                    return i;

            return -1;
        }
    }
}
