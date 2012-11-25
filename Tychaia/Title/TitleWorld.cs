using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tychaia.UI;
using Tychaia.Disk;

namespace Tychaia.Title
{
    public class TitleWorld : MenuWorld
    {
        public TitleWorld()
        {
            this.AddMenuItem("Generate World", () =>
            {
                this.m_TargetWorld = new RPGWorld(LevelAPI.NewLevel("Tychaia Demo World"));
            });
            this.AddMenuItem("Load Existing World", () =>
            {
                this.m_TargetWorld = new LoadWorld();
            });
            this.AddMenuItem("Randomize Seed", () =>
            {
                m_StaticSeed = m_Random.Next();
            });
            this.AddMenuItem("Exit", () =>
            {
                (this.Game as RuntimeGame).Exit();
            });
        }
    }
}
