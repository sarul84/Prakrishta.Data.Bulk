namespace Prakrishta.Data.Bulk.Mapping
{
    public sealed class ColumnMapOld(string name, Type type, Func<object, object?> getter, Action<object, object?> setter)
    {
        public string Name { get; } = name;
        public Type Type { get; } = type;
        public Func<object, object?> Getter { get; } = getter;
        public Action<object, object?> Setter { get; } = setter;
    }

    public sealed class ColumnMap
    {
        public string ColumnName { get; init; } = default!;
        public Func<object, object?> Getter { get; init; } = default!;
        public Type ColumnType { get; init; } = default!;
        public bool IsKey { get; init; }
    }
}
