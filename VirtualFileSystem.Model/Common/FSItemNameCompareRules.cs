using System;

namespace VFSClient.Model
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

    }

}
