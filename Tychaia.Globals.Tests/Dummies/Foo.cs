// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Globalization;

namespace Tychaia.Globals.Tests.Dummies
{
    public class Foo : IFoo
    {
        private static readonly Random m_Random = new Random();
        private readonly string m_RandomID;

        public Foo()
        {
            this.m_RandomID = m_Random.Next().ToString(CultureInfo.InvariantCulture);
        }

        public string DoSomething()
        {
            return "Foo";
        }

        public string GetRandomIdentifier()
        {
            return this.m_RandomID;
        }
    }
}