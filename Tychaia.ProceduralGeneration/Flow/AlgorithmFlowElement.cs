// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Redpoint.FlowGraph;

namespace Tychaia.ProceduralGeneration.Flow
{
    [DataContract]
    public class AlgorithmFlowElement : FlowElement
    {
        private volatile Bitmap m_CompiledBitmap = null;
        private FlowInterfaceControl m_Control;
        private IFlowProcessingPipeline m_FlowProcessingPipeline;

        [DataMember] private List<FlowConnector>
            m_InputConnectors = new List<FlowConnector>();

        [DataMember] private StorageLayer
            m_Layer;

        [DataMember] private List<FlowConnector>
            m_OutputConnectors = new List<FlowConnector>();

        private volatile Bitmap m_RuntimeBitmap = null;

        public AlgorithmFlowElement(FlowInterfaceControl control, IFlowProcessingPipeline processingPipeline,
            StorageLayer l)
        {
            this.m_Control = control;
            this.m_FlowProcessingPipeline = processingPipeline;
            this.m_Layer = l;
            var attrs = l.Algorithm.GetType().GetCustomAttributes(typeof(FlowDesignerNameAttribute), true);
            this.Name = attrs.Length > 0 ? (attrs[0] as FlowDesignerNameAttribute).Name : l.Algorithm.ToString();
            this.ImageWidth = 128;
            this.ImageHeight = 192;
            this.ObjectPropertyUpdated();

            // Create input / output connectors.
            foreach (string s in this.m_Layer.Algorithm.InputNames)
                this.m_InputConnectors.Add(new AlgorithmFlowConnector(this, s, true, l));
            this.m_OutputConnectors.Add(new AlgorithmFlowConnector(this, "Output", false, l));

            /*this.m_CompiledViewToggleThread = new Thread(() =>
            {
                while (this.m_CompiledViewToggleThread.ThreadState != ThreadState.AbortRequested)
                {
                    Thread.Sleep(100);
                    if (this.m_RealBitmap == this.m_RuntimeBitmap)
                        this.m_RealBitmap = this.m_CompiledBitmap;
                    else
                        this.m_RealBitmap = this.m_RuntimeBitmap;
                    try
                    {
                        this.m_Control.Invoke(new Action(() =>
                        {
                            this.m_Control.Invalidate(this.InvalidatingRegion.Apply(this.m_Control.Zoom));
                        }));
                    }
                    catch
                    {
                        break;
                    }
                }
            });
            this.m_CompiledViewToggleThread.Start();*/
        }

        public AlgorithmFlowElement(FlowInterfaceControl control, IFlowProcessingPipeline processingPipeline,
            IAlgorithm algorithm)
            : this(control, processingPipeline, new StorageLayer { Algorithm = algorithm })
        {
        }

        public override Bitmap Image
        {
            get { return this.m_RuntimeBitmap; }
            protected set { throw new NotImplementedException(); }
        }

        public StorageLayer Layer
        {
            get { return this.m_Layer; }
        }

        ~AlgorithmFlowElement()
        {
            //this.m_CompiledViewToggleThread.Abort();
        }

        private int[] ParentsIndexOf(StorageLayer find)
        {
            var result = new List<int>();
            for (var i = 0; i < this.m_Layer.Inputs.Length; i++)
                if (this.m_Layer.Inputs[i] == find)
                    result.Add(i);
            return result.ToArray();
        }

        public override void SetDeserializationData(FlowInterfaceControl control)
        {
            this.m_Control = control;
        }

        public void SetPipeline(IFlowProcessingPipeline processingPipeline)
        {
            this.m_FlowProcessingPipeline = processingPipeline;
        }

        public FlowConnector[] GetConnectorsForLayer(FlowConnector connector, bool isInput)
        {
            if (isInput)
                return this.m_Control.Elements
                    .Where(v => v is AlgorithmFlowElement)
                    .Select(v => v as AlgorithmFlowElement)
                    .Where(v => this.m_Layer.Inputs == null ? false : this.m_Layer.Inputs.Contains(v.m_Layer))
                    .Where(v => this.ParentsIndexOf(v.m_Layer).Contains(this.m_InputConnectors.IndexOf(connector)))
                    .Select(v => v.m_OutputConnectors[0])
                    .ToArray();
            IEnumerable<AlgorithmFlowElement> lfe = this.m_Control.Elements
                .Where(v => v is AlgorithmFlowElement)
                .Select(v => v as AlgorithmFlowElement)
                .Where(v => v.m_Layer.Inputs == null ? false : v.m_Layer.Inputs.Contains(this.m_Layer));

            // TODO: Probably can be moved into LINQ query above.
            var fll = new List<FlowConnector>();
            foreach (var el in lfe)
            {
                for (var i = 0; i < el.m_InputConnectors.Count; i++)
                {
                    if (
                        (el.m_InputConnectors[i] as AlgorithmFlowConnector).ConnectedTo.Contains(
                            this.m_OutputConnectors[0]))
                    {
                        fll.Add(el.m_InputConnectors[i]);
                    }
                }
            }
            return fll.ToArray();
        }

