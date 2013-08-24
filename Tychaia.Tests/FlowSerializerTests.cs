// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject;
using Ninject.MockingKernel.Moq;
using Tychaia.ProceduralGeneration;
using Xunit;
using System.IO;

namespace Tychaia.Tests
{
    public class FlowSerializerTests
    {
        private IKernel CreateKernel()
        {
            var kernel = new MoqMockingKernel();
            kernel.Load<TychaiaIoCModule>();
            kernel.Load<TychaiaDiskIoCModule>();
            return kernel;
        }

        [Fact]
        public void SerializeAndDeserializeBundleWithInteger()
        {
            var kernel = this.CreateKernel();
            var serializer = kernel.Get<IFlowBundleSerializer>();

            var bundle = new FlowBundle();
            bundle = bundle.Set("a", 5);
            bundle = bundle.Set("b", 10);

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                var reader = new BinaryReader(stream);
                serializer.Serialize(writer, bundle);
                stream.Seek(0, SeekOrigin.Begin);
                var compare = serializer.Deserialize(reader);
                Assert.IsType<int>(compare.Get("a"));
                Assert.IsType<int>(compare.Get("b"));
                Assert.Equal<int>(5, compare.Get("a"));
                Assert.Equal<int>(10, compare.Get("b"));
                Assert.Equal<int>(bundle.Get("a"), compare.Get("a"));
                Assert.Equal<int>(bundle.Get("b"), compare.Get("b"));
            }
        }
    }
}
