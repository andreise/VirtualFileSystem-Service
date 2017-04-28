using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace VirtualFileSystem.Model
{

    partial class FSPath
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

            public static IReadOnlyList<char> PathSeparators { get; } = new ReadOnlyCollection<char>(new char[] { PathSeparator, AltPathSeparator });

            // Volume Names

            public const char VolumeSeparator = ':';

            private static char[] GetValidVolumeChars()
            {
                const char firstChar = 'C';
                const char lastChar = 'Z';

                char[] chars = new char[(lastChar - firstChar) + 1];

                chars[0] = firstChar;
                for (int i = 0; i < chars.Length - 1; i++)
                {
                    chars[i + 1] = chars[i];
                    chars[i + 1]++;
                }

                return chars;
            }

            public static IReadOnlyList<char> ValidVolumeChars { get; } = new ReadOnlyCollection<char>(GetValidVolumeChars());

            private static string[] GetValidVolumeNames()
            {
                string[] names = new string[ValidVolumeChars.Count];

                for (int i = 0; i < names.Length; i++)
                    names[i] = new string(new char[] { ValidVolumeChars[i], VolumeSeparator });

                return names;
            }

            public static IReadOnlyList<string> ValidVolumeNames { get; } = new ReadOnlyCollection<string>(GetValidVolumeNames());

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
