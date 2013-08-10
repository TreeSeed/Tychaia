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
        [DataMember] private AlgorithmFlowElement
            m_LayerOwner;

        public AlgorithmFlowConnector(AlgorithmFlowElement owner, string name, bool isInput, StorageLayer layer)
            : base(owner, name, isInput)
        {
            this.m_LayerOwner = owner;
        }

        public override FlowConnector[] ConnectedTo
        {
            get { return this.m_LayerOwner.GetConnectorsForLayer(this, this.IsInput); }
            set { this.m_LayerOwner.SetConnectorsForLayer(this, value, this.IsInput); }
        }

        public override bool CanConnectTo(FlowConnector connector)
        {
            if (!(connector is AlgorithmFlowConnector))
                return false;
            var algorithmFlowConnector = connector as AlgorithmFlowConnector;
            AlgorithmFlowElement inputLayer, outputLayer;
            int inputIndex;
            if (this.IsOutput)
            {
                inputIndex = FlowElement.GetConnectorIndex(algorithmFlowConnector.Owner, algorithmFlowConnector);
                inputLayer = algorithmFlowConnector.m_LayerOwner;
                outputLayer = this.m_LayerOwner;
            }
            else if (this.IsInput)
            {
                inputIndex = FlowElement.GetConnectorIndex(this.Owner, this);
                inputLayer = this.m_LayerOwner;
                outputLayer = algorithmFlowConnector.m_LayerOwner;
            }
            else
                throw new NotSupportedException();
            var inputType = inputLayer.Layer.Algorithm.InputTypes[inputIndex];
            var outputType = outputLayer.Layer.Algorithm.OutputType;
            return inputType == outputType;
        }
    }
}