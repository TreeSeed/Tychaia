using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.Disk;

namespace Tychaia.Title
{
    public class LoadWorld : MenuWorld
    {
        public LoadWorld()
        {
            this.AddMenuItem("(return)", () =>
            {
                this.m_TargetWorld = new TitleWorld();
            });

            // Get all available levels.
            foreach (LevelReference levelRef in LevelAPI.GetAvailableLevels())
            {
                this.AddMenuItem(levelRef.Name, () =>
                {
                    this.m_TargetWorld = new RPGWorld(levelRef);
                });
            }
        }
    }
}
