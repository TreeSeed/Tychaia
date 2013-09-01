// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.ProceduralGeneration
{
    public class GenerationRegion
    {
        public long X;
        public long Y;
        public long Z;
        public int Width;
        public int Height;
        public int Depth;

        public dynamic GeneratedData;

        public override string ToString()
        {
            return this.X + " " +
                   this.Y + " " +
                   this.Z + " " +
                   this.Width + " " +
                   this.Height + " " +
                   this.Depth;
        }
    }
}

