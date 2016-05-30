using System;
using static System.FormattableString;

namespace VFSClient.Model.Implementation
{

    /// <summary>
    /// File System Directory
    /// </summary>
    internal class FSDirectory : FSItem
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
                throw new ArgumentException(Invariant($"The {nameof(name)} is not a valid directory name."), nameof(name));
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
