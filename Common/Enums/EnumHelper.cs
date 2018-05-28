using System;
using System.Reflection;

namespace Common.Enums
{

    public static class EnumHelper
    {

        public static bool IsEnum<TEnum>() where TEnum : struct, Enum => typeof(TEnum).GetTypeInfo().IsEnum;

        public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum => (TEnum[])Enum.GetValues(typeof(TEnum));

        public static string[] GetNames<TEnum>() where TEnum : struct, Enum => Enum.GetNames(typeof(TEnum));

        public static string GetName<TEnum>(object value) where TEnum : struct, Enum => Enum.GetName(typeof(TEnum), value);

        public static TEnum? TryParse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct, Enum =>
            Enum.TryParse(value, ignoreCase, out TEnum result) ? result : (TEnum?)null;

    }

}
