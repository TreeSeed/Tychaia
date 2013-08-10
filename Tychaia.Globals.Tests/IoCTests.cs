// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Ninject;
using Tychaia.Globals.Tests.Dummies;
using Xunit;

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