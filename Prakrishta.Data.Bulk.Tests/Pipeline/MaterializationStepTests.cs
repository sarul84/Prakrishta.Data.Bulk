using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Pipeline.Steps;

namespace Prakrishta.Data.Bulk.Tests.Pipeline
{
    [TestClass]
    public class MaterializationStepTests
    {
        [TestMethod]
        public async Task Should_Materialize_List_And_ColumnMaps()
        {
            var step = new MaterializationStep();

            var ctx = new BulkContext
            {
                EntityType = typeof(TestEntity),
                Items = new[]
                {
                new TestEntity { Id = 1, Name = "A" }
            }
            };

            await step.ExecuteAsync(ctx, CancellationToken.None);

            ctx.Properties["List"].Should().BeOfType<List<object>>();
            ctx.Properties["ColumnMaps"].Should().NotBeNull();
        }

        private sealed class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
