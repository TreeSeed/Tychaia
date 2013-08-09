// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Linq;
using Ninject;
using Protogame;

namespace Tychaia
{
    public class TychaiaGame : CoreGame<TitleWorld, TychaiaWorldManager>
    {
        private readonly IAssetManager m_AssetManager;
        private readonly ITextureAtlasAssetFactory m_TextureAtlasAssetFactory;

        public TychaiaGame(IKernel kernel) : base(kernel)
        {
            this.IsMouseVisible = true;
            this.m_AssetManager = kernel.Get<IAssetManagerProvider>().GetAssetManager();
            this.m_TextureAtlasAssetFactory = kernel.Get<ITextureAtlasAssetFactory>();
        }

        protected override void LoadContent()
        {
            // Build up the texture atlas.
            var textureAtlas = this.m_TextureAtlasAssetFactory.CreateTextureAtlasAsset(
                "atlas",
                this.GraphicsDevice,
                this.m_AssetManager.GetAll()
                    .Where(x => x.Name.StartsWith("texture."))
                    .Select(x => x.Resolve<TextureAsset>()));
            this.m_AssetManager.Save(textureAtlas);

            // Perform the core game loading (this also creates the initial world, which
            // is why we need to create the texture atlas before; since it will be used).
            base.LoadContent();

            // Set up the window.
            this.Window.Title = "Tychaia";
            this.GameContext.ResizeWindow(1024, 768);
        }
    }
}