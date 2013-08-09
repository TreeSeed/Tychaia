// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Linq;

namespace Tychaia.Globals
{
    internal class DefaultFilteredFeatures : IFilteredFeatures
    {
        private readonly Feature[] m_PermittedFeatures;

        public DefaultFilteredFeatures()
        {
            // Change this to effect what features are enabled in the program.
            this.m_PermittedFeatures = new[]
            {
                Feature.RenderEntities,
                Feature.RenderWorld,
                Feature.RenderCellSides,
                Feature.RenderCellTops
            };
        }

        public bool IsEnabled(Feature feature)
        {
            return this.m_PermittedFeatures.Contains(feature);
        }
    }
}