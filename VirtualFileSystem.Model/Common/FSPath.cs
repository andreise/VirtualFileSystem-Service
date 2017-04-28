using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Path Utils
    /// </summary>
    internal static partial class FSPath
    {

        // Path Separator Validators

        public static bool IsPathSeparator(char c) => Consts.PathSeparators.Contains(c);

        // Volume Name Validators

        public static bool IsVolumeSeparator(char c) => c == Consts.VolumeSeparator;

        public static bool IsValidVolumeChar(char c) => (c = char.ToUpperInvariant(c)) >= Consts.ValidVolumeChars[0] && c <= Consts.ValidVolumeChars[Consts.ValidVolumeChars.Count - 1];

        public static bool IsValidVolumeName(string name) => !(name is null) && name.Length == 2 && IsValidVolumeChar(name[0]) && IsVolumeSeparator(name[1]);

        // Other Path Validators

        public static bool IsAbsolutePath(string path) => !(path is null) && path.Length >= 2 && IsValidVolumeName(path.Substring(0, 2));

        public static bool IsValidPathChar(char c) => Consts.InvalidPathChars.All(item => c != item);

        public static bool IsValidFileNameChar(char c) => Consts.InvalidFileNameChars.All(item => c != item);

        public static bool IsValidFileSystemName(string name) => !(name is null) && name.Length <= Consts.MaxFileSystemNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidDirectoryName(string name) => !(name is null) && name.Length <= Consts.MaxDirectoryNameLength && name.All(c => IsValidPathChar(c));

        public static bool IsValidFileName(string name) => !(name is null) && name.Length <= Consts.MaxFileNameLength && name.All(c => IsValidFileNameChar(c));

        // Path Combining/Splitting

        public static string CombinePath(string path1, string relativePath2)
        {
            if (path1 is null)
                path1 = string.Empty;

            if (relativePath2 is null)
                relativePath2 = string.Empty;

            relativePath2 = relativePath2.Trim().TrimStart(Consts.PathSeparators.ToArray());
            if (IsAbsolutePath(relativePath2))
                return relativePath2;

            path1 = path1.Trim().TrimEnd(Consts.PathSeparators.ToArray());

            if (path1.Length == 0 && relativePath2.Length == 0)
                return string.Empty;

            if (path1.Length == 0)
                return relativePath2;

            if (relativePath2.Length == 0)
                return path1;

            return path1 + new string(Consts.PathSeparator, 1) + relativePath2;
        }

        public static string[] SplitPath(string path)
        {
            if (path is null)
                return new string[0];

            string[] tempItems = path.Split(Consts.PathSeparators.ToArray(), StringSplitOptions.RemoveEmptyEntries);

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
