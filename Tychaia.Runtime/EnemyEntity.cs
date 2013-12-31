// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Game;
using Tychaia.Globals;
using Tychaia.Data;

namespace Tychaia
{
    public class EnemyEntity : Entity, IHasRuntimeData<Enemy>
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;
        private readonly TextureAsset m_PlayerTexture;
        private readonly string m_BeingDefinitionAssetName;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IConsole m_Console;
        private readonly IFilteredFeatures m_FilteredFeatures;

        private double m_DemoTicks = 0;

        public EnemyEntity(
            IAssetManagerProvider assetManagerProvider,
            I3DRenderUtilities threedRenderUtilities,
            IChunkSizePolicy chunkSizePolicy,
            IConsole console,
            IFilteredFeatures filteredFeatures,
            Enemy runtimeData,
            Cell cell)
        {
            this.m_FilteredFeatures = filteredFeatures;
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_PlayerTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("chars.player.Player");
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Console = console;
            this.RuntimeData = runtimeData;
            
            this.m_BeingDefinitionAssetName = cell.BeingDefinitionAssetName;

            this.Width = 16;
            this.Height = 16;
            this.Depth = 16;
        }
        
        public bool InaccurateY { get; set; }

        public float MovementSpeed
        {
            get { return 4; }
        }

        public Enemy RuntimeData
        {
            get;
            set;
        }

        public void MoveInDirection(IGameContext context, int directionInDegrees)
        {
            var x = Math.Sin(MathHelper.ToRadians(directionInDegrees - 45)) * this.MovementSpeed;
            var y = -Math.Cos(MathHelper.ToRadians(directionInDegrees - 45)) * this.MovementSpeed;

            // Determine if moving here would require us to move up by more than 32 pixels.
            var targetX = this.GetSurfaceY(context, this.X + (float)x, this.Z);
            var targetZ = this.GetSurfaceY(context, this.X, this.Z + (float)y);

            // We calculate X and Z independently so that we can "slide" along the edge of somewhere
            // that the player can't go.  This creates a more natural feel when walking into something
            // that it isn't entirely possible to walk through.

            // If the target returns null, then the chunk hasn't been generated so don't permit
            // the character to move onto it.
            if (targetX != null)
            {
                // If the target height difference and our current height is greater than 32, don't permit
                // the character to move onto it.  This also prevents the character from falling off
                // tall cliffs.
                if (Math.Abs(targetX.Value - this.Y) <= 32)
                {
                    this.X += (float)x;
                }
            }

            // If the target returns null, then the chunk hasn't been generated so don't permit
            // the character to move onto it.
            if (targetZ != null)
            {
                // If the target height difference and our current height is greater than 32, don't permit
                // the character to move onto it.  This also prevents the character from falling off
                // tall cliffs.
                if (Math.Abs(targetZ.Value - this.Y) <= 32)
                {
                    this.Z += (float)y;
                }
            }
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;
            
            var matrix = Matrix.CreateTranslation(-0.5f, 1, 0);
            matrix *= Matrix.CreateScale((float)Math.Sqrt(2f) / 2f, 1, 1);
            matrix *= Matrix.CreateRotationY(MathHelper.PiOver4);
            matrix *= this.InaccurateY ? Matrix.CreateRotationY(MathHelper.PiOver4 * 0.75f) : Matrix.Identity;
            matrix *= Matrix.CreateTranslation(
                this.X / this.m_ChunkSizePolicy.CellVoxelWidth,
                this.Y / this.m_ChunkSizePolicy.CellVoxelHeight,
                this.Z / this.m_ChunkSizePolicy.CellVoxelDepth);
                
            this.m_3DRenderUtilities.RenderTexture(
                renderContext,
                matrix,
                this.m_PlayerTexture);
        }

        private float? GetSurfaceY(IGameContext context, float xx, float zz)
        {
            return null;
        }
    }
}
