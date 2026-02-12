using Prakrishta.Data.Bulk.Attributes;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Helpers;
using System.Collections.Concurrent;
using System.Reflection;

namespace Prakrishta.Data.Bulk.Mapping;

public static class ColumnMapCacheOld
{
    private static readonly ConcurrentDictionary<Type, ColumnMapOld[]> _cache = new();

    public static ColumnMapOld[] Get(Type type) => _cache.GetOrAdd(type, Build);

    public static ColumnMapOld[] Get<T>() => Get(typeof(T));

    private static ColumnMapOld[] Build(Type type)
    {
        var props = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        var maps = new List<ColumnMapOld>(props.Length);

        foreach (var prop in props)
        {
            var getter = IlGetterFactory.CreateGetter(prop);
            var setter = IlSetterFactory.CreateSetter(prop);
            maps.Add(new ColumnMapOld(prop.Name, prop.PropertyType, getter, setter));
        }

        return maps.ToArray();
    }    
}

public static class ColumnMapCache
{
    private static readonly ConcurrentDictionary<Type, ColumnMap[]> _cache = new();

    public static ColumnMap[] Get(Type type) => _cache.GetOrAdd(type, Build);

    private static ColumnMap[] Build(Type type)
    {
        var props = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        var maps = new List<ColumnMap>(props.Length);

        foreach (var prop in props)
        {
            // 1. Ignore attribute
            if (prop.GetCustomAttribute<BulkIgnoreAttribute>() != null)
                continue;

            // 2. Column name override
            var colAttr = prop.GetCustomAttribute<BulkColumnAttribute>();
            var columnName = colAttr?.ColumnName ?? prop.Name;

            // 3. Key override
            var isKey =
                prop.GetCustomAttribute<BulkKeyAttribute>() != null ||
                prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);

            // 4. Getter
            var getter = IlGetterFactory.CreateGetter(prop);

            maps.Add(new ColumnMap
            {
                ColumnName = prop.Name,
                ColumnType = prop.PropertyType,
                Getter = getter,
                IsKey = isKey
            });
        }

        return maps.ToArray();
    }
}