using Ninject;
using System;
using Xunit;
using Tychaia.Globals;

namespace Tychaia.Globals.Tests.Dummies
{
    public class Foo : IFoo
    {
        private static Random m_Random = new Random();
        private string m_RandomID;

        public Foo()
        {
            this.m_RandomID = m_Random.Next().ToString();
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

