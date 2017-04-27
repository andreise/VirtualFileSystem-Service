using System;

namespace VFSCommon
{

    public static class UserNameCompareRules
    {
        public static StringComparer Comparer => StringComparer.InvariantCultureIgnoreCase;
    }

}
