using System.Collections;
using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal class ItemSet<T> : IItemSet<T>
    {

        private readonly HashSet<T> items;

        protected IEqualityComparer<T> Comparer => this.items.Comparer;

        public ItemSet(IEqualityComparer<T> comparer = null) => this.items = new HashSet<T>(comparer);

        public int Count => this.items.Count;

        private IEnumerator<T> GetEnumeratorInternal() => ((IReadOnlyCollection<T>)this.items).GetEnumerator();

        public IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public virtual bool Add(T item) => this.items.Add(item);

        public virtual bool Remove(T item) => this.items.Remove(item);

    }

}
