using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace VFSClient.Model
{

    /// <summary>
    /// File System Path Utils
    /// </summary>
    internal static class FSPath
    {

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

        private static string[] GetValidVolumeNames()
        {
            string[] names = new string[ValidVolumeChars.Count];

            for (int i = 0; i < names.Length; i++)
                names[i] = new string(new char[] { ValidVolumeChars[i], VolumeSeparator });

            return names;
        }

        private static readonly Lazy<IReadOnlyList<char>> validVolumeChars = new Lazy<IReadOnlyList<char>>(() => new ReadOnlyCollection<char>(GetValidVolumeChars()));

        private static readonly Lazy<IReadOnlyList<string>> validVolumeNames = new Lazy<IReadOnlyList<string>>(() => new ReadOnlyCollection<string>(GetValidVolumeNames()));

        private static readonly Lazy<IReadOnlyList<char>> invalidPathChars = new Lazy<IReadOnlyList<char>>(() => new ReadOnlyCollection<char>(Path.GetInvalidPathChars()));

        private static readonly Lazy<IReadOnlyList<char>> invalidFileNameChars = new Lazy<IReadOnlyList<char>>(() => new ReadOnlyCollection<char>(Path.GetInvalidFileNameChars()));

        public static IReadOnlyList<string> ValidVolumeNames => validVolumeNames.Value;

        public static IReadOnlyList<char> ValidVolumeChars => validVolumeChars.Value;

        public static IReadOnlyList<char> InvalidPathChars => invalidPathChars.Value;

        public static IReadOnlyList<char> InvalidFileNameChars => invalidFileNameChars.Value;

        public const char VolumeSeparator = ':';

        public const char PathSeparator = '\\';

        public const char AltPathSeparator = '/';

        public const int MaxFileSystemNameLength = 255;

        public const int MaxDirectoryNameLength = 255;

        public const int MaxFileNameLength = 255;

        public static bool IsVolumeSeparator(char c) => c == VolumeSeparator;

        public static bool IsPathSeparator(char c) => c == PathSeparator || c == AltPathSeparator;

        public static bool IsValidVolumeChar(char c) => (c = FSItemNameCompareRules.NormalizeChar(c)) >= ValidVolumeChars[0] && c <= ValidVolumeChars[ValidVolumeChars.Count - 1];

        public static bool IsValidVolumeName(string name) => (object)name != null && name.Length == 2 && IsValidVolumeChar(name[0]) && IsVolumeSeparator(name[1]);

        public static bool IsValidPathChar(char c) => InvalidPathChars.All(item => c != item);

        public static bool IsValidFileNameChar(char c) => InvalidFileNameChars.All(item => c != item);

        public static bool IsValidFileSystemName(string name) => (object)name != null && name.Length <= MaxFileSystemNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidDirectoryName(string name) => (object)name != null && name.Length <= MaxDirectoryNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidFileName(string name) => (object)name != null && name.Length <= MaxFileNameLength && name.All(c => IsValidFileNameChar(c));

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FSPath()
        {
        }

    }

}
