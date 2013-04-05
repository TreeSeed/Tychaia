//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Windows.Forms;
using System.Drawing;
using Tychaia.Puzzle;
using System;

namespace TychaiaPuzzleEditor
{
    public class PuzzleEditorForm : Form, IPuzzleUI
    {
        private Timer m_Timer;
        private ComboBox m_PuzzleList;
        private IPuzzle m_Puzzle;

        public PuzzleEditorForm()
        {
            this.SuspendLayout();
            
            this.ClientSize = new Size(800, 600);
            this.m_PuzzleList = new ComboBox();
            this.InitializePuzzleList();
            this.Controls.Add(this.m_PuzzleList);
            this.m_Timer = new Timer();
            this.m_Timer.Interval = 1000 / 60;
            this.m_Timer.Tick += (sender, e) =>
            {
                this.Invalidate();
            };
            this.m_Timer.Start();

            this.ResumeLayout();
        }

        void InitializePuzzleList()
        {
            this.m_PuzzleList.Size = new Size(350, 24);
            this.m_PuzzleList.Location = new Point(this.Width - this.m_PuzzleList.Width - 20, this.Height - this.m_PuzzleList.Height - 40);
            this.m_PuzzleList.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.m_PuzzleList.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPuzzle).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        this.m_PuzzleList.Items.Add(type);
                    }
                }
            }

            this.m_PuzzleList.SelectedValueChanged += (sender, e) => 
            {
                if (this.m_PuzzleList.SelectedItem == null)
                    this.m_Puzzle = null;
                else
                    this.m_Puzzle = Activator.CreateInstance(this.m_PuzzleList.SelectedItem as Type) as IPuzzle;
            };
            this.m_PuzzleList.SelectedIndex = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.m_ActiveGraphics = e.Graphics;
            if (this.m_Puzzle != null)
                this.m_Puzzle.DrawUI(this);
            else
            {
                this.BeginUI();
                this.EndUI();
            }
            this.m_ActiveGraphics = null;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (this.m_Puzzle == null)
                return;

            if (e.Button == MouseButtons.Left)
                this.m_Puzzle.ClickLeft(e.X, e.Y);
            else if (e.Button == MouseButtons.Right)
                this.m_Puzzle.ClickRight(e.X, e.Y);
        }

        #region IPuzzleUI implementation
        
        private Color m_ActiveColor = Color.Black;
        private Graphics m_ActiveGraphics = null;

        public void BeginUI()
        {
            this.m_ActiveGraphics.Clear(Color.White);
        }

        public void EndUI()
        {
        }

        public void SetColor(Color color)
        {
            this.m_ActiveColor = color;
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var pen = new Pen(new SolidBrush(this.m_ActiveColor));
            this.m_ActiveGraphics.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2)
        {
            var pen = new Pen(new SolidBrush(this.m_ActiveColor));
            this.m_ActiveGraphics.DrawRectangle(pen, new Rectangle(x1, y1, x2 - x1, y2 - y1));
        }

        public void FillRectangle(int x1, int y1, int x2, int y2)
        {
            var brush = new SolidBrush(this.m_ActiveColor);
            this.m_ActiveGraphics.FillRectangle(brush, new Rectangle(x1, y1, x2 - x1, y2 - y1));
        }

        public void DrawCircle(int x, int y, int radius)
        {
            var pen = new Pen(new SolidBrush(this.m_ActiveColor));
            this.m_ActiveGraphics.DrawEllipse(pen, new Rectangle(x - radius, y - radius, radius * 2, radius * 2));
        }

        public void DrawText(int x, int y, string text)
        {
            var brush = new SolidBrush(this.m_ActiveColor);
            this.m_ActiveGraphics.DrawString(text, SystemFonts.DefaultFont, brush, new RectangleF(x, y, x + 1000, y + 32));
        }

        #endregion
    }
}

