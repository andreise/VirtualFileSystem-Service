using System;
using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    internal sealed class FSItemChildItems
    {

        private readonly HashSet<IFSItem> items = new HashSet<IFSItem>();

        public IReadOnlyCollection<IFSItem> Items => this.items;

        private readonly IFSItem owner;

        public FSItemChildItems(IFSItem owner)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            this.owner = owner;
        }

        public void Add(IFSItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            item.Parent = this.owner;
            this.items.Add(item);
        }

        public void Remove(IFSItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (this.items.Remove(item))
            {
                if ((object)item.Parent == (object)this.owner)
                    item.Parent = null;
            }
        }

    }

}
