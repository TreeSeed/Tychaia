// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.ProceduralGeneration
{
    public interface IRuntimeContext
    {
        /// <summary>
        /// Gets the seed.
        /// </summary>
        /// <value>The seed.</value>
        long Seed { get; }

        /// <summary>
        /// The modifier used by algorithms as an additional input to the
        /// random function calls.
        /// </summary>
        long Modifier { get; }

        /// <summary>
        /// This is the Asset Manager located in Protogame.
        /// This lets us look up a list of all the types of assets.
        /// </summary>
        IAssetManager AssetManager { get; }
    }
}
