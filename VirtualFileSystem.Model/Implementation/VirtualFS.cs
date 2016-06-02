using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
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
            IFSItem defaultVolume = new FSVolume(FSPath.ValidVolumeNames[0]);
            this.childItemsInternal.Add(defaultVolume);
            //this.currentVolume = defaultVolume;
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

        //    private IFSItem currentVolume;

        //    private static IFSItem CreateVolume(string volumeName)
        //    {
        //        if ((object)volumeName == null)
        //            throw new ArgumentNullException(paramName: nameof(volumeName), message: Invariant($"The {nameof(volumeName)} to set to current volume is null."));

        //        if (string.IsNullOrWhiteSpace(volumeName))
        //            throw new ArgumentException(paramName: nameof(volumeName), message: Invariant($"The {nameof(volumeName)} to set to current volume is empty."));

        //        volumeName = volumeName.Trim().TrimEnd(FSPath.PathSeparator, FSPath.AltPathSeparator);

        //        //if (!FSPath.IsValidVolumeName(volumeName))
        //        //    throw new ArgumentException(paramName: nameof(volumeName), message: Invariant($"The {nameof(volumeName)} to set to current volume is not a valid volume name."));

        //        return new FSVolume(volumeName);
        //    }

        //    /// <summary>
        //    /// Current Volume. Returns empty string if a current volume is not set in the file system
        //    /// </summary>
        //    /// <exception cref="ArgumentNullException">
        //    /// Throws if the value to set is null
        //    /// </exception>
        //    /// <exception cref="ArgumentException">
        //    /// Throws if the value to set is empty or is not a valid volume name, or if a volume with a value to set is not present in the file system
        //    /// </exception>
        //    public string CurrentVolume
        //    {

        //        get
        //        {
        //            return this.currentVolume?.Name ?? string.Empty;
        //        }

        //        set
        //        {
        //            IFSItem newCurrentVolume = CreateVolume(value);

        //            if (!this.childItemsInternal.Contains(newCurrentVolume))
        //                throw new ArgumentException(paramName: nameof(value), message: Invariant($"A volume with the specified name is not present in the file system."));

        //            this.currentVolume = newCurrentVolume;
        //        }

        //    }

        //    /// <summary>
        //    /// Gets a current directory in a specified volume, including volume name
        //    /// </summary>
        //    /// <param name="volumeName">Volume name</param>
        //    /// <returns>
        //    /// Returns a current directory in a specified volume, or null if a current directory is not setted in the specified volume.
        //    /// If the volume name is null, returns a current directory in a current volume
        //    /// </returns>
        //    /// <exception cref="ArgumentException">
        //    /// Throws if the volume name is empty or is not a valid volume name, or if a volume with a set value is not present in the file system
        //    /// </exception>
        //    public string GetCurrentDirectory(string volumeName)
        //    {
        //        IFSItem volume;

        //        if ((object)volumeName == null)
        //        {
        //            if ((object)this.currentVolume == null)
        //                throw new ArgumentException(paramName: nameof(volumeName), message: "A current volume is not set in the file system.");
        //            volume = this.currentVolume;
        //        }
        //        else
        //        {
        //            volume = CreateVolume(volumeName);
        //        }

        //        return Invariant($"{volume.Name}{FSPath.PathSeparator}");

        //    }

    }

}
