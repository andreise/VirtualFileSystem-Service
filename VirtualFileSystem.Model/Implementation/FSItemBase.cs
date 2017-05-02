using System;
using System.Collections.Generic;
using System.Linq;
using VFSCommon;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item
    /// </summary>
    internal abstract class FSItemBase : IFSItem
    {

        /// <summary>
        /// Item Kind
        /// </summary>
        public FSItemKind Kind { get; }

        /// <summary>
        /// Item Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parent Item, or null if this item have no a parent
        /// </summary>
        public IFSItem Parent { get; private set; }

        /// <summary>
        /// Resets Parent Item
        /// </summary>
        public void ResetParent() => this.Parent = null;

        /// <summary>
        /// Validates parent setting operation
        /// </summary>
        /// <param name="parent">Parent Item</param>
        /// <exception cref="ArgumentNullException">Throws if new parent item is null</exception>
        /// <exception cref="ArgumentException">
        /// Throws if this item and new parent item is the same item, or if new parent item cannot have this item as a child
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if parent settings operation is invalid for this item
        /// </exception>
        protected virtual void ValidateSetParent(IFSItem parent)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if ((object)this == (object)parent)
                throw new ArgumentException("Item cannot be a parent of itself.");
        }

        /// <summary>
        /// Sets Parent Item
        /// </summary>
        /// <param name="parent">Parent Item</param>
        /// <exception cref="ArgumentNullException">Throws if new parent item is null</exception>
        /// <exception cref="ArgumentException">
        /// Throws if this item and new parent item is the same item, or if new parent item cannot have this item as a child
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if parent settings operation is invalid for this item
        /// </exception>
        public void SetParent(IFSItem parent)
        {
            this.ValidateSetParent(parent);
            this.Parent = parent;
        }

        /// <summary>
        /// Gets zero-based item level in the file system hierarchy
        /// </summary>
        /// <returns>Zero-based item level in the file system hierarchy</returns>
        public int GetLevel()
        {
            int level = 0;

            IFSItem parent = this.Parent;
            while (!(parent is null))
            {
                level++;
                parent = parent.Parent;
            }

            return level;
        }

        private readonly FSItemChildItems childItems;

        /// <summary>
        /// Child Items
        /// </summary>
        public IReadOnlyCollection<IFSItem> ChildItems => this.childItems;

        private readonly HashSet<string> lockedBy = new HashSet<string>(UserNameComparerProvider.Default);

        /// <summary>
        /// User list which blocked the item
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if item is not a file</exception>
        public IReadOnlyCollection<string> LockedBy
        {
            get
            {
                if (this.Kind != FSItemKind.File)
                    throw new InvalidOperationException("Item is not a file.");

                return this.lockedBy;
            }
        }

        /// <summary>
        /// Locks Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file or if a file already locked by the specified user</exception>
        public void Lock(string userName)
        {
            if (this.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if (userName is null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{nameof(userName)} is empty."));

            if (this.lockedBy.Contains(userName))
                throw new InvalidOperationException(Invariant($"File already locked by user '{userName}'."));

            this.lockedBy.Add(userName);
        }

        /// <summary>
        /// Unlock Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file or if a file is not locked by the specified user</exception>
        public void Unlock(string userName)
        {
            if (this.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if (userName is null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{nameof(userName)} is empty."));

            if (!this.lockedBy.Contains(userName))
                throw new InvalidOperationException(Invariant($"File is not locked by user '{userName}'."));

            this.lockedBy.Remove(userName);
        }

        /// <summary>
        /// Adds child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory, or if the child item is not a directory or a file,
        /// or if the item already have a child with the same name
        /// </exception>
        public void AddChild(IFSItem child)
        {
            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (this.Kind != FSItemKind.FileSystem && this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a file system, a volume or a directory.");

            if (this.Kind == FSItemKind.FileSystem)
            {
                if (child.Kind != FSItemKind.Volume)
                    throw new InvalidOperationException("Child item is not a volume.");

                if (this.childItems.Any(item => FSItemNameComparerProvider.Default.Equals(item.Name, child.Name)))
                    throw new InvalidOperationException("Volume with the specified name already exists.");
            }
            else
            {
                if (child.Kind != FSItemKind.Directory && child.Kind != FSItemKind.File)
                    throw new InvalidOperationException("Child item is not a directory or a file.");

                if (this.childItems.Any(item => FSItemNameComparerProvider.Default.Equals(item.Name, child.Name)))
                    throw new InvalidOperationException("Directory or file with the specified name already exists.");
            }

            this.childItems.Add(child);
        }

        /// <summary>
        /// Removes child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory,
        /// or if the child item is not a directory or a file,
        /// or if the child item is a locked file,
        /// or if the item do not contain the specified child
        /// </exception>
        public void RemoveChild(IFSItem child)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (child.Kind != FSItemKind.Directory && child.Kind != FSItemKind.File)
                throw new InvalidOperationException("Child item is not a directory or a file.");

            if (child.Kind == FSItemKind.File)
            {
                if (child.LockedBy.Count > 0)
                    throw new InvalidOperationException("Child item is a locked file.");
            }

            if (!this.childItems.Contains(child))
                throw new InvalidOperationException("Item do not have the specified child item.");

            this.childItems.Remove(child);
        }

        /// <summary>
        /// Adds child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child directory or file with same name already exists,
        /// or if the item kind is not volume or directory
        /// </exception>
        /// <returns>Added child directory</returns>
        public IFSItem AddChildDirectory(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            IFSItem directory = new FSDirectory(name);

            if (this.childItems.Any(item => FSItemNameComparerProvider.Default.Equals(directory.Name, item.Name)))
                throw new InvalidOperationException("Directory or file with the specified name already exists.");

            this.childItems.Add(directory);
            return directory;
        }

        /// <summary>
        /// Adds child file
        /// </summary>
        /// <param name="name">File name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child directory or file with same name already exists,
        /// or if the item kind is not volume or directory
        /// </exception>
        /// <returns>Added child file</returns>
        public IFSItem AddChildFile(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            IFSItem file = new FSFile(name);

            if (this.childItems.Any(item => FSItemNameComparerProvider.Default.Equals(file.Name, item.Name)))
                throw new InvalidOperationException("Directory or file with the specified name already exists.");

            this.childItems.Add(file);
            return file;
        }

        /// <summary>
        /// Removes empty child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a directory, or if child directory is not empty,
        /// or if the item kind is not volume or directory, or if a child item with the specified name is not exists
        /// </exception>
        public void RemoveChildDirectory(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            IFSItem directory = new FSDirectory(name); // validate name

            IFSItem child = this.childItems.Where(item => FSItemNameComparerProvider.Default.Equals(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item with the specified name is not a directory.");

            if (child.ChildItems.Count > 0)
                throw new InvalidOperationException("Directory with the specified name is not empty.");

            this.childItems.Remove(child);
        }

        /// <summary>
        /// Removes empty child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a directory
        /// or if the item kind is not volume or directory, or if a child item with the specified name is not exists
        /// </exception>
        public void RemoveChildDirectoryWithTree(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            IFSItem directory = new FSDirectory(name); // validate name

            IFSItem child = this.childItems.Where(item => FSItemNameComparerProvider.Default.Equals(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item with the specified name is not a directory.");

            this.childItems.Remove(child);
        }

        /// <summary>
        /// Removes child file
        /// </summary>
        /// <param name="name">File name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a file, or if the item kind is not volume or directory,
        /// or if a child item with the specified name is not exists,
        /// or if a child item with the specified name is locked
        /// </exception>
        public void RemoveChildFile(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            IFSItem file = new FSFile(name); // validate name

            IFSItem child = this.childItems.Where(item => FSItemNameComparerProvider.Default.Equals(file.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("File with the specified name is not exists.");

            if (child.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item with the specified name is not a file.");

            if (child.LockedBy.Count > 0)
                throw new InvalidOperationException("File with the specified name is locked.");

            this.childItems.Remove(child);
        }

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        protected virtual void ValidateName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Invariant($"{nameof(name)} is empty."), nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="kind">Item Kind</param>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the kind is incorrect</exception>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        public FSItemBase(FSItemKind kind, string name)
        {
            if (!Enum.IsDefined(typeof(FSItemKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, Invariant($"{nameof(kind)} is incorrect."));

            this.ValidateName(name);

            this.Kind = kind;

            this.Name = name.Trim();

            this.childItems = new FSItemChildItems(this);
        }

    }

}
