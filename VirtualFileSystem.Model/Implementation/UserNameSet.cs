using Common.Collections;
using System;
using VirtualFileSystem.Common;

namespace VirtualFileSystem.Model.Implementation
{

    internal sealed class UserNameSet : ItemSet<string>
    {

        public UserNameSet() : base(UserNameComparerProvider.Default)
        {
        }

        private static void ValidateItem(string item)
        {
            if (item is null)
                throw new ArgumentNullException(paramName: nameof(item), message: "User name is null.");

            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentException(paramName: nameof(item), message: "User name is empty.");
        }

        public override bool Add(string item)
        {
            ValidateItem(item);

            return base.Add(item);
        }

        public override bool Remove(string item)
        {
            ValidateItem(item);

            return base.Remove(item);
        }

    }

}
