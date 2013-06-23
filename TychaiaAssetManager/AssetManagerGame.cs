//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using Tychaia.Assets;

namespace TychaiaAssetManager
{
    public class AssetManagerGame : CoreGame<AssetManagerWorld, WorldManager>
    {
        private IAssetManager m_AssetManager;

        private AssetManagerWorld AssetWorld
        {
            get { return this.World as AssetManagerWorld; }
        }

        public AssetManagerGame(IAssetManager manager)
        {
            this.m_AssetManager = manager;

            this.m_GameContext.Graphics.IsFullScreen = false;
            this.m_GameContext.Graphics.PreferredBackBufferWidth = 420;
            this.m_GameContext.Graphics.PreferredBackBufferHeight = 800;
            this.m_GameContext.Graphics.ApplyChanges();
            this.IsMouseVisible = true;
            this.Window.Title = "Tychaia Asset Manager";

            this.m_AssetManager.Status = "Initializing...";
            this.AssetWorld.AssetManager = this.m_AssetManager;
        }

        protected override void LoadContent()
        {
            // Load protogame's content.
            base.LoadContent();

            // Load all the textures.
            this.m_GameContext.LoadFont("Arial");
        }
    }
}
