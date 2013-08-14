// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals.Tests.Dummies
{
    public class Bar : IBar
    {
        private readonly IFoo m_Foo;

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
