using System;
using System.Collections.Generic;
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
        public virtual IFSItem Parent { get; protected internal set; }

        protected internal readonly HashSet<IFSItem> childItemsInternal = new HashSet<IFSItem>(FSItemEqualityComparer.Default);

        /// <summary>
        /// Child Items
        /// </summary>
        public IReadOnlyCollection<IFSItem> ChildItems => childItemsInternal;

        private HashSet<string> lockedBy = new HashSet<string>();

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        protected virtual void ValidateName(string name)
        {
            if ((object)name == null)
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
        /// <exception cref="InvalidOperationException">Throws if item is not a file</exception>
        public void Lock(string userName)
        {
            if (this.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if ((object)userName == null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{userName} is empty."));
        }

        /// <summary>
        /// Unlock Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file</exception>
        public void Unlock(string userName)
        {
            if (this.Kind != FSItemKind.File)
                throw new InvalidOperationException("Item is not a file.");

            if ((object)userName == null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: Invariant($"{userName} is empty."));

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
