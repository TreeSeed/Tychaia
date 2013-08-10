// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Drawing;

namespace Tychaia.Puzzle
{
    public class TestPuzzle : IPuzzle
    {
        private Color m_Color = Color.Black;

        public void DrawUI(IPuzzleUI ui)
        {
            ui.BeginUI();

            ui.SetColor(this.m_Color);
            ui.DrawRectangle(0, 0, 110, 200);
            ui.DrawText(20, 20, "hello");

            ui.EndUI();
        }

        public void ClickLeft(int x, int y)
        {
            this.m_Color = Color.Red;
        }

        public void ClickRight(int x, int y)
        {
            this.m_Color = Color.Blue;
        }

        public bool Completed
        {
            get { return false; }
        }
    }
}