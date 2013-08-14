// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //

#if FALSE
using System;
using System.Collections.Generic;
using System.IO;

namespace Tychaia.Disk.Minecraft
{
    public class MinecraftLevelAPI : ILevelAPI
    {
        public List<string> GetAvailableLevels()
        {
            // Look under %appdata%/.minecraft/saves.
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string saves = Path.Combine(appdata, ".minecraft", "saves");
            List<string> levels = new List<string>();
            if (!Directory.Exists(saves))
                return levels;
            foreach (DirectoryInfo d in new DirectoryInfo(saves).GetDirectories())
                levels.Add(d.Name);
            return levels;
        }

        public ILevel NewLevel(string name)
        {
            throw new NotSupportedException();
        }

        public ILevel LoadLevel(string name)
        {
            return new MinecraftLevel(name);
        }
    }
}
#endif
