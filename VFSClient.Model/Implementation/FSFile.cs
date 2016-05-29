using System;
using static System.FormattableString;

namespace VFSClient.Model
{

    /// <summary>
    /// File System File
    /// </summary>
    internal class FSFile : FSItem
    {

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidFileName(name))
                throw new ArgumentException(Invariant($"The {nameof(name)} is not a valid file name."), nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">File Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file name</exception>
        public FSFile(string name) : base(FSItemKind.File, name)
        {
        }

    }

}
