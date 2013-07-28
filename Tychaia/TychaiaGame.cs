//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject;
using Protogame;

namespace Tychaia
{
    public class TychaiaGame : CoreGame<TitleWorld, TychaiaWorldManager>
    {
        public TychaiaGame(IKernel kernel) : base(kernel)
        {
            this.IsMouseVisible = true;
        }
    }
}

