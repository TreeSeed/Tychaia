//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Diagnostics;

namespace TychaiaAssetManager
{
    public class AssetManager : Game
    {
        /// <summary>
        /// Runs the asset manager side-by-side with another XNA program
        /// (for example the main game) and then rebinds the IoC providers
        /// for asset management so that assets can be changed in real-time.
        /// </summary>
        public static Process RunAndConnect()
        {
            var info = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location
            };
            return Process.Start(info);
        }

        public AssetManager()
        {
            var graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 420;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            this.Window.Title = "Tychaia Asset Manager";
        }
    }
}
