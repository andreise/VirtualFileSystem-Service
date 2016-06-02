using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace VirtualFileSystem.Model
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

        public static bool IsAbsolutePath(string path) => (object)path != null && path.Length >= 2 && IsValidVolumeName(path.Substring(0, 2));

        public static bool IsValidPathChar(char c) => InvalidPathChars.All(item => c != item);

        public static bool IsValidFileNameChar(char c) => InvalidFileNameChars.All(item => c != item);

        public static bool IsValidFileSystemName(string name) => (object)name != null && name.Length <= MaxFileSystemNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidDirectoryName(string name) => (object)name != null && name.Length <= MaxDirectoryNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidFileName(string name) => (object)name != null && name.Length <= MaxFileNameLength && name.All(c => IsValidFileNameChar(c));

        public static string CombinePath(string path1, string relativePath2)
        {
            if ((object)path1 == null)
                path1 = string.Empty;

            if ((object)relativePath2 == null)
                relativePath2 = string.Empty;

            relativePath2 = relativePath2.Trim().TrimStart(PathSeparator, AltPathSeparator);
            if (IsAbsolutePath(relativePath2))
                return relativePath2;

            path1 = path1.Trim().TrimEnd(PathSeparator, AltPathSeparator);

            if (path1.Length == 0 && relativePath2.Length == 0)
                return string.Empty;

            if (path1.Length == 0)
                return relativePath2;

            if (relativePath2.Length == 0)
                return path1;

            return path1 + new string(PathSeparator, 1) + relativePath2;
        }

        public static string[] SplitPath(string path)
        {
            if ((object)path == null)
                return new string[0];

            string[] tempItems = path.Split(new char[] { PathSeparator, AltPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            List<string> items = new List<string>(tempItems.Length);
            for (int i = 0; i < tempItems.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(tempItems[i]))
                    items.Add(tempItems[i]);
            }

            return items.ToArray();
        }

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
