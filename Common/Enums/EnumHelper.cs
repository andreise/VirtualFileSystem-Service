using System;

namespace Common.Enums
{

    public static class EnumHelper
    {

        public static bool IsEnum<TEnum>() where TEnum : struct
        {
            try
            {
                Enum.TryParse(null, out TEnum _);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct =>
            Enum.TryParse(value, ignoreCase, out TEnum result) ? result : (TEnum?)null;

    }

}
