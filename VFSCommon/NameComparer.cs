using System;

namespace VFSCommon
{

    public sealed class NameComparer : StringComparer
    {

        private readonly StringComparer internalComparer;

        public NameComparer(StringComparer internalComparer = null) => this.internalComparer = internalComparer ?? Ordinal;

        private static string NormalizeName(string name) => name?.Trim();

        public override int Compare(string x, string y) => this.internalComparer.Compare(NormalizeName(x), NormalizeName(y));

        public override bool Equals(string x, string y) => this.internalComparer.Equals(NormalizeName(x), NormalizeName(y));

        public override int GetHashCode(string obj) => this.internalComparer.GetHashCode(NormalizeName(obj));

    }

}
