//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Linq;
using Protogame;

namespace Tychaia
{
    public class ScatterBackground
    {
        private IRenderUtilities m_RenderUtilities;
        private IAssetManager m_AssetManager;
    
        public ScatterBackground(IRenderUtilities renderUtilities, IAssetManager assetManager, IWorld world)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManager;
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(new BackgroundCubeEntity(this.m_RenderUtilities, this.m_AssetManager));
            }
        }

        public void Update(IWorld world)
        {
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(new BackgroundCubeEntity(this.m_RenderUtilities, this.m_AssetManager, true));
            }
        }
    }
}

