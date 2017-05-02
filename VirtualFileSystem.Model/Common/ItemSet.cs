using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal class ItemSet<T> : IItemSet<T>
    {

        private readonly HashSet<T> items;

        public ItemSet(IEqualityComparer<T> comparer = null) => this.items = new HashSet<T>(comparer);

        protected IEqualityComparer<T> Comparer => this.items.Comparer;

        public IReadOnlyCollection<T> Items => this.items;

        public virtual bool Add(T item) => this.items.Add(item);

        public virtual bool Remove(T item) => this.items.Remove(item);

    }

}
