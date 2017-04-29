using System;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// Virtual File System Factory
    /// </summary>
    public static class VirtualFSProvider
    {

        /// <summary>
        /// Creates a new virtual file system
        /// </summary>
        /// <param name="name">Virtual file system name</param>
        /// <returns>Returns a new virtual file system</returns>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public static IVirtualFS Create(string name, bool printTreeRoot = true) => new VirtualFS(name, printTreeRoot);

        private static readonly Lazy<IVirtualFS> defaultInstance = new Lazy<IVirtualFS>(() => Create("Default Virtual File System"));

        /// <summary>
        /// Default Virtual File System
        /// </summary>
        public static IVirtualFS Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static VirtualFSProvider()
        {
        }

    }

}
