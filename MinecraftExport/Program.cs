// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.IO;
using Ninject;
using Substrate;
using Substrate.Nbt;
using Tychaia.ProceduralGeneration;

namespace MinecraftExport
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MinecraftExport <type> <target_dir>");
                Console.WriteLine("Available Types: alpha, beta, anvil");
                return;
            }

            var kernel = new StandardKernel();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            var chunkProvider = kernel.Get<ChunkProvider>();

            var dest = args[1];
            var xmin = 0;
            var xmax = 1;
            var zmin = -200;
            var zmaz = 200;

            NbtVerifier.InvalidTagType +=
                e => { throw new Exception("Invalid Tag Type: " + e.TagName + " [" + e.Tag + "]"); };
            NbtVerifier.InvalidTagValue +=
                e => { throw new Exception("Invalid Tag Value: " + e.TagName + " [" + e.Tag + "]"); };
            NbtVerifier.MissingTag += e => { throw new Exception("Missing Tag: " + e.TagName); };

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            // This will instantly create any necessary directory structure
            NbtWorld world;
            switch (args[0])
            {
                case "alpha":
                    world = AlphaWorld.Create(dest);
                    break;
                case "beta":
                    world = BetaWorld.Create(dest);
                    break;
                case "anvil":
                    world = AnvilWorld.Create(dest);
                    break;
                default:
                    throw new Exception("Invalid world type specified.");
            }

            var cm = world.GetChunkManager();

            // We can set different world parameters
            world.Level.LevelName = "Tychaia";
            world.Level.Spawn = new SpawnPoint(20, 70, 20);
            world.Level.GameType = GameType.CREATIVE;
            world.Save();

            // world.Level.SetDefaultPlayer();
            // We'll let MC create the player for us, but you could use the above
            // line to create the SSP player entry in level.dat.

            // We'll create chunks at chunk coordinates xmin,zmin to xmax,zmax
            for (var xi = xmin; xi < xmax; xi++)
            {
                for (var zi = zmin; zi < zmaz; zi++)
                {
                    // This line will create a default empty chunk, and create a
                    // backing region file if necessary (which will immediately be
                    // written to disk)
                    var chunk = cm.CreateChunk(xi, zi);

                    // This will suppress generating caves, ores, and all those
                    // other goodies.
                    chunk.IsTerrainPopulated = true;

                    // Auto light recalculation is horrifically bad for creating
                    // chunks from scratch, because we're placing thousands
                    // of blocks.  Turn it off.
                    chunk.Blocks.AutoLight = false;

                    // Set the blocks
                    FlatChunk(chunk, 64, chunkProvider);

                    // Reset and rebuild the lighting for the entire chunk at once
                    chunk.Blocks.RebuildHeightMap();
                    chunk.Blocks.RebuildBlockLight();
                    chunk.Blocks.RebuildSkyLight();

                    double total = (xmax - xmin)*(zmaz - zmin);
                    double current = (zi - zmin) + (xi - xmin)*(zmaz - zmin);

                    Console.WriteLine("Built Chunk {0},{1} ({2}%)", chunk.X, chunk.Z, current/total*100);

                    // Save the chunk to disk so it doesn't hang around in RAM
                    cm.Save();
                }
            }

            // Save all remaining data (including a default level.dat)
            // If we didn't save chunks earlier, they would be saved here
            world.Save();
        }

        private static void FlatChunk(ChunkRef chunk, int height, ChunkProvider chunkProvider)
        {
            // Get the data from the generator.
            var data = chunkProvider.GetData(chunk.LocalX*16, chunk.LocalZ*16, 0);

            // Populate values.
            for (var y = 0; y < 256; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        var tid = data[x + z*16 + y*16*16];
                        if (tid.BlockAssetName == null)
                        {
                            chunk.Blocks.SetID(x, y, z, BlockType.AIR);
                            continue;
                        }

                        chunk.Blocks.SetID(x, y, z, BlockType.DIRT);
                    }
                }
            }
        }
    }
}