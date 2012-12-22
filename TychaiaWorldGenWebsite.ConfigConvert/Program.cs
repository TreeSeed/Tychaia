using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TychaiaWorldGenViewer.Flow;
using System.Reflection;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration;
using System.IO;
using System.Xml;

namespace TychaiaWorldGenWebsite.ConfigConvert
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
                return;

            var input = args[0];
            var output = args[1];

            // Do conversion.
            var layer = GetResultLayerFromFile(input);
            var javascript = GenerateJavascriptPath(layer);
            javascript = "function loadConfig(asm, rand)\n{\n  return " + javascript + ";\n}";

            using (var writer = new StreamWriter(output))
            {
                writer.WriteLine(javascript);
            }
        }

        private static string MakeInt64(long number)
        {
            uint a = (uint)(number & 0xffffff);
            uint b = (uint)((number >> 24) & 0xffffff);
            uint c = (uint)((number >> 48) & 0xffff);
            return "System.Int64.Create(" + a + ", " + b + ", " + c + ")";
        }

        private static string MakeUInt64(ulong number)
        {
            uint a = (uint)(number & 0xffffff);
            uint b = (uint)((number >> 24) & 0xffffff);
            uint c = (uint)((number >> 48) & 0xffff);
            return "System.UInt64.Create(" + a + ", " + b + ", " + c + ")";
        }

        private static string GenerateJavascriptPath(Layer layer, string initleading = "  ")
        {
            var leading = initleading + "  ";
            var js = initleading + "(function(asm, rand) {\n";
            js += leading + "var t = new asm." + layer.GetType().FullName + "(";
            for (int i = 0; i < layer.Parents.Length; i++)
            {
                if (i != 0)
                    js += ",\n";
                else
                    js += "\n";
                js += GenerateJavascriptPath(layer.Parents[i], leading + "  ");
            }
            if (layer.Parents.Length == 0)
                js += "System.Int64.FromNumberImpl(rand, System.Int64.Create)";
            js += (layer.Parents.Length != 0 ? "\n" + leading : "") + ");\n";
            foreach (var t in layer.GetType().GetProperties())
            {
                if (t.GetCustomAttributes(typeof(DataMemberAttribute), true).Count() > 0)
                {
                    var v = t.GetGetMethod().Invoke(layer, null);
                    if (v is string)
                        js += leading + "t." + t.Name + " = \"" + (string)v + "\";\n";
                    else if (v is int)
                        js += leading + "t." + t.Name + " = " + (int)v + ";\n";
                    else if (v is uint)
                        js += leading + "t." + t.Name + " = " + (uint)v + ";\n";
                    else if (v is long)
                        js += leading + "t." + t.Name + " = " + MakeInt64((long)v) + ";\n";
                    else if (v is ulong)
                        js += leading + "t." + t.Name + " = " + MakeUInt64((ulong)v) + ";\n";
                    else if (v is float)
                        js += leading + "t." + t.Name + " = " + (float)v + ";\n";
                    else if (v is double)
                        js += leading + "t." + t.Name + " = " + (double)v + ";\n";
                    else if (v is bool)
                        js += leading + "t." + t.Name + " = " + ((bool)v).ToString().ToLower() + ";\n";
                    else if (v is Enum)
                        js += leading + "t." + t.Name + " = " + (int)v + ";\n";
                    else
                        js += leading + " // Can't convert value of type " + t.PropertyType.FullName + "\n";
                }
            }
            js += leading + "return t;\n";
            js += initleading + "})(asm, rand)";
            return js;
        }

        private static Layer GetResultLayerFromFile(string path)
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            List<Type> types = new List<Type> {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(LayerFlowConnector),
                typeof(LayerFlowElement),
            };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Layer).IsAssignableFrom(t))
                        types.Add(t);
            var serTypes = types.ToArray();

            // Load configuration.
            var x = new DataContractSerializer(typeof(FlowInterfaceControl.ListFlowElement), types.ToArray());
            FlowInterfaceControl.ListFlowElement config = null;
            using (var fstream = new FileStream(path, FileMode.Open))
            using (var reader = XmlDictionaryReader.CreateTextReader(fstream, new XmlDictionaryReaderQuotas() { MaxDepth = 1000 }))
                config = x.ReadObject(reader, true) as FlowInterfaceControl.ListFlowElement;

            // Find the result layer.
            foreach (FlowElement fe in config)
            {
                if (fe is LayerFlowElement)
                {
                    if ((fe as LayerFlowElement).Layer is LayerStoreResult)
                    {
                        if (((fe as LayerFlowElement).Layer as LayerStoreResult).FinishType == FinishType.Javascript)
                        {
                            return (fe as LayerFlowElement).Layer;
                        }
                    }
                }
            }

            return null;
        }
    }
}
