using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    public static class FileSystemItemFactory
    {

        private static void ValidateName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Invariant($"{nameof(name)} is empty."), nameof(name));
        }

        public static IFileSystemItem CreateRoot(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidFileSystemName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid file system name."));

            IFileSystemItem root = new FileSystemItem(
                FileSystemItemKind.Root,
                name,
                new FileSystemItemKind[] { },
                new FileSystemItemKind[] { FileSystemItemKind.Volume },
                "File system cannot have a parent item.",
                "File system can contain volumes only as child items."
            );

            IFileSystemItem defaultVolume = CreateVolume(PathUtils.Consts.ValidVolumeNames[0]);
            root.AddChild(defaultVolume);

            return root;
        }

        public static IFileSystemItem CreateVolume(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidVolumeName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid volume name."));

            return new FileSystemItem(
                FileSystemItemKind.Volume,
                name,
                new FileSystemItemKind[] { FileSystemItemKind.Root },
                new FileSystemItemKind[] { FileSystemItemKind.Directory, FileSystemItemKind.File },
                "Volume can have only a file system as a parent.",
                "Volume can contain directories and files only as child items."
            );
        }

        public static IFileSystemItem CreateDirectory(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidDirectoryName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid directory name."));

            return new FileSystemItem(
                FileSystemItemKind.Directory,
                name,
                new FileSystemItemKind[] { FileSystemItemKind.Volume, FileSystemItemKind.Directory },
                new FileSystemItemKind[] { FileSystemItemKind.Directory, FileSystemItemKind.File },
                "Directory can have only a volume or a directory as a parent.",
                "Directory can contain directories and files only as child items."
            );
        }

        public static IFileSystemItem CreateFile(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidFileName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid file name."));

            return new FileSystemItem(
                FileSystemItemKind.File,
                name,
                new FileSystemItemKind[] { FileSystemItemKind.Volume, FileSystemItemKind.Directory },
                new FileSystemItemKind[] { },
                "File can have only a volume or a directory as a parent.",
                "File cannot contain child items."
            );
        }

    }

}