        public void SetConnectorsForLayer(AlgorithmFlowConnector connector, FlowConnector[] targets, bool isInput)
        {
            if (isInput)
            {
                // We are an input connector, we must clear our layer's current
                // parent and set it to the new value.
                if (targets.Length != 1)
                    throw new InvalidOperationException("An input can not be connected to more than one output.");
                if (this.m_Layer.Inputs == null)
                    throw new InvalidOperationException("Input array for an algorithm can not be null.");
                this.m_Layer.Inputs[
                    this.m_InputConnectors.IndexOf(connector)] =
                    (targets[0].Owner as AlgorithmFlowElement).m_Layer;
                this.ObjectPropertyUpdated();
            }
            else
            {
                // We are an output connector, we must add ourselves as the target's
                // parent.  We can do this as a reverse operation on our targets.
                foreach (var t in targets)
                {
                    (t.Owner as AlgorithmFlowElement).SetConnectorsForLayer(
                        t as AlgorithmFlowConnector,
                        new[] { connector },
                        true);
                }
            }

            // Invalidate the control area.
            foreach (Rectangle r in this.GetConnectorRegionsToInvalidate())
                this.m_Control.Invalidate(r);
        }

        public override object GetObjectToInspect()
        {
            return this.m_Layer.Algorithm;
        }

        public override void ObjectPropertyUpdated()
        {
            this.m_Control.PushForReprocessing(this);

            // Update children.
            foreach (FlowConnector output in this.m_OutputConnectors)
            {
                FlowConnector[] children = this.GetConnectorsForLayer(output, false);
                foreach (var fc in children)
                {
                    if (fc is AlgorithmFlowConnector)
                    {
                        ((fc as AlgorithmFlowConnector).Owner as AlgorithmFlowElement).ObjectPropertyUpdated();
                    }
                }
            }
        }

        public override void ObjectReprocessRequested()
        {
            // Use pipeline to put a request on for both the runtime
            // image generation and the performance measurements.
            this.m_FlowProcessingPipeline.InputPipeline.Put(new FlowProcessingRequest
            {
                RequestType = FlowProcessingRequestType.GenerateRuntimeBitmap,
                Parameters = new object[] { this.m_Layer }
            });
        }

        public void RequestPerformanceStatistics()
        {
            this.m_FlowProcessingPipeline.InputPipeline.Put(new FlowProcessingRequest
            {
                RequestType = FlowProcessingRequestType.GeneratePerformanceResults,
                Parameters = new object[] { this.m_Layer }
            });
        }

        public void ClearBitmaps()
        {
            var invalidateOld = this.InvalidatingRegion.Apply(this.m_Control.Zoom);
            if (this.m_RuntimeBitmap != null)
                this.m_RuntimeBitmap.Dispose();
            if (this.m_CompiledBitmap != null)
                this.m_CompiledBitmap.Dispose();
            if (this.m_AdditionalInformation != null)
                this.m_AdditionalInformation.Dispose();
            this.m_RuntimeBitmap = null;
            this.m_CompiledBitmap = null;
            this.m_AdditionalInformation = null;
            this.m_Control.Invalidate(invalidateOld);
            this.m_Control.Invalidate(this.InvalidatingRegion.Apply(this.m_Control.Zoom));
        }

        public void UpdateBitmaps(Bitmap runtime, Bitmap compiled, Bitmap additional)
        {
            var invalidateOld = this.InvalidatingRegion.Apply(this.m_Control.Zoom);
            if (runtime != null)
                this.m_RuntimeBitmap = runtime;
            if (compiled != null)
                this.m_CompiledBitmap = compiled;
            if (additional != null)
                this.m_AdditionalInformation = additional;
            this.m_Control.Invalidate(invalidateOld);
            this.m_Control.Invalidate(this.InvalidatingRegion.Apply(this.m_Control.Zoom));
        }

        #region Overridden Properties

        public override List<FlowConnector> InputConnectors
        {
            get { return this.m_InputConnectors; }
        }

        public override List<FlowConnector> OutputConnectors
        {
            get { return this.m_OutputConnectors; }
        }

        #endregion
    }
}
