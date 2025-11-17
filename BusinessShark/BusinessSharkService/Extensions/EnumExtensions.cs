using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? fi = value.GetType().GetField(value.ToString());
        if (fi == null) return string.Empty;

        var attr = fi.GetCustomAttribute<DescriptionAttribute>();
        return attr == null || attr.Description == null ? value.ToString() : attr.Description;
    }
}