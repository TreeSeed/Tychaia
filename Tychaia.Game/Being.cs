// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Game
{
    public class Being
    {
        public Being()
        {
            this.Inventory = new Inventory();
        }
        
        public Inventory Inventory { get; private set; }
        
        /// <summary>
        /// The current health of the being.
        /// </summary>
        public int Health { get; set; }
        
        /// <summary>
        /// The maximum health of the being.
        /// </summary>
        public int MaxHealth { get; set; }
        
        /// <summary>
        /// The current stamina of the being.
        /// </summary>
        public int Stamina { get; set; }
        
        /// <summary>
        /// The maximum stamina of the being.
        /// </summary>
        public int MaxStamina { get; set; }
        
        /// <summary>
        /// The current mana of the being.
        /// </summary>
        public int Mana { get; set; }
        
        /// <summary>
        /// The maximum mana of the being.
        /// </summary>
        public int MaxMana { get; set; }
    }
}
