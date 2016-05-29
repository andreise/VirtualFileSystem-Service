using System;

namespace VFSClient.Model
{

    /// <summary>
    /// Virtual File System Factory
    /// </summary>
    public static class VirtualFSFactory
    {

        /// <summary>
        /// Creates a new virtual file system
        /// </summary>
        /// <param name="name">Virtual file system name</param>
        /// <returns>Returns a new virtual file system</returns>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public static IVirtualFS Create(string name) => new VirtualFS(name);

        private static readonly Lazy<IVirtualFS> defaultInstance = new Lazy<IVirtualFS>(() => Create("Default Virtual File System"));

        /// <summary>
        /// Default Virtual File System
        /// </summary>
        private static IVirtualFS Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static VirtualFSFactory()
        {
        }

    }

}
