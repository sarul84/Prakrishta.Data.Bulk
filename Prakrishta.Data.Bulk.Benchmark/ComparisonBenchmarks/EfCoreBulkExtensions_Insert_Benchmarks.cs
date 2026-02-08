using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;

public class EfCoreBulkExtensions_Insert_Benchmarks : BulkBenchmarkBase
{
    private TestDbContext _context = null!;

    protected override Task OnGlobalSetupAsync()
    {
        _context = new TestDbContext(Harness.ConnectionString);
        return Task.CompletedTask;
    }

    [Benchmark]
    public async Task EFCore_BulkInsert()
    {
        await _context.BulkInsertAsync(Data);
    }

    protected override Task OnGlobalCleanupAsync()
    {
        _context.Dispose();
        return Task.CompletedTask;
    }
}

public sealed class TestDbContext : DbContext
{
    private readonly string _connectionString;

    public TestDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(b =>
        {
            b.ToTable("TestEntities", "dbo");
            b.HasKey(x => x.Id);
        });
    }
}