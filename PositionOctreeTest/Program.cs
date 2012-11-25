using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.Structure;

namespace PositionOctreeTest
{
    class Value
    {
        public Value(int v)
        {
            this.Blah = v;
        }

        public int Blah;
    }

    class Program
    {
        static void Main(string[] args)
        {
            PositionOctree<Value> octree = new PositionOctree<Value>();
            octree.Insert(new Value(5), 1, 1, 1);
            octree.Insert(new Value(24), 1000000, 1000000, 1000000);
            octree.Insert(new Value(534), 1000000000, 1000000000, 1000000000);
            octree.Insert(new Value(-5), -1, -1, -1);
            octree.Insert(new Value(-24), -1000000, -1000000, -1000000);
            octree.Insert(new Value(-534), -1000000000, -1000000000, -1000000000);
            Value v = octree.Find(1, 1, 1);
            Console.WriteLine(v.Blah);
            v = octree.Find(1000000, 1000000, 1000000);
            Console.WriteLine(v.Blah);
            v = octree.Find(1000000000, 1000000000, 1000000000);
            Console.WriteLine(v.Blah);
            v = octree.Find(-1, -1, -1);
            Console.WriteLine(v.Blah);
            v = octree.Find(-1000000, -1000000, -1000000);
            Console.WriteLine(v.Blah);
            v = octree.Find(-1000000000, -1000000000, -1000000000);
            Console.WriteLine(v.Blah);
            Console.ReadKey();
        }
    }
}
