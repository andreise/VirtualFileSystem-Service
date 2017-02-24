using System;

namespace VFSCommon
{

    public static class EnumHelper
    {

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                return null;

            TEnum result;

            if (!Enum.TryParse(value, ignoreCase, out result))
                return null;

            return result;
        }

    }

}
