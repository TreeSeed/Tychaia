//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using Redpoint.FlowGraph;

namespace Tychaia.ProceduralGeneration.Flow
{
    [DataContract]
    public class AlgorithmFlowConnector : FlowConnector
    {
        public override FlowConnector[] ConnectedTo
        {
            get
            {
                return this.m_LayerOwner.GetConnectorsForLayer(this, this.IsInput);
            }
            set
            {
                this.m_LayerOwner.SetConnectorsForLayer(this, value, this.IsInput);
            }
        }
        
        
        [DataMember]
        private AlgorithmFlowElement
            m_LayerOwner;
        
        public AlgorithmFlowConnector(AlgorithmFlowElement owner, string name, bool isInput, StorageLayer layer)
            : base(owner, name, isInput)
        {
            this.m_LayerOwner = owner;
        }
    }
}

