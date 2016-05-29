using System;

namespace VFSClient.Model
{

    public interface IVirtualFS : IFSItem
    {

        /// <summary>
        /// Current Volume
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Throws if the set value is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throws if the set value is empty or is not a valid volume name, or if a volume with a set value is not present in the file system
        /// </exception>
        string CurrentVolume { get; set; }

        /// <summary>
        /// Gets a current directory in a specified volume
        /// </summary>
        /// <param name="volumeName">Volume name</param>
        /// <returns>
        /// Returns a current directory in a specified volume, or null if a current directory is not setted in the specified volume.
        /// If the volume name is null, returns a current directory in a current volume
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Throws if the volume name is empty or is not a valid volume name, or if a volume with a set value is not present in the file system
        /// </exception>
        string GetCurrentDirectory(string volumeName);

        string SetCurrentDirectory(string volumeName, string path);

    }

}
