// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Input;
using Protogame;

namespace Tychaia
{
    public class TychaiaDefaultDesktopBinder : StaticEventBinder
    {
        public override void Configure()
        {
            this.Bind<KeyPressEvent>(x => x.Key == Keys.I).OnTogglable<InventoryUIEntity>().ToToggle("inventory");
            this.Bind<KeyPressEvent>(x => x.Key == Keys.C).OnTogglable<InventoryUIEntity>().ToToggle("character");
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.W).On<PlayerEntity>().To<MoveForwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.A).On<PlayerEntity>().To<MoveLeftAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.S).On<PlayerEntity>().To<MoveBackwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.D).On<PlayerEntity>().To<MoveRightAction>();
        }
    }
}
