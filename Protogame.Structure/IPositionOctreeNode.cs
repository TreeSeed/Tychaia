using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame.Structure
{
    public interface IPositionOctreeNode
    {
        long X { get; }
        long Y { get; }
        long Z { get; }
    }
}
