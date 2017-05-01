using System;
using System.Linq;
using System.Text;
using VFSCommon;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// Virtual File System
    /// </summary>
    internal sealed class VirtualFS : FSItemBase, IVirtualFS
    {

        // IVFSConsole

        private static string DefaultVolumePath => FSPath.Consts.ValidVolumeNames[0];

        private static string NormalizeCurrentDirectory(string currentDirectory) =>
            string.IsNullOrWhiteSpace(currentDirectory) ? DefaultVolumePath : currentDirectory.Trim();

        public string MakeDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(directory))
                directory = FSPath.CombinePath(currentDirectory, directory);

            string[] directoryParts = FSPath.SplitPath(directory);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length - 1; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException("Destination path is not a volume or a directory.");

            currentItem.AddChildDirectory(directoryParts[directoryParts.Length - 1]);
            return directory;
        }

        public string ChangeDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(directory))
                directory = FSPath.CombinePath(currentDirectory, directory);

            string[] directoryParts = FSPath.SplitPath(directory);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException("Destination path is not a volume or a directory.");

            return directory;
        }

        public string RemoveDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(directory))
                directory = FSPath.CombinePath(currentDirectory, directory);

            string[] directoryParts = FSPath.SplitPath(directory);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException("Destination path is not a directory.");

            currentItem.Parent.RemoveChildDirectory(currentItem.Name);

            return directory;
        }

        private static bool IsLockedItem(IFSItem item)
        {
            return item.Kind == FSItemKind.File && item.LockedBy.Count > 0;
        }

        private static bool HasLocks(IFSItem item)
        {
            if (IsLockedItem(item))
                return true;

            foreach (IFSItem child in item.ChildItems)
                if (HasLocks(child))
                    return true;

            return false;
        }

        public string DeleteTree(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(directory))
                directory = FSPath.CombinePath(currentDirectory, directory);

            string[] directoryParts = FSPath.SplitPath(directory);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException("Destination path is not a directory.");


            if (HasLocks(currentItem))
                throw new VFSConsoleException("Directory or its subdirectories contain one or more locked files.");

            currentItem.Parent.RemoveChildDirectoryWithTree(currentItem.Name);

            return directory;
        }

        public string MakeFile(string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(fileName))
                fileName = FSPath.CombinePath(currentDirectory, fileName);

            string[] directoryParts = FSPath.SplitPath(fileName);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length - 1; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException("Destination path is not a volume or a directory.");

            currentItem.AddChildFile(directoryParts[directoryParts.Length - 1]);
            return fileName;
        }

        public string DeleteFile(string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(fileName))
                fileName = FSPath.CombinePath(currentDirectory, fileName);

            string[] directoryParts = FSPath.SplitPath(fileName);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new VFSConsoleException("Destination path is not a file.");

            currentItem.Parent.RemoveChildFile(currentItem.Name);

            return fileName;
        }

        public string LockFile(string userName, string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(fileName))
                fileName = FSPath.CombinePath(currentDirectory, fileName);

            string[] directoryParts = FSPath.SplitPath(fileName);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new VFSConsoleException("Destination path is not a file.");

            currentItem.Lock(userName);

            return fileName;
        }

        public string UnlockFile(string userName, string currentDirectory, string fileName)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(fileName))
                fileName = FSPath.CombinePath(currentDirectory, fileName);

            string[] directoryParts = FSPath.SplitPath(fileName);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, directoryParts[i]));
                if (currentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new VFSConsoleException("Destination path is not a file.");

            currentItem.Unlock(userName);

            return fileName;
        }

        private static void CopyItemTree(IFSItem item, IFSItem destItem)
        {
            if (item.Kind != FSItemKind.Directory && item.Kind != FSItemKind.File)
                throw new InvalidOperationException(Invariant($"{nameof(item)} is not a directory or a file."));

            if (destItem.Kind != FSItemKind.Volume && destItem.Kind != FSItemKind.Directory)
                throw new InvalidOperationException(Invariant($"{nameof(destItem)} is not a volume or directory."));

            IFSItem itemCopy;

            if (item.Kind == FSItemKind.Directory)
                itemCopy = destItem.AddChildDirectory(item.Name);
            else
                itemCopy = destItem.AddChildFile(item.Name);

            foreach (IFSItem child in item.ChildItems)
                CopyItemTree(child, itemCopy);
        }

        private void CopyOrMoveInternal(string currentDirectory, string sourcePath, string destPath, bool move)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);

            if (!FSPath.IsAbsolutePath(sourcePath))
                sourcePath = FSPath.CombinePath(currentDirectory, sourcePath);

            if (!FSPath.IsAbsolutePath(destPath))
                destPath = FSPath.CombinePath(currentDirectory, destPath);

            string[] sourcePathParts = FSPath.SplitPath(sourcePath);

            IFSItem sourcePathCurrentItem = this;

            for (int i = 0; i < sourcePathParts.Length; i++)
            {
                sourcePathCurrentItem = sourcePathCurrentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, sourcePathParts[i]));
                if (sourcePathCurrentItem is null)
                    throw new VFSConsoleException("Source path is not exists.");
            }

            if (sourcePathCurrentItem.Kind != FSItemKind.Directory && sourcePathCurrentItem.Kind != FSItemKind.File)
                throw new VFSConsoleException(Invariant($"'{sourcePath}' is not a directory or a file."));

            if (sourcePathCurrentItem.Kind == FSItemKind.File)
            {
                if (sourcePathCurrentItem.LockedBy.Count > 0)
                    throw new VFSConsoleException(Invariant($"File '{sourcePath}' is locked."));
            }

            string[] destPathParts = FSPath.SplitPath(destPath);

            IFSItem destPathCurrentItem = this;

            for (int i = 0; i < destPathParts.Length; i++)
            {
                destPathCurrentItem = destPathCurrentItem.ChildItems.FirstOrDefault(item => FSItemNameComparerProvider.Default.Equals(item.Name, destPathParts[i]));
                if (destPathCurrentItem is null)
                    throw new VFSConsoleException("Destination path is not exists.");
            }

            if (destPathCurrentItem.Kind != FSItemKind.Volume && destPathCurrentItem.Kind != FSItemKind.Directory)
                throw new VFSConsoleException(Invariant($"'{destPath}' is not a volume or a directory."));

            if (sourcePathCurrentItem == destPathCurrentItem)
                throw new VFSConsoleException("Source path and destination path should be not equal.");

            if (sourcePathCurrentItem.Parent == destPathCurrentItem)
                throw new VFSConsoleException("Source path cannot be copied or moved to its parent.");

            if (sourcePathCurrentItem.Kind == FSItemKind.Directory)
            {
                IFSItem destPathCurrentItemParent = destPathCurrentItem.Parent;
                while (!(destPathCurrentItemParent is null))
                {
                    if (destPathCurrentItemParent == sourcePathCurrentItem)
                        throw new VFSConsoleException("Source directory cannot be a parent of the dest directory.");

                    destPathCurrentItemParent = destPathCurrentItemParent.Parent;
                }
            }

            if (HasLocks(sourcePathCurrentItem))
                throw new VFSConsoleException("Source path contain one or more locked files and cannot be moved.");

            if (move)
            {
                IFSItem prevParent = sourcePathCurrentItem.Parent;
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

        public bool PrintTreeRoot { get; }

        private void PrintTreeHelper(IFSItem item, StringBuilder builder)
        {
            void PrintItem()
            {
                if (item.Kind == FSItemKind.FileSystem && !this.PrintTreeRoot)
                    return;

                if (builder.Length > 0)
                    builder.AppendLine();

                int itemLevel = item.GetLevel();

                if (!this.PrintTreeRoot)
                    itemLevel--;

                if (itemLevel > 0)
                {
                    string GetIndent()
                    {
                        var indent = new StringBuilder(2 * itemLevel);
                        for (int i = 0; i < itemLevel; i++)
                            indent.Append("| ");
                        indent[indent.Length - 1] = '_';
                        return indent.ToString();
                    }

                    builder.Append(GetIndent());
                }

                builder.Append(item.Name);

                switch (item.Kind)
                {
                    case FSItemKind.Directory:
                        builder.Append(" [DIR]");
                        break;

                    case FSItemKind.File:
                        builder.Append(" [FILE]");
                        if (item.LockedBy.Count > 0)
                        {
                            var lockedBy = item.LockedBy.OrderBy(userName => userName, UserNameComparerProvider.Default);
                            builder.Append(Invariant($"[LOCKED BY: {string.Join(", ", lockedBy)}]"));
                        }
                        break;
                }
            }

            PrintItem();

            foreach (var childGroup in item.ChildItems.GroupBy(child => child.Kind).OrderBy(group => group.Key))
            {
                foreach (var child in childGroup.OrderBy(child => child.Name, FSItemNameComparerProvider.Default))
                    PrintTreeHelper(child, builder);
            }
        }

        public string PrintTree()
        {
            var builder = new StringBuilder();
            PrintTreeHelper(this, builder);
            return builder.ToString();
        }

        // Base

        /// <summary>
        /// Parent Item. Always returns null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws on a setting attempt</exception>
        public override IFSItem Parent
        {
            get => null;
            set => throw new InvalidOperationException("File system cannot has a parent item.");
        }

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
                throw new ArgumentException(Invariant($"'{name}' is not a valid file system name."));
        }

        // Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">File System Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public VirtualFS(string name, bool printTreeRoot) : base(FSItemKind.FileSystem, name)
        {
            this.PrintTreeRoot = printTreeRoot;

            IFSItem defaultVolume = new FSVolume(DefaultVolumePath);
            this.AddChild(defaultVolume);
        }

    }

}
