using System;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item Name Comparer Provider
    /// </summary>
    internal static class FSItemNameComparerProvider
    {

        private sealed class FSItemNameComparer : StringComparer
        {

            private static StringComparer InternalComparer => InvariantCultureIgnoreCase;

            private static string NormalizeName(string name) => name?.Trim();

            public override int Compare(string x, string y) => InternalComparer.Compare(NormalizeName(x), NormalizeName(y));

            public override bool Equals(string x, string y) => InternalComparer.Equals(NormalizeName(x), NormalizeName(y));

            public override int GetHashCode(string obj) => InternalComparer.GetHashCode(NormalizeName(obj));

        }

        private static readonly Lazy<StringComparer> defaultInstance = new Lazy<StringComparer>(() => new FSItemNameComparer());

        /// <summary>
        /// File System Item Name Comparer
        /// </summary>
        public static StringComparer Comparer => defaultInstance.Value;

        /// <summary>
        /// Normalizes Item Name Char
        /// </summary>
        /// <param name="name">Item Name Char</param>
        /// <returns>Returns a normalized item name char</returns>
        public static char NormalizeChar(char c) => char.ToUpperInvariant(c);

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FSItemNameComparerProvider()
        {
        }

    }

}
