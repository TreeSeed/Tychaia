using Protogame;
using Tychaia.Globals;

namespace Tychaia.Game
{
    public class Player : ChunkEntity
    {
        private double m_RotateCounter = 0;

        public double MovementSpeed
        {
            get;
            private set;
        }

        public Player(IWorld world) : base(world)
        {
            this.Images = this.GetTexture("chars.player.player");
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
            FilteredConsole.WriteLine(FilterCategory.Player, "player x/y/z is " + X + ", " + Y + "," + Z + ".");

            base.Update(gameContext, updateContext);
        }

        public bool SearchForTerrain { get; set; }
    }
}
