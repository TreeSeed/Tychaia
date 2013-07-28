using Ninject;
using System;
using Xunit;
using Tychaia.Globals;
using Tychaia.Globals.Tests.Dummies;

namespace Tychaia.Globals.Tests
{
    public class IoCTests
    {
        [Fact]
        public void LibraryTest()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IFoo>().To<Foo>();
            var foo = kernel.Get<IFoo>();
            Assert.NotNull(foo);
            Assert.Equal("Foo", foo.DoSomething());
        }
    }
}

