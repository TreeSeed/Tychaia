using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tychaia.Disk.Tychaia
{
    public class TychaiaDiskAPI : ILevelAPI
    {
        public List<string> GetAvailableLevels()
        {
            // Look under %appdata%/.tychaia/saves.
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string saves = Path.Combine(appdata, ".tychaia", "saves");
            List<string> levels = new List<string>();
            if (!Directory.Exists(saves))
                return levels;
            foreach (DirectoryInfo d in new DirectoryInfo(saves).GetDirectories())
                levels.Add(d.Name);
            return levels;
        }

        public ILevel NewLevel(string name)
        {
            throw new NotImplementedException();
        }

        public ILevel LoadLevel(string name)
        {
            throw new NotImplementedException();
        }
    }
}
