// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System;
using System.IO;

namespace Tychaia
{
    public class TychaiaLevelAPIImpl : ILevelAPIImpl
    {
        private ITychaiaLevelFactory m_TychaiaLevelFactory;

        public TychaiaLevelAPIImpl(
            ITychaiaLevelFactory tychaiaLevelFactory)
        {
            this.m_TychaiaLevelFactory = tychaiaLevelFactory;
        }

        public IEnumerable<string> GetAvailableLevels()
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
            return this.m_TychaiaLevelFactory.CreateSimpleLevel(name, path);
        }

        public ILevel LoadLevel(string name)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appdata, ".tychaia", "saves", name);
            return this.m_TychaiaLevelFactory.CreateSimpleLevel(name, path);
        }
    }
}
