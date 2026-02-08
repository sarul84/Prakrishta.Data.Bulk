using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public sealed class BenchmarkHarness : IAsyncDisposable
{
    public string DatabaseFilePath { get; }
    public string DatabaseName { get; }
    public string ConnectionString { get; }

    public BenchmarkHarness()
    {
        var sourceMdf = ResolveMdfPath();

        var tempDir = Path.Combine(
            Path.GetTempPath(),
            "PrakrishtaBulkBench",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(tempDir);

        DatabaseFilePath = Path.Combine(tempDir, "PrakrishtaBulkTestDb.mdf");
        File.Copy(sourceMdf, DatabaseFilePath, overwrite: true);

        DatabaseName = "PrakrishtaBulkBench_" + Guid.NewGuid().ToString("N");
        DetachIfExists(DatabaseName);
        ConnectionString = BuildConnectionString(DatabaseFilePath);

        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
    }

    private static void DetachIfExists(string dbFile)
    {
        var escaped = dbFile.Replace("'", "''");

        using var conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            DECLARE @db sysname;

            SELECT @db = DB_NAME(database_id)
            FROM sys.master_files
            WHERE physical_name = '{escaped}';

            IF @db IS NOT NULL
            BEGIN
                DECLARE @sql NVARCHAR(MAX) = 
                    'ALTER DATABASE [' + @db + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; ' +
                    'DROP DATABASE [' + @db + '];';

                EXEC(@sql);
            END";

        cmd.ExecuteNonQuery();
    }


    private static string BuildConnectionString(string dbFile)
    {
        return $@"
        Server=(localdb)\MSSQLLocalDB;
        Integrated Security=true;
        AttachDbFilename={dbFile};
        Trusted_Connection=Yes;";
    }

    public async Task CreateTableAsync(string tableName)
    {
        using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            IF OBJECT_ID('{tableName}', 'U') IS NOT NULL DROP TABLE {tableName};
            CREATE TABLE {tableName} (
                Id INT PRIMARY KEY,
                Name NVARCHAR(200),
                Amount DECIMAL(18,2),
                CreatedOn DATETIME2,
                OptionalValue INT NULL
            );";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SeedAsync(string tableName, int count)
    {
        using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        for (int i = 1; i <= count; i += 1000)
        {
            using var cmd = conn.CreateCommand();
            var end = Math.Min(i + 999, count);

            var values = new StringBuilder();
            for (int j = i; j <= end; j++)
            {
                if (values.Length > 0) values.Append(',');
                values.Append($"({j}, 'Name {j}', {j * 1.5m}, SYSUTCDATETIME(), {(j % 2 == 0 ? j.ToString() : "NULL")})");
            }

            cmd.CommandText = $"INSERT INTO {tableName} VALUES {values}";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task ExecuteNonQueryAsync(string sql)
    {
        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql)
    {
        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return default!;

        return (T)Convert.ChangeType(result, typeof(T));
    }

    //public async Task EnsureTvpInfrastructureAsync(string tableName)
    //{
    //    await ExecuteNonQueryAsync(@"
    //        IF TYPE_ID('dbo.TestEntityType') IS NULL
    //        CREATE TYPE dbo.TestEntityType AS TABLE
    //        (
    //            Id INT,
    //            Name NVARCHAR(200),
    //            Amount DECIMAL(18,2),
    //            CreatedOn DATETIME2,
    //            OptionalValue INT NULL
    //        );
    //    ");

    //    await ExecuteNonQueryAsync($@"
    //        IF OBJECT_ID('dbo.UpdateTestEntities_TVP', 'P') IS NULL
    //        EXEC('
    //            CREATE PROCEDURE dbo.UpdateTestEntities_TVP
    //                @Items dbo.TestEntityType READONLY
    //            AS
    //            BEGIN
    //                UPDATE T
    //                SET 
    //                    T.Name = I.Name,
    //                    T.Amount = I.Amount,
    //                    T.CreatedOn = I.CreatedOn,
    //                    T.OptionalValue = I.OptionalValue
    //                FROM {tableName} T
    //                JOIN @Items I ON T.Id = I.Id;
    //            END
    //        ');
    //    ");

    //    await ExecuteNonQueryAsync($@"
    //        IF OBJECT_ID('dbo.DeleteTestEntities_TVP', 'P') IS NULL
    //        EXEC('
    //            CREATE PROCEDURE dbo.DeleteTestEntities_TVP
    //                @Items dbo.TestEntityType READONLY
    //            AS
    //            BEGIN
    //                DELETE T
    //                FROM {tableName} T
    //                JOIN @Items I ON T.Id = I.Id;
    //            END
    //        ');
    //    ");

    //    await ExecuteNonQueryAsync($@"
    //        IF OBJECT_ID('dbo.InsertTestEntities_TVP', 'P') IS NULL
    //        EXEC('
    //            CREATE PROCEDURE dbo.InsertTestEntities_TVP
    //                @Items dbo.TestEntityType READONLY
    //            AS
    //            BEGIN
    //                INSERT INTO {tableName} (Id, Name, Amount, CreatedOn, OptionalValue)
    //                SELECT Id, Name, Amount, CreatedOn, OptionalValue
    //                FROM @Items;
    //            END
    //        ');
    //    ");
    //}

    public async Task EnsureTvpInfrastructureAsync()
    {
        await DropAndCreateTypeAsync();
        await DropAndCreateInsertProcAsync();
        await DropAndCreateUpdateProcAsync();
        await DropAndCreateDeleteProcAsync();
    }

    private async Task DropAndCreateTypeAsync()
    {
        const string sql = @"
            IF TYPE_ID('dbo.TestEntityType') IS NOT NULL
                DROP TYPE dbo.TestEntityType;
            GO
            CREATE TYPE dbo.TestEntityType AS TABLE
            (
                Id            INT            NOT NULL,
                Name          NVARCHAR(200)  NOT NULL,
                Amount        DECIMAL(18, 2) NOT NULL,
                CreatedOn     DATETIME2      NOT NULL,
                OptionalValue INT            NULL
            );";

        await ExecuteBatchAsync(sql);
    }

    private async Task DropAndCreateInsertProcAsync()
    {
        const string sql = @"
            IF OBJECT_ID('dbo.InsertTestEntities_TVP', 'P') IS NOT NULL
                DROP PROCEDURE dbo.InsertTestEntities_TVP;
            GO
            CREATE PROCEDURE dbo.InsertTestEntities_TVP
                @Items dbo.TestEntityType READONLY
            AS
            BEGIN
                SET NOCOUNT ON;

                INSERT INTO dbo.TestEntities
                (
                    Id,
                    Name,
                    Amount,
                    CreatedOn,
                    OptionalValue
                )
                SELECT
                    Id,
                    Name,
                    Amount,
                    CreatedOn,
                    OptionalValue
                FROM @Items;
            END";

        await ExecuteBatchAsync(sql);
    }

    private async Task DropAndCreateUpdateProcAsync()
    {
        const string sql = @"
            IF OBJECT_ID('dbo.UpdateTestEntities_TVP', 'P') IS NOT NULL
                DROP PROCEDURE dbo.UpdateTestEntities_TVP;
            GO
            CREATE PROCEDURE dbo.UpdateTestEntities_TVP
                @Items dbo.TestEntityType READONLY
            AS
            BEGIN
                SET NOCOUNT ON;

                UPDATE T
                SET
                    T.Name          = I.Name,
                    T.Amount        = I.Amount,
                    T.CreatedOn     = I.CreatedOn,
                    T.OptionalValue = I.OptionalValue
                FROM dbo.TestEntities T
                JOIN @Items I ON T.Id = I.Id;
            END";

        await ExecuteBatchAsync(sql);
    }

    private async Task DropAndCreateDeleteProcAsync()
    {
        const string sql = @"
            IF OBJECT_ID('dbo.DeleteTestEntities_TVP', 'P') IS NOT NULL
                DROP PROCEDURE dbo.DeleteTestEntities_TVP;
            GO
            CREATE PROCEDURE dbo.DeleteTestEntities_TVP
                @Items dbo.TestEntityType READONLY
            AS
            BEGIN
                SET NOCOUNT ON;

                DELETE T
                FROM dbo.TestEntities T
                JOIN @Items I ON T.Id = I.Id;
            END";

        await ExecuteBatchAsync(sql);
    }

    private async Task ExecuteBatchAsync(string sql)
    {
        var batches = Regex.Split(sql, @"^\s*GO\s*$",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (trimmed.Length == 0)
                continue;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = trimmed;
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static string ResolveMdfPath()
    {
        var dir = AppContext.BaseDirectory;

        while (dir != null)
        {
            var candidate = Path.Combine(dir, "DataFiles", "PrakrishtaBulkTestDb.mdf");
            if (File.Exists(candidate))
                return candidate;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new FileNotFoundException("Could not locate PrakrishtaBulkTestDb.mdf in any parent directory.");
    }

    public ValueTask DisposeAsync()
    {
        try
        {
            var dir = Path.GetDirectoryName(DatabaseFilePath);
            if (dir != null && Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
        catch { }

        return ValueTask.CompletedTask;
    }
}