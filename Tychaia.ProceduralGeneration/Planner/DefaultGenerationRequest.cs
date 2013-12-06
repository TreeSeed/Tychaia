// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration
{
    internal class DefaultGenerationRequest : IGenerationRequest
    {
        private List<GenerationRegion> m_OriginalRegions = new List<GenerationRegion>();

        public event ProgressEventHandler Progress;
        public event RegionCompleteEventHandler RegionComplete;

        public IEnumerable<GenerationRegion> OriginalRegions 
        { 
            get { return this.m_OriginalRegions; } 
        }

        public IEnumerable<GenerationRegion> PlannedRegions { get; set; }
        public IGenerator Generator { get; set; }

        public void AddRegion(long x, long y, long z, int width, int height, int depth)
        {
            this.m_OriginalRegions.Add(new GenerationRegion
            {
                X = x,
                Y = y,
                Z = z,
                Width = width,
                Height = height,
                Depth = depth
            });
        }

        public void InvokeProgress(object sender, ProgressEventArgs e)
        {
            if (this.Progress != null)
                this.Progress(sender, e);
        }

        public void InvokeRegionComplete(object sender, RegionCompleteEventArgs e)
        {
            if (this.RegionComplete != null)
                this.RegionComplete(sender, e);
        }

        public DefaultGenerationRequest(IGenerator generator)
        {
            this.Generator = generator;
        }
    }
}
