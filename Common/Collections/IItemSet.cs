using System;
using System.Collections.Generic;

namespace Common.Collections
{

    public interface IItemSet<T>
    {

        IReadOnlyCollection<T> Items { get; }

        void Clear();

        void TrimExcess();

        bool Add(T item);

        bool Remove(T item);

        int RemoveWhere(Predicate<T> match);

    }

}
