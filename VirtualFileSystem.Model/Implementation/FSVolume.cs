using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Volume
    /// </summary>
    internal sealed class FSVolume : FSItemBase
    {

        /// <summary>
        /// Valid Parent Kinds
        /// </summary>
        protected override IReadOnlyCollection<FSItemKind> ValidParentKinds { get; } =
            new ReadOnlyCollection<FSItemKind>(new FSItemKind[] { FSItemKind.FileSystem });

        /// <summary>
        /// Valid Child Kinds
        /// </summary>
        protected override IReadOnlyCollection<FSItemKind> ValidChildKinds { get; } =
            new ReadOnlyCollection<FSItemKind>(new FSItemKind[] { FSItemKind.Directory, FSItemKind.File });

        /// <summary>
        /// Valid Parent Kinds Message
        /// </summary>
        protected override string ValidParentKindsMessage => "Volume can have only a file system as a parent.";

        /// <summary>
        /// Valid Child Kinds Message
        /// </summary>
        protected override string ValidChildKindsMessage => "Volume can contain directories and files only as child items.";

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
