using System.Collections.Generic;
using System.Linq;

public static class QueueExtensions
{
    public static Queue<T> RemoveItem<T>(this Queue<T> queue, T item)
    {
        var newQueue = new Queue<T>(queue.Where(x => !EqualityComparer<T>.Default.Equals(x, item)));
        return newQueue;
    }
}