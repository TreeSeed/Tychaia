// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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
    }
}
