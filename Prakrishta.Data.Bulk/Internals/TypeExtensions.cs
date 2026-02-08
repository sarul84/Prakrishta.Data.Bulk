namespace Prakrishta.Data.Bulk.Internals
{
    public static class TypeExtensions
    {
        public static bool IsNullableValueType(this Type type)
        {
            return type.IsGenericType &&
                                type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullable(this Type type)
            => Nullable.GetUnderlyingType(type) != null;

        public static Type GetUnderlyingType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;

        public static bool IsSimple(this Type type)
        {
            type = type.GetUnderlyingType();
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime);
        }
    }
}
