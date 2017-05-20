using System;
using System.Linq;
using System.Text;
using static System.FormattableString;

namespace VirtualFileSystem.Model.Console.Implementation
{
    using VirtualFileSystem.Common.Security;

    /// <summary>
    /// File System Console
    /// </summary>
    public sealed class FileSystemConsole : IFileSystemConsole
    {

        private readonly IFileSystemItem root;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">File System Root Item</param>
        /// <exception cref="ArgumentNullException">Throws if the root is null</exception>
        /// <exception cref="ArgumentException">Throws if the item is not a root item</exception>
        public FileSystemConsole(IFileSystemItem root)
        {
            if (root is null)
                throw new ArgumentNullException(paramName: nameof(root));

            if (root.Kind != FileSystemItemKind.Root)
                throw new ArgumentException(paramName: nameof(root), message: "Item must be a root item.");

            this.root = root;
        }

        private static string DefaultVolumePath => PathUtils.Consts.ValidVolumeNames[0];

        private static string NormalizeCurrentDirectory(string currentDirectory) =>
            string.IsNullOrWhiteSpace(currentDirectory) ? DefaultVolumePath : currentDirectory.Trim();

        public string MakeDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(directory))
                directory = PathUtils.CombinePath(currentDirectory, directory);

            string[] directoryParts = PathUtils.SplitPath(directory);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length - 1; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.Volume && currentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException("Destination path is not a volume or a directory.");

