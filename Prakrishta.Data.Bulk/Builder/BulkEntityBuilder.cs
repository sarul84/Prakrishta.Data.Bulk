namespace Prakrishta.Data.Bulk.Builder;

using Prakrishta.Data.Bulk;
using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Internals;

public sealed class BulkEntityBuilder<T>
    where T : class
{
    private readonly BulkEngine _engine;
    private readonly IBulkEngineInternal _internal;
    private readonly IPartitionSwitchExecutor _switchExecutor;
    private readonly ISchemaResolver _schemaResolver;

    private string? _tableName;
    private string? _tvpName;
    private string? _procedureName;
    private string? _schema;

    public BulkEntityBuilder(BulkEngine engine)
        : this(
              engine, 
              (IBulkEngineInternal)engine, 
              new PartitionSwitchExecutor(),
              new SchemaResolver(engine.ConnectionFactory, engine.ConnectionString)) 
    { 
    }

    internal BulkEntityBuilder(
        BulkEngine engine,
        IBulkEngineInternal internalEngine,
        IPartitionSwitchExecutor switchExecutor,
        ISchemaResolver schemaResolver)
    {
        _engine = engine;
        _internal = internalEngine;
        _switchExecutor = switchExecutor;
        _schemaResolver = schemaResolver;
    }

    public BulkEntityBuilder<T> ToTable(string tableName)
    {
        _tableName = tableName;
        return this;
    }

    public BulkEntityBuilder<T> UsingTvp(string tvpName)
    {
        _tvpName = tvpName;
        return this;
    }

    public BulkEntityBuilder<T> UsingStoredProcedure(string procedureName)
    {
        _procedureName = procedureName;
        return this;
    }

    public BulkEntityBuilder<T> InSchema(string schema)
    {
        _schema = schema;
        return this;
    }

    private async Task<string> ResolveSchemaAsync(CancellationToken ct)
    {
        if (_schema != null)
            return _schema;

        if (_tableName?.Contains('.') == true)
            return _tableName.Split('.')[0];

        return await _schemaResolver.ResolveSchemaAsync(typeof(T).Name, ct);
    }

    private async Task<string> ResolveTableNameAsync(CancellationToken ct)
    {
        if (_tableName != null)
            return _tableName;

        var schema = await ResolveSchemaAsync(ct);
        return $"{schema}.{typeof(T).Name}";
    }

    private async Task<string> ResolveTvpNameAsync(CancellationToken ct)
    {
        if (_tvpName != null)
            return _tvpName;

        var schema = await ResolveSchemaAsync(ct);
        return $"{schema}.{typeof(T).Name}Type";
    }

    private async Task<string> ResolveProcedureNameAsync(string op, CancellationToken ct)
    {
        if (_procedureName != null)
            return _procedureName;

        var schema = await ResolveSchemaAsync(ct);
        return $"{schema}.{typeof(T).Name}_{op}";
    }


    public async Task<int> InsertAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
        var table = await ResolveTableNameAsync(ct);
        var tvp = await ResolveTvpNameAsync(ct);
        var proc = await ResolveProcedureNameAsync("Insert", ct);

        return await _internal.InsertAsync(items, table, tvp, proc, ct);
    }

    public async Task<int> UpdateAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
        var table = await ResolveTableNameAsync(ct);
        var tvp = await ResolveTvpNameAsync(ct);
        var proc = await ResolveProcedureNameAsync("Update", ct);

        return await _internal.UpdateAsync(items, table, tvp, proc, ct);
    }

    public async Task<int> DeleteAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
        var table = await ResolveTableNameAsync(ct);
        var tvp = await ResolveTvpNameAsync(ct);
        var proc = await ResolveProcedureNameAsync("Delete", ct);

        return await _internal.DeleteAsync(items, table, tvp, proc, ct);
    }

    public async Task<int> ReplacePartitionAsync(
    IEnumerable<T> items,
    Action<PartitionSwitchOptions> configure,
    CancellationToken ct = default)
    {
        var table = await ResolveTableNameAsync(ct);

        return await _switchExecutor.ReplacePartitionAsync(
            _engine,
            items,
            table,
            configure,
            ct);
    }

}