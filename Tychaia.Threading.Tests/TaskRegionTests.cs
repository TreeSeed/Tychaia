using System;
using Xunit;
using System.Threading;

namespace Tychaia.Threading.Tests
{
    public class TaskRegionTests
    {
        private class LongProcessor : IRegionProcessor<long?>
        {
            public int TotalProcessed = 0;

            public void Process(TaskRegionEntry<long?> entry)
            {
                entry.Result = entry.X + entry.Y + entry.Z;
                TotalProcessed += 1;
            }
        }

        [Fact]
        public void RegionIsProcessedFullyInline()
        {
            var region = new TaskRegion<LongProcessor, long?>(false);
            region.ComputeRegion(0, 0, 0, 10, 10, 10);

            // For a region of 10x10x10 using inline processing, we should
            // have to call Process() exactly 1000 times.
            for (var i = 0; i < 1000; i++)
                region.Process();

            // Now verify all of the values.
            for (var x = -5; x < 5; x++)
                for (var y = -5; y < 5; y++)
                    for (var z = -5; z < 5; z++)
                    {
                        var value = region[x, y, z];
                        Assert.Equal(value, x + y + z);
                    }
        }

        [Fact]
        public void RegionIsProcessedFullyThreaded()
        {
            var region = new TaskRegion<LongProcessor, long?>(true);
            region.ComputeRegion(0, 0, 0, 10, 10, 10);

            // For a threaded region, we need to call Process() at least
            // once every 5 seconds to keep processing alive.
            var start = DateTime.Now;
            while ((region.Processor == null ||
                    region.Processor.TotalProcessed < 1000) &&
                   (DateTime.Now - start).TotalSeconds < 10)
                region.Process();
            if ((DateTime.Now - start).TotalSeconds >= 10)
                Assert.False(true, "Processing took longer than 10 seconds.");

            // Now verify all of the values.
            for (var x = -5; x < 5; x++)
                for (var y = -5; y < 5; y++)
                    for (var z = -5; z < 5; z++)
                {
                    var value = region[x, y, z];
                    Assert.Equal(value, x + y + z);
                }
        }
    }
}

