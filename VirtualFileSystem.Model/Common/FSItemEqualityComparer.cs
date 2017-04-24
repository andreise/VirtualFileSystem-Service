﻿using System;
using System.Collections.Generic;
using static System.FormattableString;

namespace VirtualFileSystem.Model
{

    /// <summary>
    /// File System Item Equality Comparer
    /// </summary>
    internal sealed class FSItemEqualityComparer : IEqualityComparer<IFSItem>
    {

        public static bool EqualNames(string name1, string name2) => string.Equals(name1, name2, FSItemNameCompareRules.NameComparison);

        /// <summary>
        /// Compares two items
        /// </summary>
        /// <param name="x">First item</param>
        /// <param name="y">Second item</param>
        /// <returns>Returns true if items are equal, otherwise returns false</returns>
        public bool Equals(IFSItem x, IFSItem y)
        {
            if ((object)x == (object)y)
                return true;

            if ((object)x == null || (object)y == null)
                return false;

            return EqualNames(x.Name, y.Name);
        }

        /// <summary>
        /// Gets item hash code
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>Returns item hash code</returns>
        public int GetHashCode(IFSItem obj)
        {
            if ((object)obj == null)
                throw new ArgumentNullException(nameof(obj));

            if ((object)obj.Name == null)
                throw new ArgumentException(Invariant($"{nameof(obj.Name)} is null."), nameof(obj));

            return FSItemNameCompareRules.GetHashCode(obj.Name);
        }

        private static readonly Lazy<FSItemEqualityComparer> defaultInstance = new Lazy<FSItemEqualityComparer>(() => new FSItemEqualityComparer());

        /// <summary>
        /// Default instance
        /// </summary>
        public static FSItemEqualityComparer Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static FSItemEqualityComparer()
        {
        }

    }
}
