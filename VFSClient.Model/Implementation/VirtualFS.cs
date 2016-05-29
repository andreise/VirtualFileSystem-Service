using System;
using static System.FormattableString;

namespace VFSClient.Model
{

    /// <summary>
    /// Virtual File System
    /// </summary>
    internal class VirtualFS : FSItem, IVirtualFS
    {

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidFileSystemName(name))
                throw new ArgumentException(Invariant($"The {nameof(name)} is not a valid directory name."), nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">File System Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public VirtualFS(string name) : base(FSItemKind.FileSystem, name)
        {
            this.childItemsInternal.Add(new FSVolume(FSPath.ValidVolumeNames[0]));
        }

        /// <summary>
        /// Parent Item. Always returns null
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws on a setting attempt</exception>
        public override IFSItem Parent
        {
            get
            {
                return null;
            }

            protected internal set
            {
                throw new InvalidOperationException("File system cannot has a parent item.");
            }
        }
    }

}
