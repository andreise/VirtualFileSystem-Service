using System;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Factory
    /// </summary>
    public static class FileSystemFactory
    {

        /// <summary>
        /// Creates a new file system
        /// </summary>
        /// <param name="name">File system name</param>
        /// <returns>Returns a new file system</returns>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public static IFileSystem Create(string name) => new FileSystem(name);

        private static readonly Lazy<IFileSystem> defaultInstance = new Lazy<IFileSystem>(() => Create("Default Virtual File System"));

        /// <summary>
        /// Default File System
        /// </summary>
        public static IFileSystem Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FileSystemFactory()
        {
        }

    }

}
