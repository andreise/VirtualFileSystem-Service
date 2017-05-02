using System;
using System.Collections;
using System.Collections.Generic;
using VFSCommon;

namespace VirtualFileSystem.Model
{

    internal sealed class FSUserNames : IReadOnlyCollection<string>
    {

        private readonly HashSet<string> items = new HashSet<string>(UserNameComparerProvider.Default);

        public int Count => this.items.Count;

        private IEnumerator<string> GetEnumeratorInternal() => ((IReadOnlyCollection<string>)this.items).GetEnumerator();

        public IEnumerator<string> GetEnumerator() => GetEnumeratorInternal();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private static void ValidateItem(string item)
        {
            if (item is null)
                throw new ArgumentNullException(paramName: nameof(item), message: "User name is null.");

            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentException(paramName: nameof(item), message: "User name is empty.");
        }

        public bool Add(string item)
        {
            ValidateItem(item);
            return this.items.Add(item);
        }

        public bool Remove(string item)
        {
            ValidateItem(item);
            return this.items.Remove(item);
        }

    }

}
