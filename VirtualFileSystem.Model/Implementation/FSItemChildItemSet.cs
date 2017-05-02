using System;

namespace VirtualFileSystem.Model
{

    internal sealed class FSItemChildItemSet : ItemSet<IFSItem>
    {

        private readonly IFSItem owner;

        public FSItemChildItemSet(IFSItem owner)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            this.owner = owner;
        }

        private static void ValidateItem(IFSItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
        }

        public override bool Add(IFSItem item)
        {
            ValidateItem(item);

            item.SetParent(this.owner);
            return base.Add(item);
        }

        public override bool Remove(IFSItem item)
        {
            ValidateItem(item);

            bool removed = base.Remove(item);
            if (removed)
            {
                if (this.Comparer.Equals(item.Parent, this.owner))
                    item.ResetParent();
            }
            return removed;
        }

    }

}
