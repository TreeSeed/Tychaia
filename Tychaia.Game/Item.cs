// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Dx.Runtime;

namespace Tychaia.Game
{
    public class Item
    {
        [Synchronised]
        public string Name { get; set; }

        public virtual float GetNumericWeight()
        {
            return 0;
        }
    }
}
