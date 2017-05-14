using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{
    using Implementation;

    public static class FileSystemItemFactory
    {

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FileSystemItemFactory()
        {
        }

        private static IReadOnlyCollection<T> CreateReadOnlyCollection<T>(params T[] items) => new ReadOnlyCollection<T>(items);

        private static readonly IReadOnlyDictionary<FileSystemItemKind, IReadOnlyCollection<FileSystemItemKind>> validParentKinds =
            new Dictionary<FileSystemItemKind, IReadOnlyCollection<FileSystemItemKind>>()
            {
                [FileSystemItemKind.Root] = CreateReadOnlyCollection<FileSystemItemKind>(),
                [FileSystemItemKind.Volume] = CreateReadOnlyCollection(FileSystemItemKind.Root),
                [FileSystemItemKind.Directory] = CreateReadOnlyCollection(FileSystemItemKind.Volume, FileSystemItemKind.Directory),
                [FileSystemItemKind.File] = CreateReadOnlyCollection(FileSystemItemKind.Volume, FileSystemItemKind.Directory)
            };

        private static readonly IReadOnlyDictionary<FileSystemItemKind, IReadOnlyCollection<FileSystemItemKind>> validChildKinds =
            new Dictionary<FileSystemItemKind, IReadOnlyCollection<FileSystemItemKind>>()
            {
                [FileSystemItemKind.Root] = CreateReadOnlyCollection(FileSystemItemKind.Volume),
                [FileSystemItemKind.Volume] = CreateReadOnlyCollection(FileSystemItemKind.Directory, FileSystemItemKind.File),
                [FileSystemItemKind.Directory] = CreateReadOnlyCollection(FileSystemItemKind.Directory, FileSystemItemKind.File),
                [FileSystemItemKind.File] = CreateReadOnlyCollection<FileSystemItemKind>()
            };

        private static readonly IReadOnlyDictionary<FileSystemItemKind, string> validParentKindsMessage =
            new Dictionary<FileSystemItemKind, string>()
            {
                [FileSystemItemKind.Root] = "File system cannot have a parent item.",
                [FileSystemItemKind.Volume] = "Volume can have only a file system as a parent.",
                [FileSystemItemKind.Directory] = "Directory can have only a volume or a directory as a parent.",
                [FileSystemItemKind.File] = "File can have only a volume or a directory as a parent."
            };

        private static readonly IReadOnlyDictionary<FileSystemItemKind, string> validChildKindsMessage =
            new Dictionary<FileSystemItemKind, string>()
            {
                [FileSystemItemKind.Root] = "File system can contain volumes only as child items.",
                [FileSystemItemKind.Volume] = "Volume can contain directories and files only as child items.",
                [FileSystemItemKind.Directory] = "Directory can contain directories and files only as child items.",
                [FileSystemItemKind.File] = "File cannot contain child items."
            };

        private static IFileSystemItem CreateItemInternal(FileSystemItemKind kind, string name) => new FileSystemItem(
            kind,
            name,
            validParentKinds[kind],
            validChildKinds[kind],
            validParentKindsMessage[kind],
            validChildKindsMessage[kind]
        );

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

            IFileSystemItem root = CreateItemInternal(FileSystemItemKind.Root, name);

            IFileSystemItem defaultVolume = CreateVolume(PathUtils.Consts.ValidVolumeNames[0]);
            root.AddChild(defaultVolume);

            return root;
        }

        public static IFileSystemItem CreateVolume(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidVolumeName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid volume name."));

            return CreateItemInternal(FileSystemItemKind.Volume, name);
        }

        public static IFileSystemItem CreateDirectory(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidDirectoryName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid directory name."));

            return CreateItemInternal(FileSystemItemKind.Directory, name);
        }

        public static IFileSystemItem CreateFile(string name)
        {
            ValidateName(name);

            if (!PathUtils.IsValidFileName(name))
                throw new ArgumentException(Invariant($"'{name}' is not a valid file name."));

            return CreateItemInternal(FileSystemItemKind.File, name);
        }

    }

}
