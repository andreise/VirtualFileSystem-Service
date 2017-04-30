using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Volume
    /// </summary>
    internal sealed class FSVolume : FSItem
    {

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

        /// <summary>
        /// Parent Item
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Throws if this item and new parent item is the same item,
        /// or if new parent item is not null and is not a file system
        /// </exception>
        public override IFSItem Parent
        {
            get => base.Parent;
            set
            {
                if (!(value is null))
                {
                    if (value.Kind != FSItemKind.FileSystem)
                        throw new ArgumentException("Volume can have only a file system as a parent.");
                }

                base.Parent = value;
            }
        }

    }

}
