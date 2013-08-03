//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class Player : IsometricEntity
    {
        private IRenderUtilities m_RenderUtilities;
    
        private TextureAsset m_PlayerAsset;
        
        public float MovementSpeed { get { return 4; } }
    
        public Player(
            IAssetManager assetManager,
            IRenderUtilities renderUtilities)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_PlayerAsset = assetManager.Get<TextureAsset>("chars.player.Player");
            
            this.Width = 16;
            this.Height = 16;
            this.Depth = 16;
        }
        
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(this.X, this.Y),
                this.m_PlayerAsset);
        }
    }
}

