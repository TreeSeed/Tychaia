//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class TestForAttribute : Attribute
    {
        public Type Type { get; private set; }

        public TestForAttribute(Type type)
        {
            this.Type = type;
        }
    }
}

