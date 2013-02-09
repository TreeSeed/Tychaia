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
    public class StorageLayer
    {
        /// <summary>
        /// The current algorithm for this layer.
        /// </summary>
        [DataMember]
        public IAlgorithm
            Algorithm;
        
        /// <summary>
        /// The input layers.
        /// </summary>
        [DataMember]
        public StorageLayer[]
            Inputs;
    }
}

