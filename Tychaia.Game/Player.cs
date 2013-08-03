using Protogame;
using Tychaia.Globals;

namespace Tychaia.Game
{
    public class Player : ChunkEntity
    {
        private double m_RotateCounter = 0;
        private IFilteredConsole m_FilteredConsole;

        public double MovementSpeed
        {
            get;
            private set;
        }

        public Player(
            IWorld world,
            IFilteredConsole filteredConsole)
            : base(world)
		{
            this.m_FilteredConsole = filteredConsole;
            this.Width = 16;
            this.Height = 16;
            this.ImageOffsetX = 8;
            this.ImageOffsetY = 15;
            this.MovementSpeed = 2;
        }
        
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            //this.X = 0;// (float)(0 + Math.Sin(this.m_RotateCounter) * 100);
            //this.Y = 0;
            //this.Z = 32f;

            //if (this.SearchForTerrain)
            //{
            //this.Z -= 1f;
            //}

            this.m_RotateCounter += 0.1;
            this.m_FilteredConsole.WriteLine(FilterCategory.Player, "player x/y/z is " + X + ", " + Y + "," + Z + ".");

            base.Update(gameContext, updateContext);
        }

        public bool SearchForTerrain { get; set; }
    }
}
