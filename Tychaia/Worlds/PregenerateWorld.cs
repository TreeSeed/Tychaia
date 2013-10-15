// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Data;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public class PregenerateWorld : IWorld
    {
        private readonly IAssetManager m_AssetManager;
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IGenerationPlanner m_GenerationPlanner;

        private readonly FontAsset m_DefaultFont;
        private readonly List<IEntity> m_Entities;
        private volatile string m_Status;
        private ILevel m_Level;

        public PregenerateWorld(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities twodRenderUtilites,
            IChunkSizePolicy chunkSizePolicy,
            IGenerationPlanner generationPlanner,
            IGeneratorResolver generatorResolver,
            ILevel level)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_2DRenderUtilities = twodRenderUtilites;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_GenerationPlanner = generationPlanner;

            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_Entities = new List<IEntity>();
            this.m_Status = "Generating world...";
            this.m_Level = level;

            // Create plan and execute in a seperate thread.
            var t = new Thread(() =>
            {
                var generator = generatorResolver.GetGeneratorForGame();
                generator.SetSeed(10000);
                var request = this.m_GenerationPlanner.CreateRequest(generator);

                this.m_Level.ScanChunks();
                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                for (var z = -1; z <= 1; z++)
                {
                    if (this.m_Level.HasChunk(
                        x * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth,
                        y * this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight,
                        z * this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth))
                        continue;
                    request.AddRegion(
                        x * this.m_ChunkSizePolicy.ChunkCellWidth,
                        z * this.m_ChunkSizePolicy.ChunkCellDepth,
                        y * this.m_ChunkSizePolicy.ChunkCellHeight,
                        this.m_ChunkSizePolicy.ChunkCellWidth,
                        this.m_ChunkSizePolicy.ChunkCellDepth,
                        this.m_ChunkSizePolicy.ChunkCellHeight);
                }
                
                for (var x = -10; x <= 10; x++)
                for (var z = -10; z <= 10; z++)
                {
                    if (this.m_Level.HasChunk(
                        x * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth,
                        0,
                        z * this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth))
                        continue;
                    request.AddRegion(
                        x * this.m_ChunkSizePolicy.ChunkCellWidth,
                        z * this.m_ChunkSizePolicy.ChunkCellDepth,
                        0,
                        this.m_ChunkSizePolicy.ChunkCellWidth,
                        this.m_ChunkSizePolicy.ChunkCellDepth,
                        1);
                }

                request.Progress += (sender, e) => this.m_Status = "Generating world... " + e.Progress + "%";
                request.RegionComplete += (sender, e) =>
                {
                    var cells = new Cell[this.m_ChunkSizePolicy.ChunkCellWidth,
                        this.m_ChunkSizePolicy.ChunkCellHeight,
                        this.m_ChunkSizePolicy.ChunkCellDepth];
                    for (var x = 0; x < e.Region.GeneratedData.GetLength(0); x++)
                    for (var y = 0; y < e.Region.GeneratedData.GetLength(1); y++)
                    for (var z = 0; z < e.Region.GeneratedData.GetLength(2); z++)
                        cells[x, y, z] = e.Region.GeneratedData[x, y, z];
                    this.m_Level.SaveChunk(
                        e.Region.X * this.m_ChunkSizePolicy.CellVoxelWidth,
                        e.Region.Y * this.m_ChunkSizePolicy.CellVoxelHeight,
                        e.Region.Z * this.m_ChunkSizePolicy.CellVoxelDepth,
                        cells);
                };
                this.m_GenerationPlanner.Execute(request);
            });
            t.IsBackground = true;
            t.Start();
        }

        public List<IEntity> Entities
        {
            get { return this.m_Entities; }
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
                return;

            renderContext.GraphicsDevice.Clear(Color.Black);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
                return;

            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(400, 400),
                this.m_Status,
                this.m_DefaultFont);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void Dispose()
        {
        }
    }
}
