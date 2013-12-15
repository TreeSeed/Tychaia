// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration.Compiler;
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class RangedLayerTests : TestBase
    {
        [Fact]
        public void SingleTest()
        {
            var runtime = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var ranged = new RangedLayer(runtime);
            Assert.Equal("x", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("x + width", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("width", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("0", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("width", ranged.CalculationEndI.GetText());
            Assert.Equal("height", ranged.CalculationEndJ.GetText());
            Assert.Equal("depth", ranged.CalculationEndK.GetText());
            Assert.Equal("0", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("0", ranged.OffsetX.GetText());
            Assert.Equal("0", ranged.OffsetY.GetText());
            Assert.Equal("0", ranged.OffsetZ.GetText());
        }

        [Fact]
        public void PassthroughTest()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, perlin);

            var ranged = new RangedLayer(passthrough);
            Assert.Equal("x", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("x + width", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("width", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("0", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("width", ranged.CalculationEndI.GetText());
            Assert.Equal("height", ranged.CalculationEndJ.GetText());
            Assert.Equal("depth", ranged.CalculationEndK.GetText());
            Assert.Equal("0", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("0", ranged.OffsetX.GetText());
            Assert.Equal("0", ranged.OffsetY.GetText());
            Assert.Equal("0", ranged.OffsetZ.GetText());

            ranged = ranged.Inputs[0];
            Assert.Equal("x", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("x + width", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("width", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("0", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("width", ranged.CalculationEndI.GetText());
            Assert.Equal("height", ranged.CalculationEndJ.GetText());
            Assert.Equal("depth", ranged.CalculationEndK.GetText());
            Assert.Equal("0", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("0", ranged.OffsetX.GetText());
            Assert.Equal("0", ranged.OffsetY.GetText());
            Assert.Equal("0", ranged.OffsetZ.GetText());
        }

        [Fact]
        public void XBorderTest()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough { XBorder = 2 });
            passthrough.SetInput(0, perlin);

            var ranged = new RangedLayer(passthrough);
            Assert.Equal("x", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("x + width", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("width", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("4", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("(width + 4)", ranged.CalculationEndI.GetText());
            Assert.Equal("height", ranged.CalculationEndJ.GetText());
            Assert.Equal("depth", ranged.CalculationEndK.GetText());
            Assert.Equal("-2", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("-2", ranged.OffsetX.GetText());
            Assert.Equal("0", ranged.OffsetY.GetText());
            Assert.Equal("0", ranged.OffsetZ.GetText());

            ranged = ranged.Inputs[0];
            Assert.Equal("(x - 2)", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("(x + width + 2)", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("(width + 4)", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("0", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("(width + 4)", ranged.CalculationEndI.GetText());
            Assert.Equal("height", ranged.CalculationEndJ.GetText());
            Assert.Equal("depth", ranged.CalculationEndK.GetText());
            Assert.Equal("0", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("-2", ranged.OffsetX.GetText());
            Assert.Equal("0", ranged.OffsetY.GetText());
            Assert.Equal("0", ranged.OffsetZ.GetText());
        }

        [Fact]
        public void AllBorderTest()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough { XBorder = 7, YBorder = 9, ZBorder = 11 });
            passthrough.SetInput(0, perlin);

            var ranged = new RangedLayer(passthrough);
            Assert.Equal("x", ranged.X.GetText());
            Assert.Equal("y", ranged.Y.GetText());
            Assert.Equal("z", ranged.Z.GetText());
            Assert.Equal("x + width", ranged.OuterX.GetText());
            Assert.Equal("y + height", ranged.OuterY.GetText());
            Assert.Equal("z + depth", ranged.OuterZ.GetText());
            Assert.Equal("width", ranged.Width.GetText());
            Assert.Equal("height", ranged.Height.GetText());
            Assert.Equal("depth", ranged.Depth.GetText());
            Assert.Equal("14", ranged.CalculationStartI.GetText());
            Assert.Equal("18", ranged.CalculationStartJ.GetText());
            Assert.Equal("22", ranged.CalculationStartK.GetText());
            Assert.Equal("(width + 14)", ranged.CalculationEndI.GetText());
            Assert.Equal("(height + 18)", ranged.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged.CalculationEndK.GetText());
            Assert.Equal("-7", ranged.OffsetI.GetText());
            Assert.Equal("-9", ranged.OffsetJ.GetText());
            Assert.Equal("-11", ranged.OffsetK.GetText());
            Assert.Equal("-7", ranged.OffsetX.GetText());
            Assert.Equal("-9", ranged.OffsetY.GetText());
            Assert.Equal("-11", ranged.OffsetZ.GetText());

            ranged = ranged.Inputs[0];
            Assert.Equal("(x - 7)", ranged.X.GetText());
            Assert.Equal("(y - 9)", ranged.Y.GetText());
            Assert.Equal("(z - 11)", ranged.Z.GetText());
            Assert.Equal("(x + width + 7)", ranged.OuterX.GetText());
            Assert.Equal("(y + height + 9)", ranged.OuterY.GetText());
            Assert.Equal("(z + depth + 11)", ranged.OuterZ.GetText());
            Assert.Equal("(width + 14)", ranged.Width.GetText());
            Assert.Equal("(height + 18)", ranged.Height.GetText());
            Assert.Equal("(depth + 22)", ranged.Depth.GetText());
            Assert.Equal("0", ranged.CalculationStartI.GetText());
            Assert.Equal("0", ranged.CalculationStartJ.GetText());
            Assert.Equal("0", ranged.CalculationStartK.GetText());
            Assert.Equal("(width + 14)", ranged.CalculationEndI.GetText());
            Assert.Equal("(height + 18)", ranged.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged.CalculationEndK.GetText());
            Assert.Equal("0", ranged.OffsetI.GetText());
            Assert.Equal("0", ranged.OffsetJ.GetText());
            Assert.Equal("0", ranged.OffsetK.GetText());
            Assert.Equal("-7", ranged.OffsetX.GetText());
            Assert.Equal("-9", ranged.OffsetY.GetText());
            Assert.Equal("-11", ranged.OffsetZ.GetText());
        }

        [Fact]
        public void ComplexBorderTest()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin { Layer2D = false });
            var add = this.CreateRuntimeLayer(new AlgorithmAdd { Layer2d = false });
            var perlin2 = this.CreateRuntimeLayer(new AlgorithmPerlin { Layer2D = false });
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough { XBorder = 7, YBorder = 9, ZBorder = 11, Layer2D = false });
            var heightC = this.CreateRuntimeLayer(new AlgorithmHeightChange { Layer2D = false });
            passthrough.SetInput(0, perlin2);
            add.SetInput(0, perlin);
            add.SetInput(1, passthrough);
            heightC.SetInput(0, add);

            var ranged_heightC = new RangedLayer(heightC);
            var ranged_add = ranged_heightC.Inputs[0];
            var ranged_passthrough = ranged_add.Inputs[1];
            var ranged_perlin2 = ranged_passthrough.Inputs[0];
            var ranged_perlin = ranged_add.Inputs[0];

            Assert.Equal("x", ranged_heightC.X.GetText());
            Assert.Equal("y", ranged_heightC.Y.GetText());
            Assert.Equal("z", ranged_heightC.Z.GetText());
            Assert.Equal("x + width", ranged_heightC.OuterX.GetText());
            Assert.Equal("y + height", ranged_heightC.OuterY.GetText());
            Assert.Equal("z + depth", ranged_heightC.OuterZ.GetText());
            Assert.Equal("width", ranged_heightC.Width.GetText());
            Assert.Equal("height", ranged_heightC.Height.GetText());
            Assert.Equal("depth", ranged_heightC.Depth.GetText());
            Assert.Equal("18", ranged_heightC.CalculationStartI.GetText());
            Assert.Equal("22", ranged_heightC.CalculationStartJ.GetText());
            Assert.Equal("22", ranged_heightC.CalculationStartK.GetText());
            Assert.Equal("(width + 18)", ranged_heightC.CalculationEndI.GetText());
            Assert.Equal("(height + 22)", ranged_heightC.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged_heightC.CalculationEndK.GetText());
            Assert.Equal("-9", ranged_heightC.OffsetI.GetText());
            Assert.Equal("-11", ranged_heightC.OffsetJ.GetText());
            Assert.Equal("-11", ranged_heightC.OffsetK.GetText());
            Assert.Equal("-9", ranged_heightC.OffsetX.GetText());
            Assert.Equal("-11", ranged_heightC.OffsetY.GetText());
            Assert.Equal("-11", ranged_heightC.OffsetZ.GetText());

            Assert.Equal("(x - 2)", ranged_add.X.GetText());
            Assert.Equal("(y - 2)", ranged_add.Y.GetText());
            Assert.Equal("z", ranged_add.Z.GetText());
            Assert.Equal("(x + width + 2)", ranged_add.OuterX.GetText());
            Assert.Equal("(y + height + 2)", ranged_add.OuterY.GetText());
            Assert.Equal("z + depth", ranged_add.OuterZ.GetText());
            Assert.Equal("(width + 4)", ranged_add.Width.GetText());
            Assert.Equal("(height + 4)", ranged_add.Height.GetText());
            Assert.Equal("depth", ranged_add.Depth.GetText());
            Assert.Equal("14", ranged_add.CalculationStartI.GetText());
            Assert.Equal("18", ranged_add.CalculationStartJ.GetText());
            Assert.Equal("22", ranged_add.CalculationStartK.GetText());
            Assert.Equal("(width + 18)", ranged_add.CalculationEndI.GetText());
            Assert.Equal("(height + 22)", ranged_add.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged_add.CalculationEndK.GetText());
            Assert.Equal("-7", ranged_add.OffsetI.GetText());
            Assert.Equal("-9", ranged_add.OffsetJ.GetText());
            Assert.Equal("-11", ranged_add.OffsetK.GetText());
            Assert.Equal("-9", ranged_add.OffsetX.GetText());
            Assert.Equal("-11", ranged_add.OffsetY.GetText());
            Assert.Equal("-11", ranged_add.OffsetZ.GetText());

            Assert.Equal("(x - 2)", ranged_passthrough.X.GetText());
            Assert.Equal("(y - 2)", ranged_passthrough.Y.GetText());
            Assert.Equal("z", ranged_passthrough.Z.GetText());
            Assert.Equal("(x + width + 2)", ranged_passthrough.OuterX.GetText());
            Assert.Equal("(y + height + 2)", ranged_passthrough.OuterY.GetText());
            Assert.Equal("z + depth", ranged_passthrough.OuterZ.GetText());
            Assert.Equal("(width + 4)", ranged_passthrough.Width.GetText());
            Assert.Equal("(height + 4)", ranged_passthrough.Height.GetText());
            Assert.Equal("depth", ranged_passthrough.Depth.GetText());
            Assert.Equal("14", ranged_passthrough.CalculationStartI.GetText());
            Assert.Equal("18", ranged_passthrough.CalculationStartJ.GetText());
            Assert.Equal("22", ranged_passthrough.CalculationStartK.GetText());
            Assert.Equal("(width + 18)", ranged_passthrough.CalculationEndI.GetText());
            Assert.Equal("(height + 22)", ranged_passthrough.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged_passthrough.CalculationEndK.GetText());
            Assert.Equal("-7", ranged_passthrough.OffsetI.GetText());
            Assert.Equal("-9", ranged_passthrough.OffsetJ.GetText());
            Assert.Equal("-11", ranged_passthrough.OffsetK.GetText());
            Assert.Equal("-9", ranged_passthrough.OffsetX.GetText());
            Assert.Equal("-11", ranged_passthrough.OffsetY.GetText());
            Assert.Equal("-11", ranged_passthrough.OffsetZ.GetText());

            Assert.Equal("(x - 9)", ranged_perlin2.X.GetText());
            Assert.Equal("(y - 11)", ranged_perlin2.Y.GetText());
            Assert.Equal("(z - 11)", ranged_perlin2.Z.GetText());
            Assert.Equal("(x + width + 9)", ranged_perlin2.OuterX.GetText());
            Assert.Equal("(y + height + 11)", ranged_perlin2.OuterY.GetText());
            Assert.Equal("(z + depth + 11)", ranged_perlin2.OuterZ.GetText());
            Assert.Equal("(width + 18)", ranged_perlin2.Width.GetText());
            Assert.Equal("(height + 22)", ranged_perlin2.Height.GetText());
            Assert.Equal("(depth + 22)", ranged_perlin2.Depth.GetText());
            Assert.Equal("0", ranged_perlin2.CalculationStartI.GetText());
            Assert.Equal("0", ranged_perlin2.CalculationStartJ.GetText());
            Assert.Equal("0", ranged_perlin2.CalculationStartK.GetText());
            Assert.Equal("(width + 18)", ranged_perlin2.CalculationEndI.GetText());
            Assert.Equal("(height + 22)", ranged_perlin2.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged_perlin2.CalculationEndK.GetText());
            Assert.Equal("0", ranged_perlin2.OffsetI.GetText());
            Assert.Equal("0", ranged_perlin2.OffsetJ.GetText());
            Assert.Equal("0", ranged_perlin2.OffsetK.GetText());
            Assert.Equal("-9", ranged_perlin2.OffsetX.GetText());
            Assert.Equal("-11", ranged_perlin2.OffsetY.GetText());
            Assert.Equal("-11", ranged_perlin2.OffsetZ.GetText());

            Assert.Equal("(x - 2)", ranged_perlin.X.GetText());
            Assert.Equal("(y - 2)", ranged_perlin.Y.GetText());
            Assert.Equal("z", ranged_perlin.Z.GetText());
            Assert.Equal("(x + width + 2)", ranged_perlin.OuterX.GetText());
            Assert.Equal("(y + height + 2)", ranged_perlin.OuterY.GetText());
            Assert.Equal("z + depth", ranged_perlin.OuterZ.GetText());
            Assert.Equal("(width + 4)", ranged_perlin.Width.GetText());
            Assert.Equal("(height + 4)", ranged_perlin.Height.GetText());
            Assert.Equal("depth", ranged_perlin.Depth.GetText());
            Assert.Equal("14", ranged_perlin.CalculationStartI.GetText());
            Assert.Equal("18", ranged_perlin.CalculationStartJ.GetText());
            Assert.Equal("22", ranged_perlin.CalculationStartK.GetText());
            Assert.Equal("(width + 18)", ranged_perlin.CalculationEndI.GetText());
            Assert.Equal("(height + 22)", ranged_perlin.CalculationEndJ.GetText());
            Assert.Equal("(depth + 22)", ranged_perlin.CalculationEndK.GetText());
            Assert.Equal("-7", ranged_perlin.OffsetI.GetText());
            Assert.Equal("-9", ranged_perlin.OffsetJ.GetText());
            Assert.Equal("-11", ranged_perlin.OffsetK.GetText());
            Assert.Equal("-9", ranged_perlin.OffsetX.GetText());
            Assert.Equal("-11", ranged_perlin.OffsetY.GetText());
            Assert.Equal("-11", ranged_perlin.OffsetZ.GetText());
        }
    }
}
