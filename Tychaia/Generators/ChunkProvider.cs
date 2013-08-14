// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public static class ChunkProvider
    {
        private static RuntimeLayer m_ResultLayer = null;
        private const string WORLD_CONFIG_FILE = "WorldConfig.xml";

        #region Initialization

        static ChunkProvider()
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(WORLD_CONFIG_FILE))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).DefaultForGame)
                {
                    m_ResultLayer = StorageAccess.ToRuntime(layer);
                    break;
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
            public BlockAsset[, ,] Blocks;
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
            if (task == null)
                return;
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
            int computations;
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
                     (int)m_CurrentProvideState.Info.Bounds.Width,
                    (int)m_CurrentProvideState.Info.Bounds.Height,
                    (int)m_CurrentProvideState.Info.Bounds.Depth,
                     out computations);
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
                        {
                            try
                            {
                                m_CurrentProvideState.Blocks[i, j, k] = null;
                                //Block.BlockIDMapping[data[i + j * m_CurrentProvideState.Info.Bounds.Width + k * m_CurrentProvideState.Info.Bounds.Width * m_CurrentProvideState.Info.Bounds.Height]];
                            }
                            catch (KeyNotFoundException)
                            {
                                m_CurrentProvideState.Blocks[i, j, k] = null;
                            }
                        }
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
            public BlockAsset[,,] Blocks;
            public int[] RawData;
            public ChunkInfo Info;
            public Chunk Chunk;
            public Action OnSkipCallback;
            public Action OnGenerationCallback;
        }

        private static List<ProvideTask> m_Tasks = new List<ProvideTask>();
        private static List<ProvideTask> m_Skip = new List<ProvideTask>();

        private static void Run()
        {
            while (true)
            {
                ProvideTask rt;
                if (m_CurrentProvideState != null)
                    rt = m_CurrentProvideState.ProvideTask;
                else if (m_Tasks.Count == 0) //!m_Tasks.TryTake(out rt))
                {
                    Thread.Sleep(10);
                    continue;
                }
                else
                {
                    rt = m_Tasks[0];
                    m_Tasks.RemoveAt(0);
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

        public static ProvideTask FillChunk(Chunk chunk, int[] rawdata, BlockAsset[, ,] blocks, ChunkInfo info, Action onSkip, Action onGeneration)
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
