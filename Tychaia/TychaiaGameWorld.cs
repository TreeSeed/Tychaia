using System;
using Protogame;
using System.Collections.Generic;

namespace Tychaia
{
    public class TychaiaGameWorld : IWorld
    {
        public List<IEntity> Entities { get; private set; }
        
        public TychaiaGameWorld()
        {
            this.Entities = new List<IEntity>();
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

