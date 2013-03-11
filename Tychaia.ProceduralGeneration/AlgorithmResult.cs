//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Store Result")]
    public class AlgorithmResult : Algorithm<int, int>
    {
        [Description("The name to display in interfaces such as Make Me a World.")]
        public string Name
        {
            get;
            set;
        }

        [Description("Whether to display this layer as 2D in the editor.")]
        public bool Layer2D
        {
            get;
            set;
        }

        [Description("Whether this layer should be shown in the Make Me a World website.")]
        public bool ShowInMakeMeAWorld
        {
            get;
            set;
        }

        [Description("Whether this layer is the default for Make Me a World.")]
        public bool DefaultForMakeMeAWorld
        {
            get;
            set;
        }

        [Description("Whether this layer is the default for in-game.")]
        public bool DefaultForGame
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz, int[] ocx, int[] ocy, int[] ocz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

