using System;
using System.Collections.Generic;

public sealed class TestEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? OptionalValue { get; set; }
}

public static class BenchmarkDataFactory
{
    public static List<TestEntity> Create(int count)
    {
        var list = new List<TestEntity>(count);

        for (int i = 1; i <= count; i++)
        {
            list.Add(new TestEntity
            {
                Id = i,
                Name = "Name " + i,
                Amount = i * 1.5m,
                CreatedOn = DateTime.UtcNow,
                OptionalValue = (i % 2 == 0 ? i : (int?)null)
            });
        }

        return list;
    }
}