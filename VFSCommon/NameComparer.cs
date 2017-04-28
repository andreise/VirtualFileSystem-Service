using System;

namespace VFSCommon
{

    public sealed class NameComparer : StringComparer
    {

        private readonly Func<string, string> normalizeName;

        private readonly StringComparer internalComparer;

        public NameComparer(Func<string, string> normalizeName = null, StringComparer internalComparer = null)
        {
            this.normalizeName = normalizeName ?? (name => name);
            this.internalComparer = internalComparer ?? Ordinal;
        }

        public override int Compare(string x, string y) => this.internalComparer.Compare(normalizeName(x), normalizeName(y));

        public override bool Equals(string x, string y) => this.internalComparer.Equals(normalizeName(x), normalizeName(y));

        public override int GetHashCode(string obj) => this.internalComparer.GetHashCode(normalizeName(obj));

    }

}
