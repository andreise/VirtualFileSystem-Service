using System;

namespace Common.Enums
{

    public static class EnumHelper
    {

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct =>
            Enum.TryParse(value, ignoreCase, out TEnum result) ? result : (TEnum?)null;

    }

}
