using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Disk.Tychaia
{
    public class TychaiaLevel : ILevel
    {
        // Store the octree in a file.  Basically you have a header for each
        // node that is just:
        //
        // 8 x <64-bit integer>
        //
        // with the integers indicating the position in the file for either
        // the next header or the actual file data, depending on the depth.
        //
        // TODO: Solve the issue of when chunks need a "larger" space (i.e
        // when they're storing temporary or dynamic data).  We could do this
        // by just storing the terrain data in the octree and putting entities
        // in a seperate file with a different structure.

        public void Save()
        {
        }

        public bool HasRegion(long x, long y, long z, long width, long height, long depth)
        {
            return false;
        }

        public int[] ProvideRegion(long x, long y, long z, long width, long height, long depth)
        {
            return null;
        }
    }
}
