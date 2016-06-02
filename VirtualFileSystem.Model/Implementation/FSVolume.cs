using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Volume
    /// </summary>
    internal class FSVolume : FSItem
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
                throw new ArgumentException(Invariant($"The {nameof(name)} is not a valid volume name."), nameof(name));
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
