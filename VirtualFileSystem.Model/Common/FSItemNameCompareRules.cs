using System;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item Name Compare Rules
    /// </summary>
    internal static class FSItemNameCompareRules
    {

        /// <summary>
        /// Item Name Comparison
        /// </summary>
        public const StringComparison NameComparison = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// Determines whether two specified item names are equal
        /// </summary>
        /// <param name="name1">First Item Name</param>
        /// <param name="name2">Second Item Name</param>
        /// <returns>Returns true if two specified item names are equal, otherwise returns false</returns>
        public static bool EqualNames(string name1, string name2) => string.Equals(name1, name2, NameComparison);

        /// <summary>
        /// Compares two item names
        /// </summary>
        /// <param name="x">First Item Name</param>
        /// <param name="y">Second Item Name</param>
        /// <returns>
        /// Returns a positive value if the first item name is greater than the second item name,
        /// returns zero if the first item name and the second item name are equal,
        /// otherwise returns a negative value
        /// </returns>
        public static int CompareNames(string name1, string name2) => string.Compare(name1, name2, NameComparison);

        /// <summary>
        /// Normalizes Item Name Char
        /// </summary>
        /// <param name="name">Item Name Char</param>
        /// <returns>Returns a normalized item name char</returns>
        public static char NormalizeChar(char c) => char.ToUpperInvariant(c);

        /// <summary>
        /// Normalizes Item Name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <returns>Returns a normalized item name</returns>
        public static string NormalizeName(string name) => name?.ToUpperInvariant();

        /// <summary>
        /// Returns item name hash code
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <returns>Returns item name hash code</returns>
        public static int GetHashCode(string name) => NormalizeName(name)?.GetHashCode() ?? 0;

    }

}
