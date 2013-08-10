// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.Disk
{
    public interface ILevel
    {
        void Save();

        bool HasRegion(long x, long y, long z, long width, long height, long depth);
        int[] ProvideRegion(long x, long y, long z, long width, long height, long depth);
    }
}