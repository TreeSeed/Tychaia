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

        [Fact]
        public void BindTest()
        {
            IoC.ReplaceKernel(new StandardKernel());
            Assert.NotNull(IoC.Kernel);
            IoC.Kernel.Bind<IFoo>().To<Foo>();
            var foo = IoC.Kernel.Get<IFoo>();
            Assert.NotNull(foo);
            Assert.Equal("Foo", foo.DoSomething());
        }

        [Fact]
        public void NonSingletonTest()
        {
            IoC.ReplaceKernel(new StandardKernel());
            Assert.NotNull(IoC.Kernel);
            IoC.Kernel.Bind<IFoo>().To<Foo>();
            var foo1 = IoC.Kernel.Get<IFoo>();
            var foo2 = IoC.Kernel.Get<IFoo>();
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.NotEqual(foo1.GetRandomIdentifier(), foo2.GetRandomIdentifier());
        }

        [Fact]
        public void SingletonTest()
        {
            IoC.ReplaceKernel(new StandardKernel());
            Assert.NotNull(IoC.Kernel);
            IoC.Kernel.Bind<IFoo>().To<Foo>().InSingletonScope();
            var foo1 = IoC.Kernel.Get<IFoo>();
            var foo2 = IoC.Kernel.Get<IFoo>();
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.Equal(foo1.GetRandomIdentifier(), foo2.GetRandomIdentifier());
        }

        [Fact]
        public void DependencyTest()
        {
            IoC.ReplaceKernel(new StandardKernel());
            Assert.NotNull(IoC.Kernel);
            IoC.Kernel.Bind<IFoo>().To<Foo>();
            IoC.Kernel.Bind<IBar>().To<Bar>();
            var bar = IoC.Kernel.Get<IBar>();
            Assert.NotNull(bar);
            Assert.NotNull(bar.GetFoo());
            Assert.Equal("Foo", bar.GetFoo().DoSomething());
        }
    }
}

