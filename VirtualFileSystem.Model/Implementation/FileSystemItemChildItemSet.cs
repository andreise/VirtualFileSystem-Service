using Common.Collections;
using System;

namespace VirtualFileSystem.Model.Implementation
{

    internal sealed class FileSystemItemChildItemSet : ItemSet<IFileSystemItem>
    {

        private readonly IFileSystemItem owner;

        public FileSystemItemChildItemSet(IFileSystemItem owner)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            this.owner = owner;
        }

        private static void ValidateItem(IFileSystemItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));
        }

        public override bool Add(IFileSystemItem item)
        {
            ValidateItem(item);

            item.SetParent(this.owner);
            return base.Add(item);
        }

        public override bool Remove(IFileSystemItem item)
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
