// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Globals;
using PerformanceCounter = System.Diagnostics.PerformanceCounter;

namespace Tychaia
{
    public class TychaiaProfilerEntity : Entity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly FontAsset m_DefaultFontAsset;
        private readonly List<FrameProfileInfo> m_ProfilingInformation;
        private readonly IPersistentStorage m_PersistentStorage;
        private readonly TychaiaProfilerEntityUtil m_TychaiaProfilerEntityUtil;

        public TychaiaProfilerEntity(
            TychaiaProfiler profiler,
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IPersistentStorage persistentStorage)
        {
            this.Profiler = profiler;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_DefaultFontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            this.m_ProfilingInformation = new List<FrameProfileInfo>();
            this.m_PersistentStorage = persistentStorage;
            this.m_TychaiaProfilerEntityUtil = new TychaiaProfilerEntityUtil();
        }
        
        public TychaiaProfiler Profiler
        {
            get;
            private set;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            this.m_ProfilingInformation.Add(this.Sample(gameContext));
            if (this.m_ProfilingInformation.Count > 300)
                this.m_ProfilingInformation.RemoveAt(0);
        }

        public void RenderMaximums(IGameContext gameContext, IRenderContext renderContext)
        {
            if ((this.m_PersistentStorage.Settings.VisibleProfiling ?? true) == false)
                return;
                    
            this.Profiler.CheckSlowFrames();
            var stats = this.Profiler.GetRenderStats();

            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, 300, 224 + (stats == null ? 0 : (stats.Count * 20))),
                new Color(0, 0, 0, 0.5f),
                true);

            if (this.m_ProfilingInformation.Count != 0)
            {
                this.m_TychaiaProfilerEntityUtil.RenderMaximums(
                    this.m_2DRenderUtilities,
                    renderContext,
                    this.m_DefaultFontAsset,
                    this.m_ProfilingInformation);
            }
            
            if (stats != null)
            {
                var i = 0;
                foreach (var kv in stats.OrderByDescending(x => x.Value))
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(10, 224 + (i * 20)),
                        kv.Key,
                        this.m_DefaultFontAsset);
                    var color = Color.White;
                    if (kv.Value > 16000)
                        color = Color.Red;
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(290, 224 + (i * 20)),
                        ((int)kv.Value) + "us",
                        this.m_DefaultFontAsset,
                        horizontalAlignment: HorizontalAlignment.Right,
                        textColor: color);
                    i++;
                }
            }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            if ((this.m_PersistentStorage.Settings.VisibleProfiling ?? true) == false)
                return;

            if (this.m_ProfilingInformation.Count == 0)
                return;

            this.m_TychaiaProfilerEntityUtil.RenderLines(
                gameContext,
                this.m_ProfilingInformation);
        }

        private FrameProfileInfo Sample(IGameContext gameContext)
        {
            var info = new FrameProfileInfo
            {
                Entities = gameContext.World.Entities.Count,
                FunctionCalls = this.Profiler.FunctionCallsSinceLastReset,
                FPS = gameContext.FPS,
                LastFrameLength = this.Profiler.LastFrameLength,
                VirtualMemory = Process.GetCurrentProcess().VirtualMemorySize64 / 1024 / 1024,
                SendNetworkOps = this.Profiler.GetSendNetworkOps(),
                ReceiveNetworkOps = this.Profiler.GetReceiveNetworkOps()
            };
            this.Profiler.ResetCalls();
            return info;
        }
    }
}
