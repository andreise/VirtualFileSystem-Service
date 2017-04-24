using System;

namespace VFSCommon
{

    public static class EnumHelper
    {

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct =>
            typeof(TEnum).IsEnum && Enum.TryParse(value, ignoreCase, out TEnum result) ? result : (TEnum?)null;

    }

}
