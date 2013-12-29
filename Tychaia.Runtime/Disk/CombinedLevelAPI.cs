// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace Tychaia.Runtime
{
    public class CombinedLevelAPI : ILevelAPI
    {
        private ILevelAPIImpl m_DefaultLevelImplementation;
        private ILevelAPIImpl[] m_LevelImplementations;

        public CombinedLevelAPI(
            [Named("Default")] ILevelAPIImpl defaultLevelImplementation,
            ILevelAPIImpl[] levelImplementations)
        {
            this.m_DefaultLevelImplementation = defaultLevelImplementation;
            this.m_LevelImplementations = levelImplementations;
        }

        public IEnumerable<string> GetAvailableLevels()
        {
            foreach (var impl in this.m_LevelImplementations)
                foreach (var level in impl.GetAvailableLevels())
                    yield return level;
        }

        public ILevel NewLevel(string name)
        {
            return this.m_DefaultLevelImplementation.NewLevel(name);
        }

        public ILevel LoadLevel(string name)
        {
            return (from impl in this.m_LevelImplementations
                    from level in impl.GetAvailableLevels()
                    where level == name
                    select impl.LoadLevel(name)).First();
        }
    }
}
