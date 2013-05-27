//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;
using System.Collections.Generic;

namespace ProceduralGenTracing
{
    class MainClass
    {
        private static Action EnableHandler;
        private static Action DisableHandler;

        public static void Main(string[] args)
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration3 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration4 = new AlgorithmZoom2D();
            var algorithmIncrementWaterDistance1 = new AlgorithmIncrementWaterDistance()
            {
                Initial = true
            };
            var algorithmIncrementWaterDistance2 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance3 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance4 = new AlgorithmIncrementWaterDistance();
            var runtimeInitial = new RuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = new RuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = new RuntimeLayer(algorithmZoom2DIteration2);
            var runtimeZoom2DIteration3 = new RuntimeLayer(algorithmZoom2DIteration3);
            var runtimeZoom2DIteration4 = new RuntimeLayer(algorithmZoom2DIteration4);
            var runtimeIncrementWaterDistance1 = new RuntimeLayer(algorithmIncrementWaterDistance1);
            var runtimeIncrementWaterDistance2 = new RuntimeLayer(algorithmIncrementWaterDistance2);
            var runtimeIncrementWaterDistance3 = new RuntimeLayer(algorithmIncrementWaterDistance3);
            var runtimeIncrementWaterDistance4 = new RuntimeLayer(algorithmIncrementWaterDistance4);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeIncrementWaterDistance1.SetInput(0, runtimeZoom2DIteration1);
            runtimeZoom2DIteration2.SetInput(0, runtimeIncrementWaterDistance1);
            runtimeIncrementWaterDistance2.SetInput(0, runtimeZoom2DIteration2);
            runtimeZoom2DIteration3.SetInput(0, runtimeIncrementWaterDistance2);
            runtimeIncrementWaterDistance3.SetInput(0, runtimeZoom2DIteration3);
            runtimeZoom2DIteration4.SetInput(0, runtimeIncrementWaterDistance3);
            runtimeIncrementWaterDistance4.SetInput(0, runtimeZoom2DIteration4);

            runtimeInitial.Userdata = "runtimeInitial";
            runtimeZoom2DIteration1.Userdata = "runtimeZoom2DIteration1";
            runtimeIncrementWaterDistance1.Userdata = "runtimeIncrementWaterDistance1";
            runtimeZoom2DIteration2.Userdata = "runtimeZoom2DIteration2";
            runtimeIncrementWaterDistance2.Userdata = "runtimeIncrementWaterDistance2";
            runtimeZoom2DIteration3.Userdata = "runtimeZoom2DIteration3";
            runtimeIncrementWaterDistance3.Userdata = "runtimeIncrementWaterDistance3";
            runtimeZoom2DIteration4.Userdata = "runtimeZoom2DIteration4";
            runtimeIncrementWaterDistance4.Userdata = "runtimeIncrementWaterDistance4";

            EnableHandler = () =>
            {
                runtimeInitial.DataGenerated += HandleDataGenerated;
                runtimeZoom2DIteration1.DataGenerated += HandleDataGenerated;
                runtimeIncrementWaterDistance1.DataGenerated += HandleDataGenerated;
                runtimeZoom2DIteration2.DataGenerated += HandleDataGenerated;
                runtimeIncrementWaterDistance2.DataGenerated += HandleDataGenerated;
                runtimeZoom2DIteration3.DataGenerated += HandleDataGenerated;
                runtimeIncrementWaterDistance3.DataGenerated += HandleDataGenerated;
                runtimeZoom2DIteration4.DataGenerated += HandleDataGenerated;
                runtimeIncrementWaterDistance4.DataGenerated += HandleDataGenerated;
            };

            DisableHandler = () =>
            {
                runtimeInitial.DataGenerated -= HandleDataGenerated;
                runtimeZoom2DIteration1.DataGenerated -= HandleDataGenerated;
                runtimeIncrementWaterDistance1.DataGenerated -= HandleDataGenerated;
                runtimeZoom2DIteration2.DataGenerated -= HandleDataGenerated;
                runtimeIncrementWaterDistance2.DataGenerated -= HandleDataGenerated;
                runtimeZoom2DIteration3.DataGenerated -= HandleDataGenerated;
                runtimeIncrementWaterDistance3.DataGenerated -= HandleDataGenerated;
                runtimeZoom2DIteration4.DataGenerated -= HandleDataGenerated;
                runtimeIncrementWaterDistance4.DataGenerated -= HandleDataGenerated;
            };

            EnableHandler();

            var s = 64;
            var o = 10000000;
            int computations;
            runtimeIncrementWaterDistance4.GenerateData(-s + o, -s + o, -s + o, s * 2, s * 2, s * 2, out computations);
        }

        static Dictionary<string, int> m_SaveNames = new Dictionary<string, int>();
        static int m_Count = 0;

        static void HandleDataGenerated(object sender, DataGeneratedEventArgs e)
        {
            var name = (string)(sender as RuntimeLayer).Userdata;
            if (!m_SaveNames.ContainsKey(name))
                m_SaveNames[name] = 0;
            m_SaveNames[name] += 1;

            // Save the internal result.
            var bitmap = AlgorithmTraceImageGeneration.RenderTraceResult(
                sender as RuntimeLayer,
                e.Data,
                e.GSArrayWidth,
                e.GSArrayHeight,
                e.GSArrayDepth);
            Console.WriteLine(name + ": " + m_SaveNames[name] + " (internal)");
            bitmap.Save("layer_" + ++m_Count + ".png");

            // Save the normal result.
            int computations;
            DisableHandler();
            var data = (sender as RuntimeLayer)
                .GenerateData(e.GSAbsoluteX - e.GSMaxOffsetX,
                              e.GSAbsoluteY - e.GSMaxOffsetY,
                              e.GSAbsoluteZ - e.GSMaxOffsetZ,
                              e.GSArrayWidth,
                              e.GSArrayHeight,
                              e.GSArrayDepth, out computations);
            var alt = AlgorithmTraceImageGeneration.RenderTraceResult(
                    sender as RuntimeLayer,
                    data,
                    e.GSArrayWidth,
                    e.GSArrayHeight,
                    e.GSArrayDepth);
            Console.WriteLine(name + ": " + m_SaveNames[name] + " (normal)");
            alt.Save("layer_" + ++m_Count + ".png");
            EnableHandler();
        }
    }
}
