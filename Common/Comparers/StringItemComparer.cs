using System;

namespace Common.Comparers
{

    public sealed class StringItemComparer : StringComparer
    {

        private readonly Func<string, string> normalizeItem;

        private readonly StringComparer internalComparer;

        public StringItemComparer(Func<string, string> normalizeItem = null, StringComparer internalComparer = null)
        {
            this.normalizeItem = normalizeItem ?? (name => name);
            this.internalComparer = internalComparer ?? Ordinal;
        }

        public override int Compare(string x, string y) => this.internalComparer.Compare(normalizeItem(x), normalizeItem(y));

        public override bool Equals(string x, string y) => this.internalComparer.Equals(normalizeItem(x), normalizeItem(y));

        public override int GetHashCode(string obj) => this.internalComparer.GetHashCode(normalizeItem(obj));

    }

}
