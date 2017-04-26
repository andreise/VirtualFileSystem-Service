using System;
using System.Collections.Generic;
using System.Linq;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item
    /// </summary>
    internal abstract class FSItem : IFSItem
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
        /// Parent Item
        /// </summary>
        public virtual IFSItem Parent { get; set; }

        protected internal readonly HashSet<IFSItem> childItemsInternal = new HashSet<IFSItem>(FSItemEqualityComparer.Default);

        /// <summary>
        /// Child Items
        /// </summary>
        public IReadOnlyCollection<IFSItem> ChildItems => childItemsInternal;

        private readonly HashSet<string> lockedBy = new HashSet<string>();

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
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{userName} is empty."));

            if (this.lockedBy.Contains(userName))
                throw new InvalidOperationException("File already locked by the specified user.");

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
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{userName} is empty."));

            if (!this.lockedBy.Contains(userName))
                throw new InvalidOperationException("File is not locked by the specified user.");

            this.lockedBy.Remove(userName);
        }

        /// <summary>
        /// Adds child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child directory or file with same name already exists,
        /// or if the item kind is not volume or directory
        /// </exception>
        public IFSItem AddChildDirectory(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item kind is not volume or directory.");

            IFSItem directory = new FSDirectory(name);

            if (this.childItemsInternal.Any(item => FSItemNameCompareRules.EqualNames(directory.Name, item.Name)))
                throw new InvalidOperationException("Directory or file with the specified name already exists.");

            directory.Parent = this;
            this.childItemsInternal.Add(directory);
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
        public IFSItem AddChildFile(string name)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item kind is not volume or directory.");

            IFSItem file = new FSFile(name);

            if (this.childItemsInternal.Any(item => FSItemNameCompareRules.EqualNames(file.Name, item.Name)))
                throw new InvalidOperationException("Directory or file with the specified name already exists.");

            file.Parent = this;
            this.childItemsInternal.Add(file);
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
                throw new InvalidOperationException("Item kind is not volume or directory.");

            IFSItem directory = new FSDirectory(name); // validate name

            IFSItem child = this.childItemsInternal.Where(item => FSItemNameCompareRules.EqualNames(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item with the specified name is not a directory.");

            if (child.ChildItems.Count > 0)
                throw new InvalidOperationException("Directory with the specified name is not empty.");

            this.childItemsInternal.Remove(child);
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
                throw new InvalidOperationException("Item kind is not volume or directory.");

            IFSItem directory = new FSDirectory(name); // validate name

            IFSItem child = this.childItemsInternal.Where(item => FSItemNameCompareRules.EqualNames(directory.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("Directory with the specified name is not exists.");

            if (child.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item with the specified name is not a directory.");

            this.childItemsInternal.Remove(child);
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
                throw new InvalidOperationException("Item kind is not volume or directory.");

            IFSItem file = new FSFile(name); // validate name

            IFSItem child = this.childItemsInternal.Where(item => FSItemNameCompareRules.EqualNames(file.Name, item.Name)).FirstOrDefault();

            if (child is null)
                throw new InvalidOperationException("File with the specified name is not exists.");

            if (child.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item with the specified name is not a file.");

            if (child.LockedBy.Count > 0)
                throw new InvalidOperationException("File with the specified name is locked.");

            this.childItemsInternal.Remove(child);
        }

        /// <summary>
        /// Adds child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory, or if the child item is not a directory or a file,
        /// or if the item child list already contains child with the same name
        /// </exception>
        public void AddChild(IFSItem child)
        {
            if (this.Kind != FSItemKind.Volume && this.Kind != FSItemKind.Directory)
                throw new InvalidOperationException("Item is not a volume or a directory.");

            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (child.Kind != FSItemKind.Directory && child.Kind != FSItemKind.File)
                throw new InvalidOperationException("Child item is not a directory or a file.");

            if (this.childItemsInternal.Contains(child))
                throw new InvalidOperationException("Directory already contains child item (directory or file) with the same name.");

            child.Parent = this;
            this.childItemsInternal.Add(child);
        }

        /// <summary>
        /// Remove child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory,
        /// or if the child item is not a directory or a file,
        /// or if the child item is a locked file,
        /// or if the child item is not contains in the item child list
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
                if (child.LockedBy.Count > 0)
                    throw new InvalidOperationException("Child item is a locked file.");

            if (!this.childItemsInternal.Contains(child))
                throw new InvalidOperationException("Directory not contains child item (directory or file) with the specified name.");

            this.childItemsInternal.Remove(child);
        }

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
        /// Constructor
        /// </summary>
        /// <param name="kind">Item Kind</param>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the kind is incorrect</exception>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        public FSItem(FSItemKind kind, string name)
        {
            if (!Enum.IsDefined(typeof(FSItemKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, Invariant($"{nameof(kind)} is incorrect."));

            this.ValidateName(name);

            this.Kind = kind;

            this.Name = name.Trim();
        }
    }

}
