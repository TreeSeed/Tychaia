using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.ProceduralGeneration;
using System.Runtime.Serialization;
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
            // FIXME: Use StorageAccess to load reference
            // to world generation.
            throw new NotImplementedException();
        }

        #endregion

        #region Optimization Subsystem

        public static void DiscardUnneededChunks()
        {
            /* Providing chunk data is expensive; there's no need to
             * do it if the specified chunk isn't actually going to be
             * rendered onto the screen.
             */

            if (FilteredFeatures.IsEnabled(Feature.OptimizeChunkProviding))
            {
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
        }

        #endregion

        #region Providing Subsystem

        private class ProvideState
        {
            //public int Z;
            public Block[, ,] Blocks;
            public int[] RawData;
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
            DateTime start = DateTime.Now;
            FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Started with 0ms.");

            if (m_CurrentProvideState == null)
            {
                ProvideState ps = new ProvideState();
                ps.Blocks = task.Blocks;
                ps.RawData = task.RawData;
                ps.Info = task.Info;
                //ps.Z = task.Chunk.GlobalZ;
                ps.ProvideTask = task;
                ps.OnSkipCallback = task.OnSkipCallback;
                ps.OnGenerationCallback = task.OnGenerationCallback;
                m_CurrentProvideState = ps;
            }

            // Generate or load data.
            int[] data = null;
            if (m_CurrentProvideState.ProvideTask.Info.LevelDisk == null || !m_CurrentProvideState.ProvideTask.Info.LevelDisk.HasRegion(
                     m_CurrentProvideState.Info.Bounds.X,
                     m_CurrentProvideState.Info.Bounds.Y,
                     m_CurrentProvideState.Info.Bounds.Z,
                     m_CurrentProvideState.Info.Bounds.Width,
                     m_CurrentProvideState.Info.Bounds.Height,
                     m_CurrentProvideState.Info.Bounds.Depth))
                data = m_ResultLayer.GenerateData(
                     m_CurrentProvideState.Info.Bounds.X,
                     m_CurrentProvideState.Info.Bounds.Y,
                     m_CurrentProvideState.Info.Bounds.Z,
                     m_CurrentProvideState.Info.Bounds.Width,
                     m_CurrentProvideState.Info.Bounds.Height,
                     m_CurrentProvideState.Info.Bounds.Depth);
            else
                data = m_CurrentProvideState.ProvideTask.Info.LevelDisk.ProvideRegion(
                     m_CurrentProvideState.Info.Bounds.X,
                     m_CurrentProvideState.Info.Bounds.Y,
                     m_CurrentProvideState.Info.Bounds.Z,
                     m_CurrentProvideState.Info.Bounds.Width,
                     m_CurrentProvideState.Info.Bounds.Height,
                     m_CurrentProvideState.Info.Bounds.Depth);

            // Set up block mappings.
            for (int i = 0; i < m_CurrentProvideState.Info.Bounds.Width; i++)
                for (int j = 0; j < m_CurrentProvideState.Info.Bounds.Height; j++)
                    for (int k = 0; k < m_CurrentProvideState.Info.Bounds.Depth; k++)
                    {
                        int id = data[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height];
                        m_CurrentProvideState.RawData[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height] = id;
                        if (id == -1)
                            m_CurrentProvideState.Blocks[i, j, k] = null;
                        else
                            m_CurrentProvideState.Blocks[i, j, k] = Block.BlockIDMapping[data[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height]];
                    }

            FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Provided " + /*zcount +*/ " levels to chunk in " + (DateTime.Now - start).TotalMilliseconds + "ms.");

            // Signal finish.
            m_CurrentProvideState.OnGenerationCallback();
            m_CurrentProvideState = null;
        }

        #endregion

        #region Tasking Subsystem

        public class ProvideTask
        {
            public Block[,,] Blocks;
            public int[] RawData;
            public ChunkInfo Info;
            public Chunk Chunk;
            public Action OnSkipCallback;
            public Action OnGenerationCallback;
        }

        private static ConcurrentBag<ProvideTask> m_Tasks = new ConcurrentBag<ProvideTask>();
        private static List<ProvideTask> m_Skip = new List<ProvideTask>();

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

        public static ProvideTask FillChunk(Chunk chunk, int[] rawdata, Block[, ,] blocks, ChunkInfo info, Action onSkip, Action onGeneration)
        {
            ProvideTask rt = new ProvideTask()
            {
                Chunk = chunk,
                Blocks = blocks,
                RawData = rawdata,
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
