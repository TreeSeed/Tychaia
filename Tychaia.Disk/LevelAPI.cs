// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Collections.Generic;
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
        private static readonly List<ILevelAPI> m_LevelAPIs = new List<ILevelAPI>();

        static LevelAPI()
        {
            foreach (var fi in new DirectoryInfo(Environment.CurrentDirectory).GetFiles("Tychaia.Disk.*.dll"))
            {
                var a = Assembly.LoadFile(fi.FullName);
                foreach (var t in a.GetTypes())
                    if (typeof(ILevelAPI).IsAssignableFrom(t))
                    {
                        var constructorInfo = t.GetConstructor(Type.EmptyTypes);
                        if (constructorInfo != null)
                            m_LevelAPIs.Add(constructorInfo.Invoke(null) as ILevelAPI);
                    }
            }
        }

        public static IEnumerable<LevelReference> GetAvailableLevels()
        {
            foreach (var api in m_LevelAPIs)
                foreach (var s in api.GetAvailableLevels())
                    yield return new LevelReference { Name = s, Source = api };
        }

        public static LevelReference NewLevel(string name)
        {
            foreach (var api in m_LevelAPIs)
            {
                if (api.GetType().Name.Contains("Tychaia"))
                {
                    api.NewLevel(name);
                    return new LevelReference { Name = name, Source = api };
                }
            }
            return null;
        }
    }
}