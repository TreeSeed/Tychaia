// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class DefaultCaptureService : ICaptureService
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly FontAsset m_DefaultFont;

        private CaptureState m_CaptureState;

        public DefaultCaptureService(
            I2DRenderUtilities twoDRenderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        private enum CaptureStateMode
        {
            Requested,
            Converting,
            Converted,
            Actioning,
            Error,
            Success
        }

        public void CaptureFrame(IGameContext gameContext, Action<byte[]> onCapture)
        {
            if (this.m_CaptureState != null)
            {
                return;
            }

            this.m_CaptureState = new CaptureState
            {
                Mode = CaptureStateMode.Requested,
                RenderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight),
                Action = onCapture
            };
        }

        public void RenderBelow(ICoreGame game)
        {
            if (this.m_CaptureState != null && this.m_CaptureState.Mode == CaptureStateMode.Requested)
            {
                game.RenderContext.PushRenderTarget(this.m_CaptureState.RenderTarget);
            }
        }

        public void RenderAbove(ICoreGame game)
        {
            if (this.m_CaptureState != null && this.m_CaptureState.Mode == CaptureStateMode.Requested)
            {
                game.RenderContext.PopRenderTarget();

                this.m_CaptureState.Mode = CaptureStateMode.Converting;
                this.m_CaptureState.CaptureBuffer = 
                    new Microsoft.Xna.Framework.Color[this.m_CaptureState.RenderTarget.Width * this.m_CaptureState.RenderTarget.Height];
                this.m_CaptureState.RenderTarget.GetData(this.m_CaptureState.CaptureBuffer);
                this.m_CaptureState.CaptureBitmap = new Bitmap(
                    this.m_CaptureState.RenderTarget.Width, 
                    this.m_CaptureState.RenderTarget.Height,
                    PixelFormat.Format32bppArgb);
                this.m_CaptureState.CapturedFrameConversionX = 0;
                this.m_CaptureState.CapturedFrameConversionY = 0;
            }
        }

        public void Render2D(ICoreGame game)
        {
            var renderContext = game.RenderContext;

            if (this.m_CaptureState == null)
            {
                return;
            }

            if (this.m_CaptureState.Mode == CaptureStateMode.Converting ||
                this.m_CaptureState.Mode == CaptureStateMode.Actioning)
            {
                this.m_2DRenderUtilities.RenderRectangle(
                    renderContext,
                    new Microsoft.Xna.Framework.Rectangle(
                        (this.m_CaptureState.RenderTarget.Width / 2) - 100,
                        15,
                        200,
                        24),
                    Microsoft.Xna.Framework.Color.Black,
                    true);
            }

            switch (this.m_CaptureState.Mode)
            {
                case CaptureStateMode.Converting:
                    var total = this.m_CaptureState.CaptureBitmap.Width * this.m_CaptureState.CaptureBitmap.Height;
                    var count = this.m_CaptureState.CapturedFrameConversionX + 
                                (this.m_CaptureState.CapturedFrameConversionY * this.m_CaptureState.CaptureBitmap.Width);

                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Microsoft.Xna.Framework.Vector2(this.m_CaptureState.RenderTarget.Width / 2, 20),
                        "Converting screen capture: " + Math.Round((count / (double)total) * 100.0) + "%",
                        this.m_DefaultFont,
                        horizontalAlignment: HorizontalAlignment.Center);
                    break;
                case CaptureStateMode.Actioning:
                    var tick = ".";
                    for (var i = 0; i < this.m_CaptureState.UploadTick / 15; i++)
                        tick += ".";
                    this.m_CaptureState.UploadTick += 1;
                    if (this.m_CaptureState.UploadTick > 15 * 4)
                    {
                        this.m_CaptureState.UploadTick = 0;
                    }

                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Microsoft.Xna.Framework.Vector2(this.m_CaptureState.RenderTarget.Width / 2, 20),
                        "Uploading screen capture." + tick,
                        this.m_DefaultFont,
                        horizontalAlignment: HorizontalAlignment.Center);
                    break;
            }
        }

        public void Update(ICoreGame game)
        {
            if (this.m_CaptureState == null || this.m_CaptureState.Mode != CaptureStateMode.Converting)
            {
                return;
            }

            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < 3)
            {
                if (this.ProcessCaptureFramePixel())
                {
                    break;
                }
            }

            if (this.m_CaptureState.Mode != CaptureStateMode.Converting)
            {
                using (var memory = new MemoryStream())
                {
                    this.m_CaptureState.CaptureBitmap.Save(memory, ImageFormat.Png);

                    memory.Seek(0, SeekOrigin.Begin);
                    var bytes = new byte[(int)memory.Length];
                    memory.Read(bytes, 0, (int)memory.Length);

                    this.m_CaptureState.CaptureBitmap.Dispose();
                    this.m_CaptureState.CaptureBitmap = null;

                    this.m_CaptureState.Mode = CaptureStateMode.Actioning;
                    this.m_CaptureState.ActionThread = new Thread(() => 
                        {
                            try
                            {
                                this.m_CaptureState.Action(bytes);
                                this.m_CaptureState = null;
                            }
                            catch (Exception ex)
                            {
                                // We can't upload the screenshot, so report the exception.
                                CrashReport.CrashReporter.Record(ex);
                                this.m_CaptureState = null;
                            }
                        });
                    this.m_CaptureState.ActionThread.IsBackground = true;
                    this.m_CaptureState.ActionThread.Start();
                }
            }
        }

        private bool ProcessCaptureFramePixel()
        {
            var x = this.m_CaptureState.CapturedFrameConversionX;
            var y = this.m_CaptureState.CapturedFrameConversionY;

            var color = System.Drawing.Color.FromArgb(
                this.m_CaptureState.CaptureBuffer[x + (y * this.m_CaptureState.CaptureBitmap.Width)].A,
                this.m_CaptureState.CaptureBuffer[x + (y * this.m_CaptureState.CaptureBitmap.Width)].R,
                this.m_CaptureState.CaptureBuffer[x + (y * this.m_CaptureState.CaptureBitmap.Width)].G,
                this.m_CaptureState.CaptureBuffer[x + (y * this.m_CaptureState.CaptureBitmap.Width)].B);
            this.m_CaptureState.CaptureBitmap.SetPixel(x, y, color);

            this.m_CaptureState.CapturedFrameConversionX++;

            if (this.m_CaptureState.CapturedFrameConversionX >= this.m_CaptureState.CaptureBitmap.Width)
            {
                this.m_CaptureState.CapturedFrameConversionX = 0;
                this.m_CaptureState.CapturedFrameConversionY++;
            }

            if (this.m_CaptureState.CapturedFrameConversionY >= this.m_CaptureState.CaptureBitmap.Height)
            {
                this.m_CaptureState.Mode = CaptureStateMode.Converted;
                return true;
            }

            return false;
        }

        private class CaptureState
        {
            public CaptureStateMode Mode { get; set; }

            public RenderTarget2D RenderTarget { get; set; }

            public int CapturedFrameConversionX { get; set; }
            public int CapturedFrameConversionY { get; set; }

            public Action<byte[]> Action { get; set; }
            public Thread ActionThread { get; set; }

            public Microsoft.Xna.Framework.Color[] CaptureBuffer { get; set; }
            public Bitmap CaptureBitmap { get; set; }

            public int UploadTick { get; set; }
        }
    }
}
