// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NGif;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class TraceForm : Form
    {
        private readonly IAlgorithmTraceImageGeneration m_AlgorithmTraceImageGeneration;

        private readonly RuntimeLayer m_Layer;
        private List<Bitmap> m_Bitmaps;
        private Action m_DisableHandlers;
        private Action m_EnableHandlers;

        public TraceForm(
            IStorageAccess storageAccess,
            IAlgorithmTraceImageGeneration algorithmTraceImageGeneration,
            FlowElement flowElement,
            ICurrentWorldSeedProvider currentWorldSeedProvider)
        {
            this.m_AlgorithmTraceImageGeneration = algorithmTraceImageGeneration;

            this.InitializeComponent();
            this.m_Layer = storageAccess.ToRuntime(((AlgorithmFlowElement)flowElement).Layer);
            this.m_Layer.SetSeed(currentWorldSeedProvider.Seed);
            this.c_FormZoomSize.Items.Add(new ZoomLevel { Level = 1 });
            this.c_FormZoomSize.Items.Add(new ZoomLevel { Level = 2 });
            this.c_FormZoomSize.SelectedIndex = 0;
        }

        private void c_PerformTrace_Click(object sender, EventArgs e)
        {
            const int s = 64;
            const int o = 10000000;

            this.c_PerformTrace.Enabled = false;
            this.c_OnlyComparisonDataCheckBox.Enabled = false;
            this.c_GenerateGIF.Enabled = false;
            this.ControlBox = false;

            this.m_EnableHandlers =
                () => PerformOperationRecursively(
                    v => v.DataGenerated += this.HandleDataGenerated, this.m_Layer);
            this.m_DisableHandlers =
                () => PerformOperationRecursively(
                    v => v.DataGenerated -= this.HandleDataGenerated, this.m_Layer);

            this.c_TraceProgress.Maximum = 0;
            PerformOperationRecursively(
                v => this.c_TraceProgress.Maximum += this.c_OnlyComparisonDataCheckBox.Checked ? 1 : 2,
                this.m_Layer);
            this.m_Bitmaps = new List<Bitmap>();
            this.m_EnableHandlers();
            var thread = new Thread(() =>
            {
                int computations;
                this.m_Layer.GenerateData(
                    -s + o,
                    -s + o,
                    -s + o,
                    s * 2,
                    s * 2,
                    this.m_Layer.Algorithm.Is2DOnly ? 1 : s * 2,
                    out computations);
                this.m_DisableHandlers();

                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.SuspendLayout();
                        this.c_TraceImage.Enabled = true;
                        this.c_TraceScrollbar.Enabled = true;
                        this.c_TraceProgress.Value = 0;
                        this.c_TraceProgress.Enabled = false;
                        this.ControlBox = true;
                        if (this.m_Bitmaps.Count == 0)
                        {
                            this.ResumeLayout();
                            MessageBox.Show(@"No images were generated from the trace, now closing..");
                            this.Close();
                            return;
                        }
                        this.c_TraceImage.Image = this.m_Bitmaps[0];
                        this.c_TraceScrollbar.Minimum = 0;
                        this.c_TraceScrollbar.Maximum = this.m_Bitmaps.Count - 1;
                        this.c_TraceScrollbar.Value = 0;
                        this.ResumeLayout();

                        this.Rescale(this.m_Bitmaps[0]);

                        // Generate GIF if desired.
                        if (this.c_GenerateGIF.Checked)
                        {
                            using (var sfd = new SaveFileDialog())
                            {
                                sfd.Title = @"Save GIF";
                                sfd.Filter = @"Animated GIFs|*.gif";
                                sfd.RestoreDirectory = true;
                                sfd.AddExtension = true;
                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    var encoder = new AnimatedGifEncoder();
                                    encoder.Start(sfd.FileName);
                                    encoder.SetDelay(1000);
                                    encoder.SetRepeat(0);
                                    foreach (var image in this.m_Bitmaps)
                                        encoder.AddFrame(image);
                                    encoder.Finish();
                                }
                            }
                        }
                    }));
                }
            }) { IsBackground = true };
            thread.Start();
        }

        private static void PerformOperationRecursively(Action<RuntimeLayer> operation, RuntimeLayer layer)
        {
            operation(layer);
            foreach (var input in layer.GetInputs().Where(input => input != null))
                PerformOperationRecursively(operation, input);
        }

        private void HandleDataGenerated(object sender, DataGeneratedEventArgs e)
        {
            // Save the internal result.
            if (!this.c_OnlyComparisonDataCheckBox.Checked)
            {
                this.m_Bitmaps.Add(this.m_AlgorithmTraceImageGeneration.RenderTraceResult(
                    sender as RuntimeLayer,
                    e.Data,
                    e.GSArrayWidth,
                    e.GSArrayHeight,
                    e.GSArrayDepth));
                Console.WriteLine(this.m_Bitmaps[this.m_Bitmaps.Count - 1].Size);

                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.c_TraceProgress.Value += 1;
                        this.Text = @"Trace Algorithm (" + (this.c_TraceProgress.Maximum - this.c_TraceProgress.Value) +
                                    @" operations remaining)";
                    }));
                }
            }

            // Save the normal result.
            int computations;
            this.m_DisableHandlers();
            var data = ((RuntimeLayer)sender)
                .GenerateData(e.GSAbsoluteX - e.GSMaxOffsetX,
                    e.GSAbsoluteY - e.GSMaxOffsetY,
                    e.GSAbsoluteZ - e.GSMaxOffsetZ,
                    e.GSArrayWidth,
                    e.GSArrayHeight,
                    e.GSArrayDepth, out computations);
            this.m_Bitmaps.Add(this.m_AlgorithmTraceImageGeneration.RenderTraceResult(
                sender as RuntimeLayer,
                data,
                e.GSArrayWidth,
                e.GSArrayHeight,
                e.GSArrayDepth));
            Console.WriteLine(this.m_Bitmaps[this.m_Bitmaps.Count - 1].Size);
            this.m_EnableHandlers();

            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    this.c_TraceProgress.Value += 1;
                    this.Text = @"Trace Algorithm (" + (this.c_TraceProgress.Maximum - this.c_TraceProgress.Value) +
                                @" operations remaining)";
                }));
            }
        }

        private void c_TraceScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.m_Bitmaps == null ||
                e.NewValue < 0 ||
                e.NewValue >= this.m_Bitmaps.Count)
                return;
            var zoomLevel = (ZoomLevel)this.c_FormZoomSize.SelectedItem;
            if (zoomLevel.Level == 1)
            {
                this.c_TraceImage.Image = this.m_Bitmaps[e.NewValue];
                this.Rescale(this.m_Bitmaps[e.NewValue]);
                return;
            }

            // We need to scale the image.
            var image = new Bitmap(
                this.m_Bitmaps[e.NewValue].Width * zoomLevel.Level,
                this.m_Bitmaps[e.NewValue].Height * zoomLevel.Level
                );
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.DrawImage(
                    this.m_Bitmaps[e.NewValue],
                    new Rectangle(0, 0, image.Width, image.Height),
                    // destination rectangle
                    0,
                    0, // upper-left corner of source rectangle
                    this.m_Bitmaps[e.NewValue].Width, // width of source rectangle
                    this.m_Bitmaps[e.NewValue].Height, // height of source rectangle
                    GraphicsUnit.Pixel);
            }
            this.c_TraceImage.Image = image;
            this.Rescale(image);
        }

        private void Rescale(Bitmap bitmap)
        {
            var basew = 427 - 384;
            var baseh = 570 - 384;

            if (this.c_TraceImage.Image != null)
            {
                this.Size = new Size(basew, baseh) + bitmap.Size;
            }
        }

        private void c_FormZoomSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.c_TraceScrollbar_Scroll(this,
                new ScrollEventArgs(ScrollEventType.EndScroll, this.c_TraceScrollbar.Value));
        }

        private class ZoomLevel
        {
            public int Level;

            public override string ToString()
            {
                return this.Level + "x Zoom";
            }
        }
    }
}
