// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration
{
    internal class DefaultGenerationPlanner : IGenerationPlanner
    {
        public IGenerationRequest CreateRequest(IGenerator generator)
        {
            return new DefaultGenerationRequest(generator);
        }

        private void PerformOperationRecursively(Action<RuntimeLayer> operation, RuntimeLayer layer)
        {
            operation(layer);
            foreach (var input in layer.GetInputs().Where(input => input != null))
                PerformOperationRecursively(operation, input);
        }

        private void PerformOperation(Action<IGenerator> operation, IGenerator generator)
        {
            if (generator is RuntimeLayer)
                this.PerformOperationRecursively(operation, (RuntimeLayer)generator);
            else
                operation(generator);
        }

        private int GetVolume(GenerationRegion region)
        {
            return region.Width * region.Height * region.Depth;
        }

        private GenerationRegion CombinedRegion(GenerationRegion regionA, GenerationRegion regionB)
        {
            var combined = new GenerationRegion();
            combined.X = Math.Min(regionA.X, regionB.X);
            combined.Y = Math.Min(regionA.Y, regionB.Y);
            combined.Z = Math.Min(regionA.Z, regionB.Z);
            combined.Width = (int)(Math.Max(regionA.X + regionA.Width, regionB.X + regionB.Width) - combined.X);
            combined.Height = (int)(Math.Max(regionA.Y + regionA.Height, regionB.Y + regionB.Height) - combined.Y);
            combined.Depth = (int)(Math.Max(regionA.Z + regionA.Depth, regionB.Z + regionB.Depth) - combined.Z);
            return combined;
        }

        private float GetSavingsRatio(GenerationRegion regionA, GenerationRegion regionB)
        {
            var volumeA = GetVolume(regionA);
            var volumeB = GetVolume(regionB);
            var regionC = CombinedRegion(regionA, regionB);
            var volumeC = GetVolume(regionC);
            return (volumeA + volumeB) / (float)volumeC;
        }

        public void Execute(IGenerationRequest request)
        {
            // Get the wastage comparison of every region with every other region.
            var regions = new List<GenerationRegion>(request.OriginalRegions);
            while (true)
            {
                var restart = false;
                foreach (var regionA in regions)
                {
                    foreach (var regionB in regions)
                    {
                        if (regionA == regionB)
                            continue;
                        var combined = CombinedRegion(regionA, regionB);
                        if (combined.Width * combined.Height * combined.Depth > 50 * 1024 * 1024 &&
                            GetSavingsRatio(regionA, regionB) > 0.5)
                        {
                            regions.Remove(regionA);
                            regions.Remove(regionB);
                            regions.Add(CombinedRegion(regionA, regionB));
                            restart = true;
                            break;
                        }
                    }
                    if (restart) break;
                }
                if (restart) continue;
                break;
            }
            request.PlannedRegions = regions;

            // Set up progress tracking.
            var total = 0;
            var current = 0;
            DataGeneratedEventHandler onProgress = (sender, e) =>
            {
                current++;
                request.InvokeProgress(sender, new ProgressEventArgs(current / (float)total * 100f));
            };
            this.PerformOperation(x => total++, request.Generator);
            this.PerformOperation(x => x.DataGenerated += onProgress, request.Generator);
            total *= request.PlannedRegions.Count();
            foreach (var planned in request.PlannedRegions)
            {
                int computations;
                var result = request.Generator.GenerateData(
                    planned.X,
                    planned.Y,
                    planned.Z,
                    planned.Width,
                    planned.Height,
                    planned.Depth,
                    out computations);

                foreach (var original in request.OriginalRegions)
                {
                    if (original.GeneratedData == null &&
                        original.X >= planned.X &&
                        original.Y >= planned.Y &&
                        original.Z >= planned.X &&
                        original.X + original.Width <= planned.X + planned.Width &&
                        original.Y + original.Height <= planned.Y + planned.Height &&
                        original.Z + original.Depth <= planned.Z + planned.Depth)
                    {
                        // The original region is inside the plan and it hasn't
                        // recieved data yet.  Copy out the data and fire the event.
                        // FIXME: Don't always assume the data will be FlowBundles.
                        original.GeneratedData = new FlowBundle[original.Width, original.Height, original.Depth];
                        for (var x = original.X - planned.X; x < (original.X - planned.X) + original.Width; x++)
                        for (var y = original.Y - planned.Y; y < (original.Y - planned.Y) + original.Height; y++)
                        for (var z = original.Z - planned.Z; z < (original.Z - planned.Z) + original.Depth; z++)
                            original.GeneratedData[
                                (x - (original.X - planned.X)),
                                (y - (original.Y - planned.Y)),
                                (z - (original.Z - planned.Z))] =
                                result[x + y * planned.Width + z * planned.Width * planned.Height];
                        request.InvokeRegionComplete(this, new RegionCompleteEventArgs(original));
                    }
                }
            }
            this.PerformOperation(x => x.DataGenerated -= onProgress, request.Generator);
        }
    }
}

