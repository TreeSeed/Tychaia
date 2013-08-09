// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.Puzzle
{
    public interface IPuzzle
    {
        bool Completed { get; }

        void DrawUI(IPuzzleUI ui);
        void ClickLeft(int x, int y);
        void ClickRight(int x, int y);
    }
}