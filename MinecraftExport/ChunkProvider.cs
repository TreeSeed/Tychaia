// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Blocks;

namespace MinecraftExport
{
    public class ChunkProvider
    {
        private readonly IGenerator m_ExportGenerator;

        public ChunkProvider(
            IGeneratorResolver generatorResolver)
        {
            this.m_ExportGenerator = generatorResolver.GetGeneratorForExport();
        }

        public BlockInfo[] GetData(int x, int y, int z)
        {
            int computations;
            return this.m_ExportGenerator.GenerateData(
                x,
                y,
                z,
                16,
                16,
                256,
                out computations);
        }
    }
}