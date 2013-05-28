using Ninject;
using System;
using Xunit;
using Tychaia.Globals;

namespace Tychaia.Globals.Tests.Dummies
{
    public class Bar : IBar
    {
        private IFoo m_Foo;

        public Bar(IFoo foo)
        {
            this.m_Foo = foo;
        }

        public IFoo GetFoo()
        {
            return this.m_Foo;
        }
    }
}

