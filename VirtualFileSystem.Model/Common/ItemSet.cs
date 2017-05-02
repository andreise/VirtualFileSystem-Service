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

        public IEnumerator<T> GetEnumerator() => ((IReadOnlyCollection<T>)this.items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public virtual bool Add(T item) => this.items.Add(item);

        public virtual bool Remove(T item) => this.items.Remove(item);

    }

}
