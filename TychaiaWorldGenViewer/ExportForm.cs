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
        private int m_Z = 0;
        private Bitmap m_Bitmap;

        public ExportForm(Flow.FlowElement flowElement)
        {
            InitializeComponent();

            LayerFlowElement lfw = flowElement as LayerFlowElement;
            this.m_Layer = lfw.Layer;
            if (this.m_Layer is Layer2D)
                this.m_Bitmap = new Bitmap(1024, 1024);
            else
            {
                this.m_Bitmap = new Bitmap(1024 + 32, 1024 + 256);
            }
            this.c_RenderBox.Image = this.m_Bitmap;
            this.c_Timer.Start();
        }

        private static SolidBrush m_UnknownAssociation2D = new SolidBrush(Color.FromArgb(63, 63, 63));
        private static SolidBrush m_UnknownAssociation3D = new SolidBrush(Color.FromArgb(0, 0, 0, 0));

        #region 2D Rendering

        private static Bitmap RenderPartial2D(Layer l, int sx, int sy, int width, int height)
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
                                        m_UnknownAssociation2D,
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

        #endregion

        #region 3D Rendering

        #region Cell Render Ordering

        private static int[][] CellRenderOrder = new int[4][] { null, null, null, null };
        private const int RenderToNE = 0;
        private const int RenderToNW = 1;
        private const int RenderToSE = 2;
        private const int RenderToSW = 3;

        private static int[] CalculateCellRenderOrder(int targetDir, int width, int height, int depth)
        {
            /*               North
             *        0  1  2  3  4  5  6 
             *        1  2  3  4  5  6  7 
             *        2  3  4  5  6  7  8
             *  East  3  4  5  6  7  8  9  West
             *        4  5  6  7  8  9  10
             *        5  6  7  8  9  10 11
             *        6  7  8  9  10 11 12
             *               South
             *  
             * Start value is always 0.
             * Last value is (MaxX + MaxY).
             * This is the AtkValue.
             * 
             * We attack from the left side of the render first
             * with (X: 0, Y: AtkValue) until Y would be less than
             * half of AtkValue.
             * 
             * We then attack from the right side of the render
             * with (X: AtkValue, Y: 0) until X would be less than
             * half of AtkValue - 1.
             * 
             * If we are attacking from the left, but Y is now
             * greater than MaxY, then we are over half-way and are
             * now starting at the bottom of the grid.
             * 
             * In this case, we start with (X: AtkValue - MaxY, Y: MaxY)
             * and continue until we reach the same conditions that
             * apply normally.  The same method applies to the right hand
             * side where we start with (X: MaxX, Y: AtkValue - MaxX).
             *
             */

            if (targetDir != RenderToNE)
                throw new InvalidOperationException();

            int[] result = new int[width * height];
            int count = 0;
            int start = 0;
            int maxx = width - 1;
            int maxy = height - 1;
            int last = maxx + maxy;
            int x, y;

            for (int atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                { x = 0; y = atk; }
                else
                { x = atk - maxy; y = maxy; }
                while (y > atk / 2)
                    result[count++] = y-- * width + x++;

                // Attack from the right.
                if (atk < maxx)
                { x = atk; y = 0; }
                else
                { x = maxx; y = atk - maxx; }
                while (y <= atk / 2)
                    result[count++] = y++ * width + x--;
            }

            return result;
        }

        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height, int depth)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height, depth);
            return CellRenderOrder[cameraDirection];
        }

        #endregion
        
        private static Bitmap RenderPartial3D(Layer l, int sx, int sy, int sz, int width, int height, int depth)
        {
            Bitmap b = new Bitmap(width * 2, height * 3);
            Graphics g = Graphics.FromImage(b);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, Brush> brushes = l.GetLayerColors();
            int[] data = null;
            try
            {
                data = l.GenerateData(LayerFlowImageGeneration.X + sx, LayerFlowImageGeneration.Y + sy, LayerFlowImageGeneration.Z + sz, width, height, depth);

                int[] render = GetCellRenderOrder(RenderToNE, width, height, depth);
                int ztop = depth;
                int zbottom = 0;
                for (int z = zbottom; z < ztop; z++)
                {
                    int rcx = width / 2 - 1 + 16;
                    int rcy = height / 2 - 15 + 32;
                    int rw = 2;
                    int rh = 1;
                    for (int i = 0; i < render.Length; i++)
                    {
                        // Calculate the X / Y of the tile in the grid.
                        int x = render[i] % width;
                        int y = render[i] / width;

                        // Calculate the render position on screen.
                        int rx = rcx + (int)((x - y) / 2.0 * rw);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                        int ry = rcy + (x + y) * rh - (rh / 2 * (width + height)) - (z - zbottom) * 1;

                        while (true)
                        {
                            try
                            {
                                if (l.IsLayerColorsFlags())
                                {
                                    Color accum = Color.FromArgb(0, 0, 0, 0);
                                    foreach (KeyValuePair<int, System.Drawing.Brush> kv in brushes)
                                    {
                                        SolidBrush sb = kv.Value as SolidBrush;
                                        if ((data[x + y * width + z * width * height] & kv.Key) != 0)
                                        {
                                            accum = Color.FromArgb(
                                                Math.Min(255, accum.A + sb.Color.A),
                                                Math.Min((byte)255, (byte)(accum.R + sb.Color.R * (sb.Color.A / 255.0) / brushes.Count)),
                                                Math.Min((byte)255, (byte)(accum.G + sb.Color.G * (sb.Color.A / 255.0) / brushes.Count)),
                                                Math.Min((byte)255, (byte)(accum.B + sb.Color.B * (sb.Color.A / 255.0) / brushes.Count))
                                                );
                                        }
                                    }
                                    if (accum.R == 255 && accum.G == 255 && accum.B == 255)
                                        accum = Color.FromArgb(63, 0, 0, 0);
                                    g.FillRectangle(
                                        new SolidBrush(accum),
                                        new Rectangle(rx, ry, rw, rh)
                                        );
                                    break;
                                }
                                else
                                {
                                    if (brushes != null && brushes.ContainsKey(data[x + y * width + z * width * height]))
                                    {
                                        SolidBrush sb = brushes[data[x + y * width + z * width * height]] as SolidBrush;
                                        //sb.Color = Color.FromArgb(255, sb.Color);
                                        g.FillRectangle(
                                            sb,
                                            new Rectangle(rx, ry, rw, rh)
                                            );
                                    }
                                    break;
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // Graphics can be in use elsewhere, but we don't care; just try again.
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return b;
        }

        #endregion

        private void c_Timer_Tick(object sender, EventArgs e)
        {
            if (this.m_Layer is Layer2D)
            {
                Bitmap temp = RenderPartial2D(this.m_Layer, this.m_X, this.m_Y, 32, 32);
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

                double count = (this.m_X / 32) + (this.m_Y / 32) * (1024 / 32);
                double total = (1024 / 32) * (1024 / 32);
                this.Text = "Export Layer (" + Math.Round(count / total * 100.0, 2) + "% complete)";

                this.c_RenderBox.Refresh();
            }
            else if (this.m_Layer is Layer3D)
            {
                Bitmap temp = RenderPartial3D(this.m_Layer, this.m_X, this.m_Y, this.m_Z, 32, 32, 32);
                Graphics g = Graphics.FromImage(this.m_Bitmap);
                int rcx = 32 / 2 + 512 - 48 + 16;
                int rcy = 32 / 2 - 15 - 32 + 256;
                int rx = rcx + (int)((this.m_X / 32 - this.m_Y / 32) / 2.0 * 64);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                int ry = rcy + (this.m_X / 32 + this.m_Y / 32) * 32 /*- (32 / 2 * (32 + 32))*/ - (this.m_Z / 32 - 0) * 32;
                g.DrawImage(temp, rx, ry);
                temp.Dispose();

                this.m_X += 32;
                if (this.m_X >= 512)
                {
                    this.m_X = 0;
                    this.m_Y += 32;
                }
                if (this.m_Y >= 512)
                {
                    this.m_X = 0;
                    this.m_Y = 0;
                    this.m_Z += 32;
                }
                if (this.m_Z >= 192)
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

                double count = (this.m_X / 32) + (this.m_Y / 32) * (512 / 32) + (this.m_Z / 32) * (512 / 32) * (512 / 32);
                double total = (512 / 32) * (512 / 32) * (192 / 32);
                this.Text = "Export Layer (" + Math.Round(count / total * 100.0, 2) + "% complete)";

                this.c_RenderBox.Refresh();
            }
            else
            {
                this.Close();
            }
        }
    }
}
