// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;

namespace PerspectiveTest
{
    public class PerspectiveGame : Game
    {
        private IRenderDemo[] m_Demos;
        private int m_Current;
    
        public PerspectiveGame(IEnumerable<IRenderDemo> demos)
        {
            var graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            
            this.IsMouseVisible = true;
            this.m_Demos = demos.ToArray();
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            foreach (var demo in this.m_Demos)
                demo.LoadContent(this);
        }
        
        protected override void Update(GameTime gameTime)
        {
            this.m_Demos[this.m_Current].Update(this);
            
            var mouse = Mouse.GetState();
            if (mouse.LeftPressed(this))
            {
                this.m_Current++;
                if (this.m_Current >= this.m_Demos.Length)
                    this.m_Current = 0;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            this.m_Demos[this.m_Current].Draw(this);
        }
    }
}
