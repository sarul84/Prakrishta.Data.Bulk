using System.Collections.Concurrent;
using System.Reflection;

namespace Prakrishta.Data.Bulk.Internals
{
    public static class ReflectionUtils
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _cache = new();

        public static PropertyInfo[] GetProperties(Type type)
        {
            return _cache.GetOrAdd(type, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        public static object? GetValue(object instance, PropertyInfo property)
        {
            return property.GetValue(instance);
        }
    }
}
