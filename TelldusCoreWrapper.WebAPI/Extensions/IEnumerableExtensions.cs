using System.Collections.Generic;

namespace TelldusCoreWrapper.WebAPI.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static Queue<TItem> ToQueue<TItem>(this IEnumerable<TItem> collection)
        {
            return new Queue<TItem>(collection);
        }
    }
}
