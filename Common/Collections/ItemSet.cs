using System;
using System.Collections.Generic;

namespace Common.Collections
{

    public class ItemSet<T> : IItemSet<T>
    {

        private readonly HashSet<T> items;

        public ItemSet(IEqualityComparer<T> comparer = null) => this.items = new HashSet<T>(comparer);

        protected IEqualityComparer<T> Comparer => this.items.Comparer;

        public IReadOnlyCollection<T> Items => this.items;

        public void Clear() => this.items.Clear();

        public void TrimExcess() => this.items.TrimExcess();

        public virtual bool Add(T item) => this.items.Add(item);

        public virtual bool Remove(T item) => this.items.Remove(item);

        public int RemoveWhere(Predicate<T> match) => this.items.RemoveWhere(match);

    }

}
