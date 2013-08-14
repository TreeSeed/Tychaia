// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "issue")]
    public class AnalysisIssue
    {
        [DataMember(Name = "description")] public string
            Description;

        [DataMember(Name = "id")] public string
            ID;

        [DataMember(Name = "locations")] public List<AnalysisLocation>
            Locations = new List<AnalysisLocation>();

        [DataMember(Name = "name")] public string
            Name;

        /// <summary>
        /// Locations can't overlap in the XML or the XSLT won't render
        /// them correctly.  Flattens all of the locations out into
        /// linear segments.
        /// </summary>
        public void FlattenLocations()
        {
            var newLocations = new List<AnalysisLocation>();
            var locationStack = new Stack<AnalysisLocation>();
            if (this.Locations.Count == 0)
                return;
            var minStart = this.Locations.Min(x => x.Start);
            var maxEnd = this.Locations.Max(x => x.End);
            AnalysisLocation lastOriginalLocation = null;

            var uid = 0;
            foreach (var loc in this.Locations)
                loc.UniqueID = uid++;

            for (var i = minStart; i <= maxEnd; i++)
            {
                // Push on, in order of their end point, the locations
                // that start here.
                foreach (var location in this.Locations.Where(x => x.Start == i).OrderByDescending(x => x.End))
                    locationStack.Push(location);

                // Add or extend the analysis location.
                if (locationStack.Count > 0)
                {
                    if (locationStack.Peek() == lastOriginalLocation)
                    {
                        newLocations.Last().End += 1;
                    }
                    else
                    {
                        var copy = locationStack.Peek().Copy();
                        copy.Start = i;
                        copy.End = i + 1;
                        foreach (var parent in locationStack.ToArray())
                            copy.UniqueIDRefs += "[" + parent.UniqueID + "]";

                        if (copy.UniqueIDRefs == "")
                            throw new InvalidOperationException();
                        newLocations.Add(copy);
                    }
                    lastOriginalLocation = locationStack.Peek();
                }
                else
                    lastOriginalLocation = null;

                // Pop off the locations while their end point is equal
                // to here.
                while (locationStack.Count > 0 && locationStack.Peek().End == i + 1)
                    locationStack.Pop();
            }

            this.Locations = newLocations;
        }
    }
}
