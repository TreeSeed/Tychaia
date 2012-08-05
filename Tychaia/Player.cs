using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework.Input;
using Tychaia.Generators;

namespace Tychaia
{
    public class Player : ChunkEntity
    {
        private double m_RotateCounter = 0;

        public Player(World world) : base(world)
        {
            this.Images = this.GetTexture("chars.player.player");
            this.Width = 16;
            this.Height = 16;
        }

        public override void Update(World world)
        {
            this.X = (float)(400 + Math.Sin(this.m_RotateCounter) * 300);
            this.Y = 100;
            this.m_RotateCounter += 0.1;

            base.Update(world);
        }
    }
}
