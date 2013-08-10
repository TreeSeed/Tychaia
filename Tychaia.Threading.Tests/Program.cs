// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
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