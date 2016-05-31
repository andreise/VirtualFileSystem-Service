using System;

namespace VFSClient.Model
{

    /// <summary>
    /// Virtual File System
    /// </summary>
    public interface IVirtualFS : IFSItem
    {

        

        ///// <summary>
        ///// Current Volume. Returns empty string if a current volume is not set in the file system
        ///// </summary>
        ///// <exception cref="ArgumentNullException">
        ///// Throws if the value to set is null
        ///// </exception>
        ///// <exception cref="ArgumentException">
        ///// Throws if the value to set is empty or is not a valid volume name, or if a volume with a value to set is not present in the file system
        ///// </exception>
        //string CurrentVolume { get; set; }

        ///// <summary>
        ///// Gets a current directory in a specified volume, including volume name
        ///// </summary>
        ///// <param name="volumeName">Volume name</param>
        ///// <returns>
        ///// Returns a current directory in a specified volume, or null if a current directory is not setted in the specified volume.
        ///// If the volume name is null, returns a current directory in a current volume
        ///// </returns>
        ///// <exception cref="ArgumentException">
        ///// Throws if the volume name is empty or is not a valid volume name, or if a volume with a set value is not present in the file system
        ///// </exception>
        //string GetCurrentDirectory(string volumeName);

    }

}
