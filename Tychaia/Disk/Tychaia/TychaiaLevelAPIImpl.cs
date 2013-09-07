// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System;
using System.IO;
using Tychaia.Globals;

namespace Tychaia
{
    public class TychaiaLevelAPIImpl : ILevelAPIImpl
    {
        private ITychaiaLevelFactory m_TychaiaLevelFactory;
        private IPersistentStorage m_PersistentStorage;

        public TychaiaLevelAPIImpl(
            ITychaiaLevelFactory tychaiaLevelFactory,
            IPersistentStorage persistentStorage)
        {
            this.m_TychaiaLevelFactory = tychaiaLevelFactory;
            this.m_PersistentStorage = persistentStorage;
        }

        public IEnumerable<string> GetAvailableLevels()
        {
            var dir = this.m_PersistentStorage.SaveDirectory;
            List<string> levels = new List<string>();
            if (!dir.Exists)
                return levels;
            foreach (DirectoryInfo d in dir.GetDirectories())
                levels.Add(d.Name);
            return levels;
        }

        public ILevel NewLevel(string name)
        {
            var dir = new DirectoryInfo(Path.Combine(
                this.m_PersistentStorage.SaveDirectory.FullName,
                name));
            if (!dir.Exists)
                dir.Create();
            return this.m_TychaiaLevelFactory.CreateSimpleLevel(name, dir.FullName);
        }

        public ILevel LoadLevel(string name)
        {
            var dir = new DirectoryInfo(Path.Combine(
                this.m_PersistentStorage.SaveDirectory.FullName,
                name));
            return this.m_TychaiaLevelFactory.CreateSimpleLevel(name, dir.FullName);
        }
    }
}
