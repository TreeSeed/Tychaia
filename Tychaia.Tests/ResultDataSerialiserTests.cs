// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using Ninject;
using Ninject.MockingKernel.Moq;
using Tychaia.ProceduralGeneration;
using Xunit;

namespace Tychaia.Tests
{
    public class ResultDataSerialiserTests
    {
        private IKernel CreateKernel()
        {
            var kernel = new MoqMockingKernel();
            kernel.Load<TychaiaIoCModule>();
            kernel.Load<TychaiaDiskIoCModule>();
            return kernel;
        }

        [Fact]
        public void SerialiseAndDeserialiseResultData()
        {
            var kernel = this.CreateKernel();
            var serialiser = kernel.Get<IResultDataSerialiser>();

            var resultData = new ResultData();
            resultData.HeightMap = 4;
            resultData.BlockInfo.BlockAssetName = "hello";

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                var reader = new BinaryReader(stream);
                serialiser.Serialise(writer, resultData);
                stream.Seek(0, SeekOrigin.Begin);
                var compare = serialiser.Deserialise(reader);
                Assert.Equal(4, compare.HeightMap);
                Assert.Equal("hello", compare.BlockInfo.BlockAssetName);
            }
        }
    }
}
