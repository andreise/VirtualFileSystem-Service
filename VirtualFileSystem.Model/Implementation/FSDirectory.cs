using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Directory
    /// </summary>
    internal sealed class FSDirectory : FSItemBase
    {

        /// <summary>
        /// Valid Parent Kinds
        /// </summary>
        protected override IReadOnlyCollection<FSItemKind> ValidParentKinds { get; } =
            new ReadOnlyCollection<FSItemKind>(new FSItemKind[] { FSItemKind.Volume, FSItemKind.Directory });

        /// <summary>
        /// Valid Child Kinds
        /// </summary>
        protected override IReadOnlyCollection<FSItemKind> ValidChildKinds { get; } =
            new ReadOnlyCollection<FSItemKind>(new FSItemKind[] { FSItemKind.Directory, FSItemKind.File });

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
        protected override void ValidateSetParent(IFSItem parent)
        {
            base.ValidateSetParent(parent);

            if (parent.Kind != FSItemKind.Volume && parent.Kind != FSItemKind.Directory)
                throw new ArgumentException("Directory can have only a volume or a directory as a parent.");
        }

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid directory name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidDirectoryName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid directory name."));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Directory Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid directory name</exception>
        public FSDirectory(string name) : base(FSItemKind.Directory, name)
        {
        }

    }

}
