using System;

namespace Tychaia.Threading.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var test = new TaskRegionTests();
            test.RegionIsProcessedFullyThreaded();
        }
    }
}

