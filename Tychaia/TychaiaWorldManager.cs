// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class TychaiaWorldManager : IWorldManager
    {
#if DEBUG
        private readonly TychaiaProfilerEntity m_TychaiaProfilerEntity;
        private readonly TychaiaProfiler m_TychaiaProfiler;

        public TychaiaWorldManager(
            TychaiaProfilerEntity tychaiaProfilerEntity)
        {
            this.m_TychaiaProfilerEntity = tychaiaProfilerEntity;
            this.m_TychaiaProfiler = this.m_TychaiaProfilerEntity.Profiler;
        }
#endif

        public void Render<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
#if DEBUG
            var handle = this.m_TychaiaProfiler.Measure("tychaia-render");
#endif

            game.RenderContext.Render(game.GameContext);

            game.RenderContext.Is3DContext = true;

            using (this.m_TychaiaProfiler.Measure("tychaia-render_3d"))
            {
                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);
    
                using (this.m_TychaiaProfiler.Measure("tychaia-render_entities_3d"))
                {
                    foreach (var entity in game.GameContext.World.Entities.ToArray())
                        using (this.m_TychaiaProfiler.Measure("tychaia-render_entities_3d_" + entity.GetType().Name))
                            entity.Render(game.GameContext, game.RenderContext);
                }
    
                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);
            }

            game.RenderContext.Is3DContext = false;

            var handle2d = this.m_TychaiaProfiler.Measure("tychaia-render_2d");
            
            game.RenderContext.SpriteBatch.Begin();

            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                game.GameContext.World.RenderBelow(game.GameContext, game.RenderContext);

                using (this.m_TychaiaProfiler.Measure("tychaia-render_entities_2d"))
                {
                    foreach (var entity in game.GameContext.World.Entities.OrderBy(x => x.Z).ToArray())
                        entity.Render(game.GameContext, game.RenderContext);
                }

                game.GameContext.World.RenderAbove(game.GameContext, game.RenderContext);

#if DEBUG
                handle2d.Dispose();
                handle.Dispose();
                this.m_TychaiaProfilerEntity.RenderMaximums(game.GameContext, game.RenderContext);
                handle = this.m_TychaiaProfiler.Measure("tychaia-render");
                handle2d = this.m_TychaiaProfiler.Measure("tychaia-render_2d");
#endif
            }

            game.RenderContext.SpriteBatch.End();

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

#if DEBUG
            handle.Dispose();
#endif

#if DEBUG
            // Cache the matrixes because we need to render the profiler UI.
            var oldView = game.RenderContext.View;
            var oldProjection = game.RenderContext.Projection;
            var oldWorld = game.RenderContext.World;

            // Set up the matrix to match the sprite batch.
            var projection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height, 0, 0, 1);
            var halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            game.RenderContext.World = Matrix.Identity;
            game.RenderContext.View = Matrix.Identity;
            game.RenderContext.Projection = halfPixelOffset * projection;
            ((BasicEffect)game.RenderContext.Effect).LightingEnabled = false;
            game.RenderContext.EnableVertexColors();

            // Render profiler.
            foreach (var pass in game.RenderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.m_TychaiaProfilerEntity.Render(game.GameContext, game.RenderContext);
            }

            // Restore matrixes.
            game.RenderContext.View = oldView;
            game.RenderContext.Projection = oldProjection;
            game.RenderContext.World = oldWorld;
#endif
        }

        public void Update<T>(T game) where T : Microsoft.Xna.Framework.Game, ICoreGame
        {
#if DEBUG
            this.m_TychaiaProfiler.StartRenderStats();
            using (this.m_TychaiaProfiler.Measure("tychaia-update"))
            {
#endif
            
            game.UpdateContext.Update(game.GameContext);

            foreach (var entity in game.GameContext.World.Entities.ToArray())
                entity.Update(game.GameContext, game.UpdateContext);

            game.GameContext.World.Update(game.GameContext, game.UpdateContext);

#if DEBUG
            }
            
            this.m_TychaiaProfilerEntity.Update(game.GameContext, game.UpdateContext);
#endif
        }
    }
}