using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Directory
    /// </summary>
    internal sealed class FSDirectory : FSItemBase
    {

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

        /// <summary>
        /// Parent Item
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Throws if this item and new parent item is the same item,
        /// or if new parent item is not null and is not a volume or a directory
        /// </exception>
        public override IFSItem Parent
        {
            get => base.Parent;
            set
            {
                if (!(value is null))
                {
                    if (value.Kind != FSItemKind.Volume && value.Kind != FSItemKind.Directory)
                        throw new ArgumentException("Directory can have only a volume or a directory as a parent.");
                }

                base.Parent = value;
            }
        }

    }

}
