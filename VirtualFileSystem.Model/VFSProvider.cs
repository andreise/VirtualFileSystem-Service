using System;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// Virtual File System Provider
    /// </summary>
    public static class VFSProvider
    {

        /// <summary>
        /// Creates a new virtual file system
        /// </summary>
        /// <param name="name">Virtual file system name</param>
        /// <returns>Returns a new virtual file system</returns>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public static IVFSConsole Create(string name, bool printTreeRoot = true) => new VirtualFS(name, printTreeRoot);

        private static readonly Lazy<IVFSConsole> defaultInstance = new Lazy<IVFSConsole>(() => Create("Default Virtual File System"));

        /// <summary>
        /// Default Virtual File System
        /// </summary>
        public static IVFSConsole Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static VFSProvider()
        {
        }

    }

}
