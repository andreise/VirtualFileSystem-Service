using System;
using System.Collections;
using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal sealed class FSItemChildItems : IReadOnlyCollection<IFSItem>
    {

        private readonly HashSet<IFSItem> items = new HashSet<IFSItem>();

        private readonly IFSItem owner;

        public FSItemChildItems(IFSItem owner)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            this.owner = owner;
        }

        public int Count => this.items.Count;

        private IEnumerator<IFSItem> GetEnumeratorInternal() => ((IReadOnlyCollection<IFSItem>)this.items).GetEnumerator();

        public IEnumerator<IFSItem> GetEnumerator() => GetEnumeratorInternal();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public bool Add(IFSItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            item.SetParent(this.owner);
            return this.items.Add(item);
        }

        public bool Remove(IFSItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            bool removed = this.items.Remove(item);
            if (removed)
            {
                if ((object)item.Parent == (object)this.owner)
                    item.ResetParent();
            }
            return removed;
        }

    }

}
