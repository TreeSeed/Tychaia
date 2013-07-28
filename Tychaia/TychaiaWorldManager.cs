//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class TychaiaWorldManager : IWorldManager
    {
        private Effect m_Effect;
        
        public void Render<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
            if (this.m_Effect == null)
                this.m_Effect = new BasicEffect(game.GraphicsDevice);
            
            game.RenderContext.Render(game.GameContext);
            
            game.RenderContext.SpriteBatch.Begin();
            
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
            
                foreach (var entity in game.GameContext.World.Entities.ToArray())
                    entity.Render(game.GameContext, game.RenderContext);
            
                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }
            
            game.RenderContext.SpriteBatch.End();
        }
        
        public void Update<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);
            
            foreach (var entity in game.GameContext.World.Entities.ToArray())
                entity.Update(game.GameContext, game.UpdateContext);
            
            game.GameContext.World.Update(game.GameContext, game.UpdateContext);
        }
    }
}

