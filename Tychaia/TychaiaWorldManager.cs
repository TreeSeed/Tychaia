//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Tychaia
{
    public class TychaiaWorldManager : IWorldManager
    {
        private TychaiaProfilerEntity m_TychaiaProfilerEntity;
        
        public TychaiaWorldManager(
            TychaiaProfilerEntity tychaiaProfilerEntity)
        {
            this.m_TychaiaProfilerEntity = tychaiaProfilerEntity;
        }
        
        public void Render<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
            game.RenderContext.Render(game.GameContext);
            
            game.RenderContext.Is3DContext = true;
            
            game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
        
            foreach (var entity in game.GameContext.World.Entities)
                entity.Render(game.GameContext, game.RenderContext);
        
            game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            
            game.RenderContext.Is3DContext = false;
            
            game.RenderContext.SpriteBatch.Begin();
            
            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
            
                foreach (var entity in game.GameContext.World.Entities.OrderBy(x => x.Z))
                    entity.Render(game.GameContext, game.RenderContext);
            
                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }
            
            game.RenderContext.SpriteBatch.End();
            
            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.m_TychaiaProfilerEntity.Render(game.GameContext, game.RenderContext);
            }
        }
        
        public void Update<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
            game.UpdateContext.Update(game.GameContext);
            
            foreach (var entity in game.GameContext.World.Entities)
                entity.Update(game.GameContext, game.UpdateContext);
            
            game.GameContext.World.Update(game.GameContext, game.UpdateContext);
            
            this.m_TychaiaProfilerEntity.Update(game.GameContext, game.UpdateContext);
        }
    }
}

