// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class ActualProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IGeneratorResolver m_GeneratorResolver;

        public ActualProceduralConfiguration(
            IGeneratorResolver generatorResolver)
        {
            this.m_GeneratorResolver = generatorResolver;
        }

        public IGenerator GetConfiguration()
        {
            return this.m_GeneratorResolver.GetGeneratorForGame();
        }
    }
}

