using System;
using System.Linq;
using System.Text;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    internal sealed class FSException : Exception
    {
        public FSException(string message): base(message)
        {
        }
    }

    /// <summary>
    /// Virtual File System
    /// </summary>
    internal sealed class VirtualFS : FSItem, IVirtualFS
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

        private static readonly string defaultVolumePath = FSPath.ValidVolumeNames[0];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">File System Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file system name</exception>
        public VirtualFS(string name) : base(FSItemKind.FileSystem, name)
        {
            IFSItem defaultVolume = new FSVolume(defaultVolumePath);
            this.childItemsInternal.Add(defaultVolume);
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static VirtualFS()
        {
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

            set
            {
                throw new InvalidOperationException("File system cannot has a parent item.");
            }

        }

        private static string NormalizeCurrentDirectory(string currentDirectory)
            => string.IsNullOrWhiteSpace(currentDirectory) ? defaultVolumePath : currentDirectory.Trim();

        public string MakeDirectory(string currentDirectory, string directory)
        {
            currentDirectory = NormalizeCurrentDirectory(currentDirectory);
            if (!FSPath.IsAbsolutePath(directory))
                directory = FSPath.CombinePath(currentDirectory, directory);

            string[] directoryParts = FSPath.SplitPath(directory);

            IFSItem currentItem = this;

            for (int i = 0; i < directoryParts.Length - 1; i++)
            {
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new FSException("Destination path is not a directory.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new FSException("Destination path is not a directory.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Directory)
                throw new FSException("Destination path is not a directory.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Directory)
                throw new FSException("Destination path is not a directory.");


            if (HasLocks(currentItem))
                throw new FSException("Directory or its subdirectories contains one or more locked files.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.Volume && currentItem.Kind != FSItemKind.Directory)
                throw new FSException("Destination path is not a directory.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new FSException("Destination path is not a file.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new FSException("Destination path is not a file.");

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
                currentItem = currentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, directoryParts[i]));
                if ((object)currentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (currentItem.Kind != FSItemKind.File)
                throw new FSException("Destination path is not a file.");

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
                sourcePathCurrentItem = sourcePathCurrentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, sourcePathParts[i]));
                if ((object)sourcePathCurrentItem == null)
                    throw new FSException("Source path is not exists.");
            }

            if (sourcePathCurrentItem.Kind != FSItemKind.Directory && sourcePathCurrentItem.Kind != FSItemKind.File)
                throw new FSException(Invariant($"{nameof(sourcePath)} is not a directory or a file."));

            if (sourcePathCurrentItem.Kind == FSItemKind.File)
                if (sourcePathCurrentItem.LockedBy.Count > 0)
                    throw new FSException(Invariant($"File '{sourcePath}' is locked."));

            string[] destPathParts = FSPath.SplitPath(destPath);

            IFSItem destPathCurrentItem = this;

            for (int i = 0; i < destPathParts.Length; i++)
            {
                destPathCurrentItem = destPathCurrentItem.ChildItems.FirstOrDefault(item => FSItemEqualityComparer.EqualNames(item.Name, destPathParts[i]));
                if ((object)destPathCurrentItem == null)
                    throw new FSException("Destination path is not exists.");
            }

            if (destPathCurrentItem.Kind != FSItemKind.Volume && destPathCurrentItem.Kind != FSItemKind.Directory)
                throw new FSException(Invariant($"{nameof(destPath)} is not a volume or a directory."));

            if (sourcePathCurrentItem == destPathCurrentItem)
                throw new FSException("Source path and destination path should be not equal.");

            if (sourcePathCurrentItem.Parent == destPathCurrentItem)
                throw new FSException("Source path cannot be copied or moved to its parent.");

            if (sourcePathCurrentItem.Kind == FSItemKind.Directory)
            {
                IFSItem destPathCurrentItemParent = destPathCurrentItem.Parent;
                while ((object)destPathCurrentItemParent != null)
                {
                    if (destPathCurrentItemParent == sourcePathCurrentItem)
                        throw new FSException("Source directory cannot be parent of the dest directory.");

                    destPathCurrentItemParent = destPathCurrentItemParent.Parent;
                }
            }

            if (HasLocks(sourcePathCurrentItem))
                throw new FSException("Source path contains one or more locked files and cannot be moved.");

            if (move)
            {
                sourcePathCurrentItem.Parent.RemoveChild(sourcePathCurrentItem);
                sourcePathCurrentItem.Parent = null;
                destPathCurrentItem.AddChild(sourcePathCurrentItem);
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

        private static void PrintTreeHelper(IFSItem item, StringBuilder builder)
        {
            if (item.Kind != FSItemKind.FileSystem)
            {
                int itemGeneration = 0;

                IFSItem itemParent = item.Parent;
                while ((object)itemParent != null)
                {
                    itemGeneration++;
                    itemParent = itemParent.Parent;
                }

                if (itemGeneration > 0)
                {
                    StringBuilder indent = new StringBuilder(2 * itemGeneration);
                    for (int i = 0; i < itemGeneration; i++)
                        indent.Append("| ");
                    indent[indent.Length - 1] = '_';
                    builder.Append(indent.ToString());
                }
                builder.Append(item.Name);
                if (item.Kind == FSItemKind.Directory)
                {
                    builder.Append(" [DIR]");
                }
                else if (item.Kind == FSItemKind.File)
                {
                    builder.Append(" [FILE]");
                    if (item.LockedBy.Count > 0)
                    {
                        var lockedBy = item.LockedBy.OrderBy(userName => userName, StringComparer.InvariantCulture);
                        builder.Append(Invariant($"[LOCKED BY: {string.Join(", ", lockedBy)}]"));
                    }
                }
                builder.AppendLine();
            }

            if (item.Kind == FSItemKind.FileSystem)
            {
                var volumes = item.ChildItems.OrderBy(item1 => item1, FSItemComparer.Default);
                foreach (IFSItem volume in volumes)
                    PrintTreeHelper(volume, builder);
            }
            if (item.Kind == FSItemKind.Volume || item.Kind == FSItemKind.Directory)
            {
                var directories = item.ChildItems.Where(item1 => item1.Kind == FSItemKind.Directory).OrderBy(item1 => item1, FSItemComparer.Default);
                foreach (IFSItem directory in directories)
                    PrintTreeHelper(directory, builder);

                var files = item.ChildItems.Where(item1 => item1.Kind == FSItemKind.File).OrderBy(item1 => item1, FSItemComparer.Default);
                foreach (IFSItem file in files)
                    PrintTreeHelper(file, builder);
            }
        }

        public string PrintTree()
        {
            var builder = new StringBuilder();
            PrintTreeHelper(this, builder);
            return builder.ToString();
        }

    }

}
