// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Xunit;

namespace Tychaia.Globals.Tests
{
    public class TranslationTests
    {
        [Fact]
        public void LibraryTest()
        {
            var sizePolicy = new DefaultChunkSizePolicy();
            var translation = new DefaultPositionScaleTranslation(sizePolicy);
            var width = sizePolicy.CellVoxelWidth * sizePolicy.ChunkCellWidth;

            Assert.Equal(-2, translation.Translate(-(width + 1)));
            Assert.Equal(-1, translation.Translate(-width));
            Assert.Equal(-1, translation.Translate(-(width - 1)));
            Assert.Equal(-1, translation.Translate(-1));
            Assert.Equal(0, translation.Translate(0));
            Assert.Equal(0, translation.Translate(1));
            Assert.Equal(0, translation.Translate(width - 1));
            Assert.Equal(1, translation.Translate(width));
            Assert.Equal(1, translation.Translate(width + 1));
        }
    }
}
