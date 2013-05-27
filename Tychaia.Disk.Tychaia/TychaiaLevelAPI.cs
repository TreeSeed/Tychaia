using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if FALSE
namespace Tychaia.Disk.Tychaia
{
    public class TychaiaLevelAPI : ILevelAPI
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
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = Path.Combine(appdata, ".tychaia", "saves", name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return new TychaiaLevel(name);
        }

        public ILevel LoadLevel(string name)
        {
            return new TychaiaLevel(name);
        }
    }
}
#endif
