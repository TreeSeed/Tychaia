using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Disk
{
    public interface ILevel
    {
        void Save();

        bool HasRegion(long x, long y, long z, long width, long height, long depth);
        int[] ProvideRegion(long x, long y, long z, long width, long height, long depth);
    }
}
