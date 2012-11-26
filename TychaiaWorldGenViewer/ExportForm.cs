using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TychaiaWorldGenViewer.Flow;
using Tychaia.ProceduralGeneration;
using System.Drawing.Imaging;

namespace TychaiaWorldGenViewer
{
    public partial class ExportForm : Form
    {
        private Layer m_Layer;
        private int m_X = 0;
        private int m_Y = 0;
        private Bitmap m_Bitmap;

        public ExportForm(Flow.FlowElement flowElement)
        {
            InitializeComponent();

            LayerFlowElement lfw = flowElement as LayerFlowElement;
            this.m_Layer = lfw.Layer;
            this.m_Bitmap = new Bitmap(1024, 1024);
            this.c_RenderBox.Image = this.m_Bitmap;
            this.c_Timer.Start();
        }

        private static SolidBrush m_UnknownAssociation = new SolidBrush(Color.FromArgb(63, 63, 63));
        private static Bitmap RenderPartial(Layer l, int sx, int sy, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, Brush> brushes = l.GetLayerColors();
            int[] data = null;
            try
            {
                data = l.GenerateData(LayerFlowImageGeneration.X + sx, LayerFlowImageGeneration.Y + sy, width, height);
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        while (true)
                        {
                            try
                            {
                                if (brushes != null && brushes.ContainsKey(data[x + y * width]))
                                    g.FillRectangle(
                                        brushes[data[x + y * (width)]],
                                        new Rectangle(x, y, 1, 1)
                                        );
                                else
                                    g.FillRectangle(
                                        m_UnknownAssociation,
                                        new Rectangle(x, y, 1, 1)
                                        );
                                break;
                            }
                            catch (InvalidOperationException)
                            {
                                // Graphics can be in use elsewhere, but we don't care; just try again.
                            }
                        }
                    }
            }
            catch (Exception)
            {
            }
            return b;
        }

        private void c_Timer_Tick(object sender, EventArgs e)
        {
            Bitmap temp = RenderPartial(this.m_Layer, this.m_X, this.m_Y, 32, 32);
            Graphics g = Graphics.FromImage(this.m_Bitmap);
            g.DrawImage(temp, this.m_X, this.m_Y);
            temp.Dispose();

            this.m_X += 32;
            if (this.m_X == 1024)
            {
                this.m_X = 0;
                this.m_Y += 32;
            }
            if (this.m_Y == 1024)
            {
                this.c_Timer.Stop();
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG File|*.png";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.m_Bitmap.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show("Layer render exported to file successfully.");
                }
            }

            this.c_RenderBox.Refresh();
        }
    }
}
