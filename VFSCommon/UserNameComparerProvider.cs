using Common.Comparers;
using System;

namespace VFSCommon
{

    /// <summary>
    /// User Name Comparer Provider
    /// </summary>
    public static class UserNameComparerProvider
    {

        private static readonly Lazy<StringComparer> defaultInstance = new Lazy<StringComparer>(
            () => new StringItemComparer(name => name?.Trim(), StringComparer.OrdinalIgnoreCase)
        );

        /// <summary>
        /// User Name Comparer
        /// </summary>
        public static StringComparer Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static UserNameComparerProvider()
        {
        }

    }

}
