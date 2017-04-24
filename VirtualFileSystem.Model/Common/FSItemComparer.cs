using System;
using System.Collections.Generic;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item Comparer
    /// </summary>
    internal sealed class FSItemComparer : IComparer<IFSItem>
    {

        /// <summary>
        /// Compares two items
        /// </summary>
        /// <param name="x">First item</param>
        /// <param name="y">Second item</param>
        /// <returns>
        /// Returns a positive value if the first item is greater than the second item,
        /// returns zero if the first item and the second item are equal,
        /// otherwise returns a negative value
        /// </returns>
        public int Compare(IFSItem x, IFSItem y)
        {
            if ((object)x == (object)y)
                return 0;

            if ((object)x == null)
                return -1;

            if ((object)y == null)
                return 1;

            return FSItemNameCompareRules.CompareNames(x.Name, y.Name);
        }

        private static readonly Lazy<FSItemComparer> defaultInstance = new Lazy<FSItemComparer>(() => new FSItemComparer());

        /// <summary>
        /// Default Instance
        /// </summary>
        public static FSItemComparer Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FSItemComparer()
        {
        }

    }

}
