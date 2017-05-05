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
        /// Valid Parent Kinds Message
        /// </summary>
        protected override string ValidParentKindsMessage => "Directory can have only a volume or a directory as a parent.";

        /// <summary>
        /// Valid Child Kinds Message
        /// </summary>
        protected override string ValidChildKindsMessage => string.Empty;

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
