//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class IsometricEntity : Entity, IIsometricBoundingBox
    {
        public virtual float Z { get; set; }
        public virtual float Depth { get; set; }
        public virtual float ZSpeed { get; set; }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.X += this.XSpeed;
            this.Y += this.YSpeed;
            this.Z += this.ZSpeed;
        }
    }
}

