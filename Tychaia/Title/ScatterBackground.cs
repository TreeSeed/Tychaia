using Protogame;
using System.Linq;

namespace Tychaia
{
    public class ScatterBackground
    {
        public ScatterBackground(GameContext context, World world)
        {
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(new BackgroundCubeEntity(context));
            }
        }

        public void Update(GameContext context, World world)
        {
            while (world.Entities.Count(x => x is BackgroundCubeEntity) < 100)
            {
                world.Entities.Add(new BackgroundCubeEntity(context, true));
            }
        }
    }
}

