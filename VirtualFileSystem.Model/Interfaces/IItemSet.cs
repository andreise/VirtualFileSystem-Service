using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal interface IItemSet<T> : IReadOnlyCollection<T>
    {

        bool Add(T item);

        bool Remove(T item);

    }

}
