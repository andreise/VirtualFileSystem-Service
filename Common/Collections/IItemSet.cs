using System.Collections.Generic;

namespace Common.Collections
{

    public interface IItemSet<T>
    {

        IReadOnlyCollection<T> Items { get; }

        bool Add(T item);

        bool Remove(T item);

    }

}
