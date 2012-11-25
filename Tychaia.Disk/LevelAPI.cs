using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Tychaia.Disk
{
    public class LevelReference
    {
        public string Name;
        public ILevelAPI Source;
    }

    public static class LevelAPI
    {
        private static List<ILevelAPI> m_LevelAPIs = new List<ILevelAPI>();

        static LevelAPI()
        {
            foreach (FileInfo fi in new DirectoryInfo(Environment.CurrentDirectory).GetFiles("Tychaia.Disk.*.dll"))
            {
                Assembly a = Assembly.LoadFile(fi.FullName);
                foreach (Type t in a.GetTypes())
                    if (typeof(ILevelAPI).IsAssignableFrom(t))
                        m_LevelAPIs.Add(t.GetConstructor(Type.EmptyTypes).Invoke(null) as ILevelAPI);
            }
        }

        public static IEnumerable<LevelReference> GetAvailableLevels()
        {
            foreach (ILevelAPI api in m_LevelAPIs)
                foreach (string s in api.GetAvailableLevels())
                    yield return new LevelReference { Name = s, Source = api };
        }

        public static LevelReference NewLevel(string name)
        {
            foreach (ILevelAPI api in m_LevelAPIs)
            {
                if (api.GetType().Name.Contains("Minecraft"))
                {
                    api.NewLevel(name);
                    return new LevelReference { Name = name, Source = api };
                }
            }
            return null;
        }
    }
}
