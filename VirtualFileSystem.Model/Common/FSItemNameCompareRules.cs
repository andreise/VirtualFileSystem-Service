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
