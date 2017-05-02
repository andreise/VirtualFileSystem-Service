using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal interface IItemSet<T>
    {

        IReadOnlyCollection<T> Items { get; }

        bool Add(T item);

        bool Remove(T item);

    }

}
