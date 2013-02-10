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
        [DataMember]
        private IAlgorithm m_Algorithm;
        [DataMember]
        private StorageLayer[] m_Layers;

        /// <summary>
        /// The current algorithm for this layer.
        /// </summary>
        public IAlgorithm Algorithm
        {
            get
            {
                return this.m_Algorithm;
            }
            set
            {
                this.m_Algorithm = value;
                if (this.m_Algorithm == null)
                    this.m_Layers = new StorageLayer[0];
                else
                    this.m_Layers = new StorageLayer[this.m_Algorithm.InputTypes.Length];
            }
        }
        
        /// <summary>
        /// The input layers.
        /// </summary>
        public StorageLayer[] Inputs
        {
            get
            {
                if (this.m_Layers == null)
                {
                    if (this.m_Algorithm == null)
                        this.m_Layers = new StorageLayer[0];
                    else
                        this.m_Layers = new StorageLayer[this.m_Algorithm.InputTypes.Length];
                }
                return this.m_Layers;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }
}

