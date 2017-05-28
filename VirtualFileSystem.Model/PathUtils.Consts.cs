using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace VirtualFileSystem.Model
{

    partial class PathUtils
    {

        /// <summary>
        /// File System Path Consts
        /// </summary>
        public static class Consts
        {

            // Item Name Lengths

            private const int MaxItemNameLength = 255;

            public const int MaxFileSystemNameLength = MaxItemNameLength;

            public const int MaxDirectoryNameLength = MaxItemNameLength;

            public const int MaxFileNameLength = MaxItemNameLength;

            // Path Separators

            public const char PathSeparator = '\\';

            public const char AltPathSeparator = '/';

            public static IReadOnlyList<char> PathSeparators { get; } = new ReadOnlyCollection<char>(new[] { PathSeparator, AltPathSeparator });

            // Volume Names

            public const char VolumeSeparator = ':';

            private static char[] GetValidVolumeChars(char firstChar, char lastChar) =>
                Enumerable.Range(firstChar, (lastChar - firstChar) + 1).Select(n => (char)n).ToArray();

            public static IReadOnlyList<char> ValidVolumeChars { get; } = new ReadOnlyCollection<char>(GetValidVolumeChars('C', 'Z'));

            public static IReadOnlyList<char> AltValidVolumeChars { get; } = new ReadOnlyCollection<char>(GetValidVolumeChars('c', 'z'));

            private static string[] GetValidVolumeNames(IReadOnlyList<char> validVolumeChars) =>
                validVolumeChars.Select(c => new string(new[] { c, VolumeSeparator })).ToArray();

            public static IReadOnlyList<string> ValidVolumeNames { get; } = new ReadOnlyCollection<string>(GetValidVolumeNames(ValidVolumeChars));

            public static IReadOnlyList<string> AltValidVolumeNames { get; } = new ReadOnlyCollection<string>(GetValidVolumeNames(AltValidVolumeChars));

            // Invalid Path/FileName Chars

            private static readonly Lazy<IReadOnlyList<char>> invalidPathChars = new Lazy<IReadOnlyList<char>>(
                () => new ReadOnlyCollection<char>(Path.GetInvalidPathChars())
            );

            public static IReadOnlyList<char> InvalidPathChars => invalidPathChars.Value;

            private static readonly Lazy<IReadOnlyList<char>> invalidFileNameChars = new Lazy<IReadOnlyList<char>>(
                () => new ReadOnlyCollection<char>(Path.GetInvalidFileNameChars())
            );

            public static IReadOnlyList<char> InvalidFileNameChars => invalidFileNameChars.Value;


            /// <summary>
            /// Static constructor
            /// </summary>
            /// <remarks>
            /// Needs for the guaranted static fields initialization in a multithreading work
            /// </remarks>
            static Consts()
            {
            }

        }

    }

}
