//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Web.Script.Serialization;
using Xunit;

public class SerializeTests
{
    [Fact]
    public void TestDoubleSerialize()
    {
        var data = "{\"Loader\":\"test\"}";
        var originalData = data;
        dynamic obj;

        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            obj = (dynamic)serializer.Deserialize<object>(data);
        }
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonUnconverter() });
            data = serializer.Serialize(obj);
        }
        Assert.Equal(data, originalData);
    }
}
