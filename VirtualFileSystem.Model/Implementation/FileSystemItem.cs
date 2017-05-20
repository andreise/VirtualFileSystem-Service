using Common.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.FormattableString;

namespace VirtualFileSystem.Model.Implementation
{

    /// <summary>
    /// File System Item
    /// </summary>
    internal sealed class FileSystemItem : IFileSystemItem
    {

        /// <summary>
        /// Item Kind
        /// </summary>
        public FileSystemItemKind Kind { get; }

        /// <summary>
        /// Item Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Valid Parent Kinds
        /// </summary>
        private readonly IReadOnlyCollection<FileSystemItemKind> validParentKinds;

        /// <summary>
        /// Valid Child Kinds
        /// </summary>
        private readonly IReadOnlyCollection<FileSystemItemKind> validChildKinds;

        /// <summary>
        /// Valid Parent Kinds Message
        /// </summary>
        private readonly string validParentKindsMessage;

        /// <summary>
        /// Valid Child Kinds Message
        /// </summary>
        private readonly string validChildKindsMessage;

        /// <summary>
        /// Parent Item, or null if this item have no a parent
        /// </summary>
        public IFileSystemItem Parent { get; private set; }

        /// <summary>
        /// Resets Parent Item
        /// </summary>
        public void ResetParent() => this.Parent = null;

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
        public void SetParent(IFileSystemItem parent)
        {
            if (this.validParentKinds.Count == 0)
                throw new InvalidOperationException(this.validParentKindsMessage);

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if ((object)this == (object)parent)
                throw new ArgumentException("Item cannot be a parent of itself.");

            if (!this.validParentKinds.Contains(parent.Kind))
                throw new ArgumentException(this.validParentKindsMessage);

            this.Parent = parent;
        }

        /// <summary>
        /// Gets zero-based item level in the file system hierarchy
        /// </summary>
        /// <returns>Zero-based item level in the file system hierarchy</returns>
        public int GetLevel()
        {
            int level = 0;

            IFileSystemItem current = this;
            while (
                !((current = current.Parent) is null)
            )
            {
                level++;
            }

            return level;
        }

        private readonly IItemSet<IFileSystemItem> childItems;

        /// <summary>
        /// Child Items
        /// </summary>
        public IReadOnlyCollection<IFileSystemItem> ChildItems => this.childItems.Items;

        private readonly IItemSet<string> lockedBy = new UserNameSet();

        /// <summary>
        /// User list which blocked the item
        /// </summary>
        public IReadOnlyCollection<string> LockedBy => this.lockedBy.Items;

        /// <summary>
        /// Locks Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file or if a file already locked by the specified user</exception>
        public void Lock(string userName)
        {
            if (this.Kind != FileSystemItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if (userName is null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: "User name is empty.");

            if (this.LockedBy.Contains(userName))
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
            if (this.Kind != FileSystemItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if (userName is null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: "User name is empty.");

            if (!this.LockedBy.Contains(userName))
                throw new InvalidOperationException(Invariant($"File is not locked by user '{userName}'."));

            this.lockedBy.Remove(userName);
        }

        /// <summary>
        /// Determines the item or its child tree's items are locked
        /// </summary>
        /// <returns>
        /// Returns true if the item or its child tree's items are locked, otherwise returns false
        /// </returns>
        public bool HasLocks() => this.LockedBy.Count > 0 || this.ChildItems.Any(child => child.HasLocks());

        private void ValidateCanHasChildItems()
        {
            if (this.validChildKinds.Count == 0)
                throw new InvalidOperationException(this.validChildKindsMessage);
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
        public void AddChild(IFileSystemItem child)
        {
            this.ValidateCanHasChildItems();

            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (!this.validChildKinds.Contains(child.Kind))
                throw new ArgumentException(this.validChildKindsMessage);

            bool IsChildAlreadyExists() => this.ChildItems.Any(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, child.Name));

            if (IsChildAlreadyExists())
                throw new InvalidOperationException("Child item with the specified name already exists.");

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
        public void RemoveChild(IFileSystemItem child)
        {
            this.ValidateCanHasChildItems();

            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (child.Kind != FileSystemItemKind.Directory && child.Kind != FileSystemItemKind.File)
                throw new InvalidOperationException("Child item is not a directory or a file.");

            if (!this.ChildItems.Contains(child))
                throw new InvalidOperationException("Item do not have the specified child item.");

            if (child.LockedBy.Count > 0)
                throw new InvalidOperationException("Child item is locked.");

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
        public IFileSystemItem AddChildDirectory(string name)
        {
            this.ValidateCanHasChildItems();

            IFileSystemItem directory = FileSystemItemFactory.CreateDirectory(name);

            if (this.ChildItems.Any(item => FileSystemItemNameComparerProvider.Default.Equals(directory.Name, item.Name)))
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
        public IFileSystemItem AddChildFile(string name)
        {
            this.ValidateCanHasChildItems();

            IFileSystemItem file = FileSystemItemFactory.CreateFile(name);

            if (this.ChildItems.Any(item => FileSystemItemNameComparerProvider.Default.Equals(file.Name, item.Name)))
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
            this.ValidateCanHasChildItems();

            IFileSystemItem directory = FileSystemItemFactory.CreateDirectory(name); // validate name

            IFileSystemItem child = this.ChildItems.Where(item => FileSystemItemNameComparerProvider.Default.Equals(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FileSystemItemKind.Directory)
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
            this.ValidateCanHasChildItems();

            IFileSystemItem directory = FileSystemItemFactory.CreateDirectory(name); // validate name

            IFileSystemItem child = this.ChildItems.Where(item => FileSystemItemNameComparerProvider.Default.Equals(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FileSystemItemKind.Directory)
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
            this.ValidateCanHasChildItems();

            IFileSystemItem file = FileSystemItemFactory.CreateFile(name); // validate name

            IFileSystemItem child = this.ChildItems.Where(item => FileSystemItemNameComparerProvider.Default.Equals(file.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("File with the specified name is not exists.");

            if (child.Kind != FileSystemItemKind.File)
                throw new InvalidOperationException("Item with the specified name is not a file.");

            if (child.LockedBy.Count > 0)
                throw new InvalidOperationException("File with the specified name is locked.");

            this.childItems.Remove(child);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="kind">Item Kind</param>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the kind is incorrect</exception>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        internal FileSystemItem(
            FileSystemItemKind kind,
            string name,
            IReadOnlyCollection<FileSystemItemKind> validParentKinds,
            IReadOnlyCollection<FileSystemItemKind> validChildKinds,
            string validParentKindsMessage,
            string validChildKindsMessage
        )
        {
            this.Kind = kind;
            this.Name = name;

            this.validParentKinds = validParentKinds;
            this.validChildKinds = validChildKinds;

            this.validParentKindsMessage = validParentKindsMessage;
            this.validChildKindsMessage = validChildKindsMessage;

            this.childItems = new FileSystemItemChildItemSet(this);
        }

    }

}
