using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Volume
    /// </summary>
    internal sealed class FSVolume : FSItemBase
    {

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

            if (parent.Kind != FSItemKind.FileSystem)
                throw new ArgumentException("Volume can have only a file system as a parent.");
        }

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid volume name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidVolumeName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid volume name."));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Volume Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid volume name</exception>
        public FSVolume(string name) : base(FSItemKind.Volume, name)
        {
        }

    }

}
