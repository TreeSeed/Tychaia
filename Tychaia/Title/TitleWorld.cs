//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Ninject;
using Protogame;
using Tychaia.Assets;
using Tychaia.Disk;
using Tychaia.Globals;

namespace Tychaia.Title
{
    public class TitleWorld : MenuWorld
    {
        private static bool m_GameJustStarted = true;
        private float m_FadeAmount = 0.0f;

        public TitleWorld()
        {
            if (m_GameJustStarted)
            {
                this.m_FadeAmount = 1.0f;
            }

            var manager = IoC.Kernel.Get<IAssetManagerProvider>().GetAssetManager(false);
            var textGenerateWorld = manager.Get("language.GENERATE_WORLD").Resolve<TextAsset>();
            var textLoadExistingWorld = manager.Get("language.LOAD_EXISTING_WORLD").Resolve<TextAsset>();
            var textRandomizeSeed = manager.Get("language.RANDOMIZE_SEED").Resolve<TextAsset>();
            var textExit = manager.Get("language.EXIT").Resolve<TextAsset>();

            this.AddMenuItem(textGenerateWorld, () =>
            {
                this.m_TargetWorld = new RPGWorld(LevelAPI.NewLevel("Tychaia Demo World"));
            });
            this.AddMenuItem(textLoadExistingWorld, () =>
            {
                this.m_TargetWorld = IoC.Kernel.Get<LoadWorld>();
            });
            this.AddMenuItem(textRandomizeSeed, () =>
            {
                m_StaticSeed = m_Random.Next();
            });
            this.AddMenuItem(textExit, () =>
            {
                this.Game.Exit();
            });
        }

        public override void DrawBelow(GameContext context)
        {
            if (this.m_FadeAmount < 1.0f)
                base.DrawBelow(context);
        }

        public override void DrawAbove(GameContext context)
        {
            if (this.m_FadeAmount < 1.0f)
                base.DrawAbove(context);

            var graphics = new XnaGraphics(context);
            graphics.FillRectangle(
                context.ScreenBounds,
                new Color(0, 0, 0, this.m_FadeAmount));
            this.m_FadeAmount -= 0.01f;
        }
    }
}
