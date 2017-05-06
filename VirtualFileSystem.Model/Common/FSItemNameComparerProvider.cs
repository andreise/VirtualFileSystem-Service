using Common.Comparers;
using System;
using VFSCommon;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item Name Comparer Provider
    /// </summary>
    internal static class FSItemNameComparerProvider
    {

        private static readonly Lazy<StringComparer> defaultInstance = new Lazy<StringComparer>(
            () => new StringItemComparer(name => name?.Trim(), StringComparer.OrdinalIgnoreCase)
        );

        /// <summary>
        /// File System Item Name Comparer
        /// </summary>
        public static StringComparer Default => defaultInstance.Value;

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
