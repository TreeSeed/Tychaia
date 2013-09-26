// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Game
{
    public class Being
    {
        public Inventory Inventory { get; private set; }

        public Being()
        {
            this.Inventory = new Inventory();
        }
    }
}

