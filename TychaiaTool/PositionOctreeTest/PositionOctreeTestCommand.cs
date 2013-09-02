// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using ManyConsole;
using Protogame;
using TychaiaTool.PositionOctreeTest;

namespace TychaiaTool
{
    public class PositionOctreeTestCommand : ConsoleCommand
    {
        public PositionOctreeTestCommand()
        {
            this.IsCommand("test-position-octree", "Verify that the position octree is working correctly");
        }

        public override int Run(string[] remainingArguments)
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

            return 0;
        }
    }
}

