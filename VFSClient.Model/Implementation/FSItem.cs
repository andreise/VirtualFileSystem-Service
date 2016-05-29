using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VFSClient.Model
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
        public IFSItem Parent { get; internal set; } = null;

        internal readonly HashSet<IFSItem> childItemsInternal = new HashSet<IFSItem>(FSItemEqualityComparer.Default);

        /// <summary>
        /// Child Items
        /// </summary>
        public IReadOnlyCollection<IFSItem> ChildItems => childItemsInternal;

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        protected virtual void ValidateName(string name)
        {
            if ((object)name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Length == 0)
                throw new ArgumentException(nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="kind">FSItem Kind</param>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the kind is incorrect</exception>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        public FSItem(FSItemKind kind, string name)
        {
            if (!Enum.IsDefined(typeof(FSItemKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "FSItem kind is incorrect.");

            this.ValidateName(name);

            this.Kind = kind;

            this.Name = name;
        }
    }

}
