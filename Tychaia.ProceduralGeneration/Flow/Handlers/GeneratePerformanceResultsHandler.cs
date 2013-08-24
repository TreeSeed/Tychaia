// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration.Flow.Handlers
{
    public class GeneratePerformanceResultsHandler
    {
        private ICurrentWorldSeedProvider m_CurrentWorldSeedProvider;
        private IRenderingLocationProvider m_RenderingLocationProvider;

        public GeneratePerformanceResultsHandler(
            ICurrentWorldSeedProvider currentWorldSeedProvider,
            IRenderingLocationProvider renderingLocationProvider)
        {
            this.m_CurrentWorldSeedProvider = currentWorldSeedProvider;
            this.m_RenderingLocationProvider = renderingLocationProvider;
        }

        public void Handle(StorageLayer layer, Action<FlowProcessingResponse> put)
        {
            HandlerHelper.SendStartMessage(
                "Performance\r\ntesting...",
                FlowProcessingRequestType.GenerateRuntimeBitmap,
                layer,
                put);

            // Settings.
            var testTime = 50;
            var warningLimit = 5000; // 5ms
            var badLimit = 16000; // 16ms

            // Perform conversions.
            var runtime = StorageAccess.ToRuntime(layer);
            runtime.SetSeed(this.m_CurrentWorldSeedProvider.Seed);
            IGenerator compiled = null;
            try
            {
                compiled = StorageAccess.ToCompiled(runtime);
            }
            catch (Exception)
            {
                // Failed to compile layer.
            }

            // First check how many iterations of 8x8x8 generation the runtime layer can do in the test time.
            var runtimeStart = DateTime.Now;
            var runtimeComputations = 0;
            var iterations = 0;
            while ((DateTime.Now - runtimeStart).TotalMilliseconds < testTime)
            {
                runtime.GenerateData(0, 0, 0, 32, 32, 32, out runtimeComputations);
                iterations++;
            }
            var runtimeEnd = DateTime.Now;

            // Now check how long it takes the compiled layer to do as many iterations of generating 8x8x8.
            var compiledStart = DateTime.Now;
            var compiledComputations = 0;
            if (compiled != null)
            {
                try
                {
                    for (var i = 0; i < iterations; i++)
                        compiled.GenerateData(0, 0, 0, 32, 32, 32, out compiledComputations);
                }
                catch
                {
                    compiled = null;
                }
            }
            var compiledEnd = DateTime.Now;

            // Determine the per-operation cost.
            var runtimeCost = runtimeEnd - runtimeStart;
            var compiledCost = compiledEnd - compiledStart;
            var runtimeus = Math.Round((runtimeCost.TotalMilliseconds / iterations) * 1000, 0); // Microseconds.
            var compiledus = Math.Round((compiledCost.TotalMilliseconds / iterations) * 1000, 0);

            // Define colors and determine values.
            var okay = new SolidBrush(Color.LightGreen);
            var warning = new SolidBrush(Color.Orange);
            var bad = new SolidBrush(Color.IndianRed);
            var runtimeColor = okay;
            var compiledColor = okay;
            if (runtimeus > warningLimit)
                runtimeColor = warning;
            if (compiledus > warningLimit)
                compiledColor = warning;
            if (runtimeus > badLimit)
                runtimeColor = bad;
            if (compiledus > badLimit)
                compiledColor = bad;

            // Draw performance measurements.
            Bitmap bitmap;
            if (runtimeComputations != compiledComputations && compiled != null)
                bitmap = new Bitmap(128, 48);
            else
                bitmap = new Bitmap(128, 32);
            var graphics = Graphics.FromImage(bitmap);
            var font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            graphics.Clear(Color.Black);
            if (runtimeComputations != compiledComputations && compiled != null)
            {
                graphics.DrawString("Computation mismatch!", font, bad, new PointF(0, 0));
                graphics.DrawString("Runtime:", font, runtimeColor, new PointF(0, 16));
                graphics.DrawString(runtimeComputations + "c", font, runtimeColor, new PointF(70, 16));
                if (compiled != null)
                {
                    graphics.DrawString("Compiled:", font, compiledColor, new PointF(0, 32));
                    graphics.DrawString(compiledComputations + "c", font, compiledColor, new PointF(70, 32));
                }
                else
                    graphics.DrawString("Unable to compile.", font, bad, new PointF(0, 32));
            }
            else
            {
                graphics.DrawString("Runtime:", font, runtimeColor, new PointF(0, 0));
                graphics.DrawString(runtimeus + "\xB5s", font, runtimeColor, new PointF(70, 0));
                if (compiled != null)
                {
                    graphics.DrawString("Compiled:", font, compiledColor, new PointF(0, 16));
                    graphics.DrawString(compiledus + "\xB5s", font, compiledColor, new PointF(70, 16));
                }
                else
                    graphics.DrawString("Unable to compile.", font, bad, new PointF(0, 16));
            }
            var additionalInformation = bitmap;
            Bitmap compiledBitmap = null;

            // Use the compiled layer to re-render the output.
            if (compiled != null)
            {
                compiledBitmap = AlgorithmFlowImageGeneration.RegenerateImageForLayer(
                    layer,
                    this.m_CurrentWorldSeedProvider.Seed,
                    this.m_RenderingLocationProvider.X,
                    this.m_RenderingLocationProvider.Y,
                    this.m_RenderingLocationProvider.Z,
                    64, 64, 64, true);
            }

            put(new FlowProcessingResponse
            {
                RequestType = FlowProcessingRequestType.GeneratePerformanceResults,
                IsStartNotification = false,
                Results = new object[] { layer, additionalInformation, compiledBitmap }
            });
        }
    }
}
