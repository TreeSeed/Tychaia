//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Drawing;

namespace Tychaia.Puzzle
{
    public interface IPuzzleUI
    {
        void BeginUI();
        void EndUI();
        void SetColor(Color color);
        void DrawLine(int x1, int y1, int x2, int y2);
        void DrawRectangle(int x1, int y1, int x2, int y2);
        void FillRectangle(int x1, int y1, int x2, int y2);
        void DrawCircle(int x, int y, int radius);
        void DrawText(int x, int y, string text);
    }
}

