// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration
{
    public interface IGenerationRequest
    {
        event ProgressEventHandler Progress;
        event RegionCompleteEventHandler RegionComplete;

        IEnumerable<GenerationRegion> OriginalRegions { get; }
        IEnumerable<GenerationRegion> PlannedRegions { get; set; }
        IGenerator Generator { get; set; }

        void AddRegion(long x, long y, long z, int width, int height, int depth);
        void InvokeProgress(object sender, ProgressEventArgs e);
        void InvokeRegionComplete(object sender, RegionCompleteEventArgs e);
    }

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
    public delegate void RegionCompleteEventHandler(object sender, RegionCompleteEventArgs e);

    public class ProgressEventArgs : EventArgs
    {
        public float Progress { get; private set; }

        public ProgressEventArgs(float progress)
        {
            this.Progress = progress;
        }
    }

    public class RegionCompleteEventArgs : EventArgs
    {
        public GenerationRegion Region { get; private set; }

        public RegionCompleteEventArgs(GenerationRegion region)
        {
            this.Region = region;
        }
    }
}
