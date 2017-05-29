using System;
using System.Reflection;

namespace Common.Enums
{

    public static class EnumHelper
    {

        public static bool IsEnum<TEnum>() where TEnum : struct => typeof(TEnum).GetTypeInfo().IsEnum;

        public static TEnum[] GetValues<TEnum>() where TEnum : struct => (TEnum[])Enum.GetValues(typeof(TEnum));

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct =>
            Enum.TryParse(value, ignoreCase, out TEnum result) ? result : (TEnum?)null;

        public static TEnum? TryParseSafe<TEnum>(string value, bool ignoreCase = false) where TEnum : struct =>
            IsEnum<TEnum>() ? TryParse<TEnum>(value, ignoreCase) : null;

    }

}
