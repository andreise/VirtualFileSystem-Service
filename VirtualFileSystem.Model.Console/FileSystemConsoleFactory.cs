namespace VirtualFileSystem.Model.Console
{
    using Implementation;

    /// <summary>
    /// File System Console Factory
    /// </summary>
    public static class FileSystemConsoleFactory
    {

        /// <summary>
        /// Creates a new console for the specified file system 
        /// </summary>
        /// <param name="root">File System Root Item</param>
        /// <exception cref="ArgumentNullException">Throws if the root is null</exception>
        /// <exception cref="ArgumentException">Throws if the item is not a root item</exception>
        public static IFileSystemConsole Create(IFileSystemItem root) => new FileSystemConsole(root);

    }

}