            currentItem.AddChildDirectory(directoryParts[directoryParts.Length - 1]);
            return directory;
        }

        public string ChangeDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(directory))
                directory = PathUtils.CombinePath(currentDirectory, directory);

            string[] directoryParts = PathUtils.SplitPath(directory);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.Volume && currentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException("Destination path is not a volume or a directory.");

            return directory;
        }

        public string RemoveDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(directory))
                directory = PathUtils.CombinePath(currentDirectory, directory);

            string[] directoryParts = PathUtils.SplitPath(directory);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException("Destination path is not a directory.");

            currentItem.Parent.RemoveChildDirectory(currentItem.Name);

            return directory;
        }

        private static bool HasLocks(IFileSystemItem item)
        {
            if (item.LockedBy.Count > 0)
                return true;

            foreach (IFileSystemItem child in item.ChildItems)
            {
                if (HasLocks(child))
                    return true;
            }

            return false;
        }

        public string DeleteTree(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(directory))
                directory = PathUtils.CombinePath(currentDirectory, directory);

            string[] directoryParts = PathUtils.SplitPath(directory);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException("Destination path is not a directory.");

            if (HasLocks(currentItem))
                throw new FileSystemConsoleException("Directory or its subdirectories contain one or more locked files.");

            currentItem.Parent.RemoveChildDirectoryWithTree(currentItem.Name);

            return directory;
        }

        public string MakeFile(string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(fileName))
                fileName = PathUtils.CombinePath(currentDirectory, fileName);

            string[] directoryParts = PathUtils.SplitPath(fileName);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length - 1; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.Volume && currentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException("Destination path is not a volume or a directory.");

            currentItem.AddChildFile(directoryParts[directoryParts.Length - 1]);
            return fileName;
        }

        public string DeleteFile(string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(fileName))
                fileName = PathUtils.CombinePath(currentDirectory, fileName);

            string[] directoryParts = PathUtils.SplitPath(fileName);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.File)
                throw new FileSystemConsoleException("Destination path is not a file.");

            currentItem.Parent.RemoveChildFile(currentItem.Name);

            return fileName;
        }

        public string LockFile(string userName, string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(fileName))
                fileName = PathUtils.CombinePath(currentDirectory, fileName);

            string[] directoryParts = PathUtils.SplitPath(fileName);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.File)
                throw new FileSystemConsoleException("Destination path is not a file.");

            currentItem.Lock(userName);

            return fileName;
        }

        public string UnlockFile(string userName, string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!PathUtils.IsAbsolutePath(fileName))
                fileName = PathUtils.CombinePath(currentDirectory, fileName);

            string[] directoryParts = PathUtils.SplitPath(fileName);

            IFileSystemItem currentItem = this.root;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FileSystemItemKind.File)
                throw new FileSystemConsoleException("Destination path is not a file.");

            currentItem.Unlock(userName);

            return fileName;
        }

        private static void CopyItemTree(IFileSystemItem item, IFileSystemItem destItem)
        {
            if (item.Kind != FileSystemItemKind.Directory && item.Kind != FileSystemItemKind.File)
                throw new InvalidOperationException(Invariant($"{nameof(item)} is not a directory or a file."));

            if (destItem.Kind != FileSystemItemKind.Volume && destItem.Kind != FileSystemItemKind.Directory)
                throw new InvalidOperationException(Invariant($"{nameof(destItem)} is not a volume or directory."));

            IFileSystemItem itemCopy;

            if (item.Kind == FileSystemItemKind.Directory)
                itemCopy = destItem.AddChildDirectory(item.Name);
            else
                itemCopy = destItem.AddChildFile(item.Name);

            foreach (IFileSystemItem child in item.ChildItems)
                CopyItemTree(child, itemCopy);
        }

        private void CopyOrMoveInternal(string currentDirectory, string sourcePath, string destPath, bool move)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);

            if (!PathUtils.IsAbsolutePath(sourcePath))
                sourcePath = PathUtils.CombinePath(currentDirectory, sourcePath);

            if (!PathUtils.IsAbsolutePath(destPath))
                destPath = PathUtils.CombinePath(currentDirectory, destPath);

            string[] sourcePathParts = PathUtils.SplitPath(sourcePath);

            IFileSystemItem sourcePathCurrentItem = this.root;

            for (int i = 0; i < sourcePathParts.Length; i++)
            {
                sourcePathCurrentItem = sourcePathCurrentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, sourcePathParts[i]));
                if (sourcePathCurrentItem is null)
                    throw new FileSystemConsoleException("Source path is not exists.");
            }

            if (sourcePathCurrentItem.Kind != FileSystemItemKind.Directory && sourcePathCurrentItem.Kind != FileSystemItemKind.File)
                throw new FileSystemConsoleException(Invariant($"'{sourcePath}' is not a directory or a file."));

            if (sourcePathCurrentItem.Kind == FileSystemItemKind.File)
            {
                if (sourcePathCurrentItem.LockedBy.Count > 0)
                    throw new FileSystemConsoleException(Invariant($"File '{sourcePath}' is locked."));
            }

            string[] destPathParts = PathUtils.SplitPath(destPath);

            IFileSystemItem destPathCurrentItem = this.root;

            for (int i = 0; i < destPathParts.Length; i++)
            {
                destPathCurrentItem = destPathCurrentItem.ChildItems.FirstOrDefault(item => FileSystemItemNameComparerProvider.Default.Equals(item.Name, destPathParts[i]));
                if (destPathCurrentItem is null)
                    throw new FileSystemConsoleException("Destination path is not exists.");
            }

            if (destPathCurrentItem.Kind != FileSystemItemKind.Volume && destPathCurrentItem.Kind != FileSystemItemKind.Directory)
                throw new FileSystemConsoleException(Invariant($"'{destPath}' is not a volume or a directory."));

            if (sourcePathCurrentItem == destPathCurrentItem)
                throw new FileSystemConsoleException("Source path and destination path must be not equal.");

            if (sourcePathCurrentItem.Parent == destPathCurrentItem)
                throw new FileSystemConsoleException("Source path cannot be copied or moved to its parent.");

            if (sourcePathCurrentItem.Kind == FileSystemItemKind.Directory)
            {
                IFileSystemItem destPathCurrentItemParent = destPathCurrentItem.Parent;
                while (!(destPathCurrentItemParent is null))
                {
                    if (destPathCurrentItemParent == sourcePathCurrentItem)
                        throw new FileSystemConsoleException("Source directory cannot be a parent of the dest directory.");

                    destPathCurrentItemParent = destPathCurrentItemParent.Parent;
                }
            }

            if (HasLocks(sourcePathCurrentItem))
                throw new FileSystemConsoleException("Source path contain one or more locked files and cannot be moved.");

            if (move)
            {
                IFileSystemItem prevParent = sourcePathCurrentItem.Parent;
                destPathCurrentItem.AddChild(sourcePathCurrentItem);
                prevParent.RemoveChild(sourcePathCurrentItem);
            }
            else
            {
                CopyItemTree(sourcePathCurrentItem, destPathCurrentItem);
            }
        }

        public void Copy(string currentDirectory, string sourcePath, string destPath) => CopyOrMoveInternal(
            currentDirectory, sourcePath, destPath, move: false
        );

        public void Move(string currentDirectory, string sourcePath, string destPath) => CopyOrMoveInternal(
            currentDirectory, sourcePath, destPath, move: true
        );

        private static void PrintTreeHelper(IFileSystemItem item, StringBuilder builder, bool printRoot)
        {
            void PrintItem()
            {
                if (item.Kind == FileSystemItemKind.Root && !printRoot)
                    return;

                if (builder.Length > 0)
                    builder.AppendLine();

                int itemLevel = item.GetLevel();

                if (!printRoot)
                    itemLevel--;

                if (itemLevel > 0)
                {
                    for (int i = 0; i < itemLevel; i++)
                        builder.Append("| ");

                    builder[builder.Length - 1] = '_';
                }

                builder.Append(item.Name);

                string GetItemKindDescription()
                {
                    switch (item.Kind)
                    {
                        case FileSystemItemKind.Directory:
                            return " [DIR]";
                        case FileSystemItemKind.File:
                            return " [FILE]";
                        default:
                            return null;
                    }
                }

                builder.Append(GetItemKindDescription());

                if (item.LockedBy.Count > 0)
                {
                    var lockedBy = item.LockedBy.OrderBy(userName => userName, UserNameComparerProvider.Default);
                    builder.Append(Invariant($" [LOCKED BY: {string.Join(", ", lockedBy)}]"));
                }
            }

            PrintItem();

            foreach (var childGroup in item.ChildItems.GroupBy(child => child.Kind).OrderBy(group => group.Key))
            {
                foreach (var child in childGroup.OrderBy(child => child.Name, FileSystemItemNameComparerProvider.Default))
                    PrintTreeHelper(child, builder, printRoot);
            }
        }

        public string PrintTree(bool printRoot)
        {
            var builder = new StringBuilder();
            PrintTreeHelper(this.root, builder, printRoot);
            return builder.ToString();
        }

    }

}
