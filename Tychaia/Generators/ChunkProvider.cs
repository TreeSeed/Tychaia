using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.ProceduralGeneration;
using System.Runtime.Serialization;
using TychaiaWorldGenViewer.Flow;
using System.Xml;
using System.Reflection;
using System.IO;
using Tychaia.Globals;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Tychaia.Generators
{
    public static class ChunkProvider
    {
        private static Layer m_ResultLayer = null;
        private static Type[] m_SerializableTypes = null;
        private const string m_WorldConfig = "WorldConfig.xml";

        #region Initialization

        static ChunkProvider()
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
            m_SerializableTypes = types.ToArray();

            // Load configuration.
            DataContractSerializer x = new DataContractSerializer(typeof(FlowInterfaceControl.ListFlowElement), m_SerializableTypes);
            FlowInterfaceControl.ListFlowElement config = null;
            using (FileStream fstream = new FileStream(m_WorldConfig, FileMode.Open))
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fstream, new XmlDictionaryReaderQuotas()))
                config = x.ReadObject(reader, true) as FlowInterfaceControl.ListFlowElement;

            // Find the result layer.
            foreach (FlowElement fe in config)
            {
                if (fe is LayerFlowElement)
                {
                    if ((fe as LayerFlowElement).Layer is Layer3DStoreResult)
                    {
                        m_ResultLayer = (fe as LayerFlowElement).Layer;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Optimization Subsystem

        public static void DiscardUnneededChunks()
        {
            /* Providing chunk data is expensive; there's no need to
             * do it if the specified chunk isn't actually going to be
             * rendered onto the screen.
             */

            int discarded = 0;
            foreach (ProvideTask rt in m_Tasks.ToArray())
            {
                if (!ChunkRenderer.HasNeeded(rt.Chunk))
                {
                    m_Skip.Add(rt);
                    discarded++;
                }
            }

            if (discarded > 0)
            {
                FilteredConsole.WriteLine(FilterCategory.Optimization, "SKIPPED PROVIDING " + discarded + " UNNEEDED CHUNKS!");
                discarded = 0;
            }
        }

        #endregion

        #region Providing Subsystem

        private class ProvideState
        {
            public int Z;
            public Block[, ,] Blocks;
            public ChunkInfo Info;
            public Action OnSkipCallback;
            public Action OnGenerationCallback;
            public ProvideTask ProvideTask;
        }

        private static ProvideState m_CurrentProvideState = null;

        private static void ProvideBlocksToChunk(ProvideTask task)
        {
            if (m_ResultLayer == null)
                throw new InvalidOperationException("No 3D store result layer was found in the world configuration.");
            int depth = Settings.ChunkDepth;
            int depthPerScan = depth;
            DateTime start = DateTime.Now;
            FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Started with 0ms.");

            if (m_CurrentProvideState == null)
            {
                ProvideState ps = new ProvideState();
                ps.Blocks = task.Blocks;
                ps.Info = task.Info;
                ps.Z = 0;
                ps.ProvideTask = task;
                ps.OnSkipCallback = task.OnSkipCallback;
                ps.OnGenerationCallback = task.OnGenerationCallback;
                m_CurrentProvideState = ps;
            }

            int zcount = 0;
            while (m_CurrentProvideState.Z < depth /*&& (DateTime.Now - start).TotalMilliseconds < 1000 / 120*/)
            {
                int[] data = m_ResultLayer.GenerateData(
                    m_CurrentProvideState.Info.Bounds.X,
                    m_CurrentProvideState.Info.Bounds.Y,
                    m_CurrentProvideState.Z,
                    m_CurrentProvideState.Info.Bounds.Width,
                    m_CurrentProvideState.Info.Bounds.Height,
                    depthPerScan);
                for (int i = 0; i < m_CurrentProvideState.Info.Bounds.Width; i++)
                    for (int j = 0; j < m_CurrentProvideState.Info.Bounds.Height; j++)
                        for (int k = 0; k < depthPerScan; k++)
                        {
                            int id = data[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height];
                            if (id == -1)
                                m_CurrentProvideState.Blocks[i, j, m_CurrentProvideState.Z + k] = null;
                            else
                                m_CurrentProvideState.Blocks[i, j, m_CurrentProvideState.Z + k] = Block.BlockIDMapping[data[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height]];
                        }
                m_CurrentProvideState.Z += depthPerScan;
                zcount += depthPerScan;
            }
            FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Provided " + zcount + " levels to chunk in " + (DateTime.Now - start).TotalMilliseconds + "ms.");

            if (m_CurrentProvideState.Z >= depth)
            {
                // Signal finish.
                m_CurrentProvideState.OnGenerationCallback();
                m_CurrentProvideState = null;
            }
        }

        #endregion

        #region Tasking Subsystem

        public class ProvideTask
        {
            public Block[,,] Blocks;
            public ChunkInfo Info;
            public Chunk Chunk;
            public Action OnSkipCallback;
            public Action OnGenerationCallback;
        }

        private static ConcurrentBag<ProvideTask> m_Tasks = new ConcurrentBag<ProvideTask>();
        private static List<ProvideTask> m_Skip = new List<ProvideTask>();
        private static object m_SkipLock = new object();

        private static void Run()
        {
            while (true)
            {
                ProvideTask rt;
                if (m_CurrentProvideState != null)
                    rt = m_CurrentProvideState.ProvideTask;
                else if (!m_Tasks.TryTake(out rt))
                {
                    Thread.Sleep(10);
                    continue;
                }
                if (m_Skip.Contains(rt))
                {
                    m_Skip.Remove(rt);
                    rt.OnSkipCallback();
                    continue;
                }
                ProvideBlocksToChunk(rt);
                Thread.Sleep(10);
            }
        }

        public static void Initialize()
        {
            Thread t = new Thread(Run);
            t.IsBackground = true;
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }

        public static void ProcessSingle()
        {
            /*
            ProvideTask rt;
            if (m_CurrentProvideState != null)
                rt = m_CurrentProvideState.ProvideTask;
            else if (!m_Tasks.TryTake(out rt))
                return;

            ProvideBlocksToChunk(rt);
            */
        }
        
        public static ProvideTask FillChunk(Chunk chunk, Block[, ,] blocks, ChunkInfo info, Action onSkip, Action onGeneration)
        {
            ProvideTask rt = new ProvideTask()
            {
                Chunk = chunk,
                Blocks = blocks,
                Info = info,
                OnSkipCallback = onSkip,
                OnGenerationCallback = onGeneration
            };
            m_Tasks.Add(rt);
            return rt;
        }

        #endregion
    }
}
