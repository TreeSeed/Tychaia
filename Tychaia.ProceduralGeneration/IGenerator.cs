// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.ProceduralGeneration
{
    public interface IGenerator
    {
        /// <summary>
        /// The world seed.
        /// </summary>
        long Seed { get; }

        /// <summary>
        /// Sets the seed of this layer and all of it's input layers
        /// recursively.
        /// </summary>
        /// <param name="seed">Seed.</param>
        void SetSeed(long seed);

        /// <summary>
        /// Generates data using the current algorithm.
        /// </summary>
        dynamic GenerateData(long x, long y, long z, int width, int height, int depth, out int computations);

        /// <summary>
        /// Occurs when data has been generated for an algorithm.
        /// </summary>
        event DataGeneratedEventHandler DataGenerated;
    }
}