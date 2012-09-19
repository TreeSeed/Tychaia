using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Informs the game that this is a layer it can take and use for the actual
    /// generation of the in-game world.
    /// </summary>
    [DataContract]
    public class Layer3DStoreResult : Layer3D
    {
        [DataMember]
        [DefaultValue(Finish3DType.Chunk)]
        [Description("The type of result that is used by the game.")]
        public Finish3DType FinishType
        {
            get;
            set;
        }

        public Layer3DStoreResult(Layer parent)
            : base(parent)
        {
            // Set defaults.
            this.FinishType = Finish3DType.Chunk;
        }

        protected override int[] GenerateDataImpl(int x, int y, int z, int width, int height, int depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            return this.Parents[0].GenerateData(x, y, z, width, height, depth);
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;

            return this.Parents[0].GetLayerColors();
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }

        public override int StandardDepth
        {
            get { return Settings.ChunkDepth; }
        }

        public override string ToString()
        {
            return "Store Result 3D";
        }

    }

    public enum Finish3DType
    {
        Chunk = 0
    }
}
