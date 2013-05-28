using Ninject;
using System;
using Xunit;
using Tychaia.Globals;

namespace Tychaia.Globals.Tests.Dummies
{
    public interface IFoo
    {
        string DoSomething();
        string GetRandomIdentifier();
    }
}

