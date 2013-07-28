using System;
using System.Drawing;
using System.Windows.Forms;
using Tychaia.ProceduralGeneration;
using System.Drawing.Imaging;
using Tychaia.Globals;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration.Flow;
using Ninject;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class ExportForm : Form
    {
        private IRenderingLocationProvider m_RenderingLocationProvider;
        private RuntimeLayer m_Layer;
        private int m_X = 0;
        private int m_Y = 0;
        private int m_Z = 0;
        private Bitmap m_Bitmap;

        public ExportForm(
            IRenderingLocationProvider renderingLocationProvider,
            FlowElement flowElement)
        {
            InitializeComponent();

            this.m_RenderingLocationProvider = renderingLocationProvider;
            this.m_Layer = StorageAccess.ToRuntime((flowElement as AlgorithmFlowElement).Layer);
            this.m_Bitmap = new Bitmap(1024 + 32, 1024 + 256);
            this.c_RenderBox.Image = this.m_Bitmap;
            this.c_Timer.Start();
        }

        #region 3D Rendering

        #region Cell Render Ordering

        private static int[][] m_CellRenderOrder = new int[4][]
            {
                null,
                null,
                null,
                null
            };
        private const int RenderToNE = 0;
        private const int RenderToNW = 1;
        private const int RenderToSE = 2;
        private const int RenderToSW = 3;

        private static int[] CalculateCellRenderOrder(int targetDir, int width, int height)
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

            var result = new int[width * height];
            var count = 0;
            var start = 0;
            var maxx = width - 1;
            var maxy = height - 1;
            var last = maxx + maxy;
            int x, y;

            for (var atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                {
                    x = 0;
                    y = atk;
                }
                else
                {
                    x = atk - maxy;
                    y = maxy;
                }
                while (y > atk / 2)
                    result[count++] = y-- * width + x++;

                // Attack from the right.
                if (atk < maxx)
                {
                    x = atk;
                    y = 0;
                }
                else
                {
                    x = maxx;
                    y = atk - maxx;
                }
                while (y <= atk / 2)
                    result[count++] = y++ * width + x--;
            }

            return result;
        }

        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height)
        {
            if (m_CellRenderOrder[cameraDirection] == null)
                m_CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height);
            return m_CellRenderOrder[cameraDirection];
        }

        #endregion

        private Bitmap RenderPartial3D(RuntimeLayer layer, int sx, int sy, int sz, int width, int height, int depth)
        {
            var bitmap = new Bitmap(width * 2, height * 3);
            var graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            int[] data;
            try
            {
                int computations;
                data = layer.GenerateData(
                    this.m_RenderingLocationProvider.X + sx,
                    this.m_RenderingLocationProvider.Y + sy,
                    this.m_RenderingLocationProvider.Z + sz,
                    width, height, depth, out computations);

                var render = GetCellRenderOrder(RenderToNE, width, height);
                var ztop = layer.Algorithm.Is2DOnly ? 1 : depth;
                var zbottom = 0;
                for (var z = zbottom; z < ztop; z++)
                {
                    var rcx = width / 2 - 1 + 16;
                    var rcy = height / 2 - 15 + 32;
                    var rw = 2;
                    var rh = 1;
                    for (var i = 0; i < render.Length; i++)
                    {
                        // Calculate the X / Y of the tile in the grid.
                        var x = render[i] % width;
                        var y = render[i] / width;

                        // Calculate the render position on screen.
                        var rx = rcx + (int)((x - y) / 2.0 * rw);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                        var ry = rcy + (x + y) * rh - (rh / 2 * (width + height)) - (z - zbottom) * 1;

                        while (true)
                        {
                            try
                            {
                                Color lc;
                                if (layer.GetInputs().Length > 0)
                                    lc = layer.Algorithm.GetColorForValue(
                                    StorageAccess.FromRuntime(layer.GetInputs()[0]),
                                    data[x + y * width + z * width * height]);
                                else
                                    lc = layer.Algorithm.GetColorForValue(
                                        null,
                                        data[x + y * width + z * width * height]);
                                var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                graphics.FillRectangle(
                                    sb,
                                    new Rectangle(rx, ry, rw, rh)
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
            }
            catch (Exception)
            {
            }

            return bitmap;
        }

        #endregion

        private void c_Timer_Tick(object sender, EventArgs e)
        {
            Bitmap temp = this.RenderPartial3D(this.m_Layer, this.m_X, this.m_Y, this.m_Z, 32, 32, 32);
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
            if (this.m_Z >= 192 || (this.m_Layer.Algorithm.Is2DOnly && this.m_Z >= 32))
            {
                this.c_Timer.Stop();
                var sfd = new SaveFileDialog();
                sfd.Filter = "PNG File|*.png";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.m_Bitmap.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show("Layer render exported to file successfully.");
                }
            }

            double count = (this.m_X / 32) + (this.m_Y / 32) * (512 / 32) + (this.m_Z / 32) * (512 / 32) * (512 / 32);
            double total = (512 / 32) * (512 / 32) * ((this.m_Layer.Algorithm.Is2DOnly ? 32 : 192) / 32);
            this.Text = "Export Layer (" + Math.Round(count / total * 100.0, 2) + "% complete)";

            this.c_RenderBox.Refresh();
        }
    }
}
