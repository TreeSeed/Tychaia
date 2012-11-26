using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Substrate;
using Substrate.Core;
using System.IO;
using Substrate.Nbt;
using Tychaia.Generators;

namespace MinecraftExport
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MinecraftExport <type> <target_dir>");
                Console.WriteLine("Available Types: alpha, beta, anvil");
                return;
            }

            string dest = args[1];
            int xmin = 0;
            int xmax = 1;
            int zmin = -200;
            int zmaz = 200;

            NbtVerifier.InvalidTagType += (e) =>
            {
                throw new Exception("Invalid Tag Type: " + e.TagName + " [" + e.Tag + "]");
            };
            NbtVerifier.InvalidTagValue += (e) =>
            {
                throw new Exception("Invalid Tag Value: " + e.TagName + " [" + e.Tag + "]");
            };
            NbtVerifier.MissingTag += (e) =>
            {
                throw new Exception("Missing Tag: " + e.TagName);
            };

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            // This will instantly create any necessary directory structure
            NbtWorld world;
            switch (args[0])
            {
                case "alpha": world = AlphaWorld.Create(dest); break;
                case "beta": world = BetaWorld.Create(dest); break;
                case "anvil": world = AnvilWorld.Create(dest); break;
                default: throw new Exception("Invalid world type specified.");
            }

            IChunkManager cm = world.GetChunkManager();

            // We can set different world parameters
            world.Level.LevelName = "Tychaia";
            world.Level.Spawn = new SpawnPoint(20, 70, 20);
            world.Level.GameType = GameType.CREATIVE;
            world.Save();

            // world.Level.SetDefaultPlayer();
            // We'll let MC create the player for us, but you could use the above
            // line to create the SSP player entry in level.dat.

            // We'll create chunks at chunk coordinates xmin,zmin to xmax,zmax
            for (int xi = xmin; xi < xmax; xi++)
            {
                for (int zi = zmin; zi < zmaz; zi++)
                {
                    // This line will create a default empty chunk, and create a
                    // backing region file if necessary (which will immediately be
                    // written to disk)
                    ChunkRef chunk = cm.CreateChunk(xi, zi);

                    // This will suppress generating caves, ores, and all those
                    // other goodies.
                    chunk.IsTerrainPopulated = true;

                    // Auto light recalculation is horrifically bad for creating
                    // chunks from scratch, because we're placing thousands
                    // of blocks.  Turn it off.
                    chunk.Blocks.AutoLight = false;

                    // Set the blocks
                    FlatChunk(chunk, 64);

                    // Reset and rebuild the lighting for the entire chunk at once
                    chunk.Blocks.RebuildHeightMap();
                    chunk.Blocks.RebuildBlockLight();
                    chunk.Blocks.RebuildSkyLight();

                    double total = (xmax - xmin) * (zmaz - zmin);
                    double current = (zi - zmin) + (xi - xmin) * (zmaz - zmin);

                    Console.WriteLine("Built Chunk {0},{1} ({2}%)", chunk.X, chunk.Z, current / total * 100);

                    // Save the chunk to disk so it doesn't hang around in RAM
                    cm.Save();
                }
            }

            // Save all remaining data (including a default level.dat)
            // If we didn't save chunks earlier, they would be saved here
            world.Save();
        }

        static void FlatChunk(ChunkRef chunk, int height)
        {
            // Get the data from the generator.
            int[] data = ChunkProvider.GetData(chunk.LocalX * 16, chunk.LocalZ * 16, 0);

            // Populate values.
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int tid = data[x + z * 16 + y * 16 * 16];
                        if (tid == -1)
                        {
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.AIR);
                            continue;
                        }

                        if (Block.BlockIDMapping[tid] == Block.DirtBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.DIRT);
                        else if (Block.BlockIDMapping[tid] == Block.GrassBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.GRASS);
                        else if (Block.BlockIDMapping[tid] == Block.GrassLeafBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.LEAVES);
                        else if (Block.BlockIDMapping[tid] == Block.LeafBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.LEAVES);
                        else if (Block.BlockIDMapping[tid] == Block.LeafGreyBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.LEAVES);
                        else if (Block.BlockIDMapping[tid] == Block.LavaBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.STATIONARY_LAVA);
                        else if (Block.BlockIDMapping[tid] == Block.SandBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.SAND);
                        else if (Block.BlockIDMapping[tid] == Block.SandGrassBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.SANDSTONE);
                        else if (Block.BlockIDMapping[tid] == Block.SnowBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.SNOW_BLOCK);
                        else if (Block.BlockIDMapping[tid] == Block.StoneBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.STONE);
                        else if (Block.BlockIDMapping[tid] == Block.TrunkBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.WOOD);
                        else if (Block.BlockIDMapping[tid] == Block.WaterBlock)
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.STATIONARY_WATER);
                        else
                            chunk.Blocks.SetID(x, y, z, (int)BlockType.OBSIDIAN);
                    }
                }
            }
        }
    }
}
