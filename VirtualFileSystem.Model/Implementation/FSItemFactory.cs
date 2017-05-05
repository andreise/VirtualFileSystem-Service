using System;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    internal static class FSItemFactory
    {

        private static void ValidateName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Invariant($"{nameof(name)} is empty."), nameof(name));
        }

        public static IFSItem CreateRoot(string name)
        {
            ValidateName(name);

            if (!FSPath.IsValidFileSystemName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid file system name."));

            IFSItem root = new FSItem(
                FSItemKind.Root,
                name,
                new FSItemKind[] { },
                new FSItemKind[] { FSItemKind.Volume },
                "File system cannot have a parent item.",
                "File system can contain volumes only as child items."
            );

            IFSItem defaultVolume = CreateVolume(FSPath.Consts.ValidVolumeNames[0]);
            root.AddChild(defaultVolume);

            return root;
        }

        public static IFSItem CreateVolume(string name)
        {
            ValidateName(name);

            if (!FSPath.IsValidVolumeName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid volume name."));

            return new FSItem(
                FSItemKind.Volume,
                name,
                new FSItemKind[] { FSItemKind.Root },
                new FSItemKind[] { FSItemKind.Directory, FSItemKind.File },
                "Volume can have only a file system as a parent.",
                "Volume can contain directories and files only as child items."
            );
        }

        public static IFSItem CreateDirectory(string name)
        {
            ValidateName(name);

            if (!FSPath.IsValidDirectoryName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid directory name."));

            return new FSItem(
                FSItemKind.Directory,
                name,
                new FSItemKind[] { FSItemKind.Volume, FSItemKind.Directory },
                new FSItemKind[] { FSItemKind.Directory, FSItemKind.File },
                "Directory can have only a volume or a directory as a parent.",
                "Directory can contain directories and files only as child items."
            );
        }

        public static IFSItem CreateFile(string name)
        {
            ValidateName(name);

            if (!FSPath.IsValidFileName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid file name."));

            return new FSItem(
                FSItemKind.File,
                name,
                new FSItemKind[] { FSItemKind.Volume, FSItemKind.Directory },
                new FSItemKind[] { },
                "File can have only a volume or a directory as a parent.",
                "File cannot contain child items."
            );
        }

    }

}
