// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Protogame;

namespace Tychaia
{
    public class ScatterBackground
    {
        private readonly IBackgroundCubeEntityFactory m_BackgroundCubeEntityFactory;

        public ScatterBackground(
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            IWorld world)
        {
            this.m_BackgroundCubeEntityFactory = backgroundCubeEntityFactory;
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(this.m_BackgroundCubeEntityFactory.CreateBackgroundCubeEntity(false));
            }
        }

        public void Update(IWorld world)
        {
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(this.m_BackgroundCubeEntityFactory.CreateBackgroundCubeEntity(true));
            }
        }
    }
}
