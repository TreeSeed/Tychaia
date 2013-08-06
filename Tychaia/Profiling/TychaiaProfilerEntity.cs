//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class TychaiaProfilerEntity : Entity
    {
        private TychaiaProfiler m_Profiler;
        private IRenderTargetFactory m_RenderStateFactory;
        private I2DRenderUtilities m_2DRenderUtilities;
        private IAssetManager m_AssetManager;
        private FontAsset m_DefaultFontAsset;
        private List<FrameProfileInfo> m_ProfilingInformation;
    
        public TychaiaProfilerEntity(
            TychaiaProfiler profiler,
            IRenderTargetFactory renderStateFactory,
            I2DRenderUtilities _2dRenderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_Profiler = profiler;
            this.m_RenderStateFactory = renderStateFactory;
            this.m_2DRenderUtilities = _2dRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_DefaultFontAsset = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_ProfilingInformation = new List<FrameProfileInfo>();
        }
        
        private FrameProfileInfo Sample(IGameContext gameContext, IUpdateContext updateContext)
        {
            var info = new FrameProfileInfo
            {
                Entities = gameContext.World.Entities.Count,
                FunctionCalls = this.m_Profiler.FunctionCallsSinceLastReset,
                RenderTargetsUsed = this.m_RenderStateFactory.RenderTargetsUsed,
                RenderTargetsRAM = this.m_RenderStateFactory.RenderTargetMemory,
                FPS = gameContext.FPS
            };
            this.m_Profiler.ResetCalls();
            return info;
        }
    
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);
            
            this.m_ProfilingInformation.Add(this.Sample(gameContext, updateContext));
            if (this.m_ProfilingInformation.Count > 300)
                this.m_ProfilingInformation.RemoveAt(0);
        }
        
        public void RenderMaximums(IGameContext gameContext, IRenderContext renderContext)
        {
            Action<int, double, Color> drawMaximum = (offset, maximum, color) =>
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(4 + offset, 4),
                    maximum.ToString(),
                    this.m_DefaultFontAsset,
                    textColor: color);
            };
            
            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, 300, 224),
                new Color(0, 0, 0, 0.5f),
                filled: true);
            
            if (this.m_ProfilingInformation.Count == 0)
                return;
            
            var maximumEntities = this.m_ProfilingInformation.Select(x => x.Entities).Last();
            var maximumFunctionCalls = this.m_ProfilingInformation.Select(x => x.FunctionCalls).Last();
            var maximumRenderTargetsUsed = this.m_ProfilingInformation.Select(x => x.RenderTargetsUsed).Last();
            var maximumRenderTargetsRAM = this.m_ProfilingInformation.Select(x => x.RenderTargetsRAM).Last();
            var maximumFPS = this.m_ProfilingInformation.Select(x => x.FPS).Last();
            
            drawMaximum(0, maximumEntities, Color.Cyan);
            drawMaximum(60, maximumFunctionCalls, Color.Green);
            drawMaximum(120, maximumRenderTargetsUsed, Color.Orange);
            drawMaximum(180, maximumRenderTargetsRAM / 1024 / 1024, Color.Purple);
            drawMaximum(240, maximumFPS, Color.Yellow);
        }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);
            
            if (this.m_ProfilingInformation.Count == 0)
                return;
                
            Action<int, Func<FrameProfileInfo, double>, double, VertexPositionColor[], Color, int> addToLine = (i, value, maximum, vertexList, color, offset) =>
            {
                var sample = this.m_ProfilingInformation[i];
                vertexList[i] = new VertexPositionColor(
                    new Vector3(i, (float)(224 - (value(sample) / maximum) * (200 - offset)), 0),
                    color);
            };
            Action<VertexPositionColor[], short[]> renderLine = (vertexList, lsi) =>
            {
                if (vertexList.Length == 0)
                    return;
                gameContext.Graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip,
                    vertexList,
                    0,
                    vertexList.Length,
                    lsi,
                    0,
                    lsi.Length - 1);
            };
            
            var maximumEntities = this.m_ProfilingInformation.Select(x => x.Entities).Max();
            var maximumFunctionCalls = this.m_ProfilingInformation.Select(x => x.FunctionCalls).Max();
            var maximumRenderTargetsUsed = this.m_ProfilingInformation.Select(x => x.RenderTargetsUsed).Max();
            var maximumRenderTargetsRAM = this.m_ProfilingInformation.Select(x => x.RenderTargetsRAM).Max();
            var maximumFPS = this.m_ProfilingInformation.Select(x => x.FPS).Max();

            var entitiesLine = new VertexPositionColor[this.m_ProfilingInformation.Count - 1];
            var functionCallsLine = new VertexPositionColor[this.m_ProfilingInformation.Count - 1];
            var renderTargetsUsedLine = new VertexPositionColor[this.m_ProfilingInformation.Count - 1];
            var renderTargetsRAMLine = new VertexPositionColor[this.m_ProfilingInformation.Count - 1];
            var fpsLine = new VertexPositionColor[this.m_ProfilingInformation.Count - 1];
            var lineStripIndices = new short[this.m_ProfilingInformation.Count - 1];
            for (short i = 0; i < this.m_ProfilingInformation.Count - 1; i++)
            {
                lineStripIndices[i] = i;
                addToLine(i, x => x.Entities, maximumEntities, entitiesLine, Color.Cyan, 0);
                addToLine(i, x => x.FunctionCalls, maximumFunctionCalls, functionCallsLine, Color.Green, 4);
                addToLine(i, x => x.RenderTargetsUsed, maximumRenderTargetsUsed, renderTargetsUsedLine, Color.Orange, 8);
                addToLine(i, x => x.RenderTargetsRAM, maximumRenderTargetsRAM, renderTargetsRAMLine, Color.Purple, 12);
                addToLine(i, x => x.FPS, maximumFPS, fpsLine, Color.Yellow, 16);
            }
            renderLine(entitiesLine, lineStripIndices);
            renderLine(functionCallsLine, lineStripIndices);
            renderLine(renderTargetsUsedLine, lineStripIndices);
            renderLine(renderTargetsRAMLine, lineStripIndices);
            renderLine(fpsLine, lineStripIndices);
        }
        
        private class FrameProfileInfo
        {
            public int FunctionCalls;
            public int RenderTargetsUsed;
            public long RenderTargetsRAM;
            public int FPS;
            public int Entities;
        }
    }
}

