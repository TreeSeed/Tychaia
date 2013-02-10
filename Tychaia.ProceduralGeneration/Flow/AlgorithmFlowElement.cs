//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using Redpoint.FlowGraph;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Tychaia.Globals;
using System.Threading;

namespace Tychaia.ProceduralGeneration.Flow
{
    [DataContract]
    public class AlgorithmFlowElement : FlowElement
    {
        [DataMember]
        private StorageLayer
            m_Layer;
        private FlowInterfaceControl m_Control;
        private Bitmap m_RealBitmap;
        [DataMember]
        private List<FlowConnector>
            m_InputConnectors = new List<FlowConnector>();
        [DataMember]
        private List<FlowConnector>
            m_OutputConnectors = new List<FlowConnector>();
        private Bitmap m_CachedBitmap;
        
        public override Bitmap Image
        {
            get
            {
                if (this.m_RealBitmap == null)
                    return this.m_CachedBitmap;
                else
                {
                    this.m_CachedBitmap = this.m_RealBitmap;
                    return this.m_RealBitmap;
                }
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }
        
        public StorageLayer Layer
        {
            get
            {
                return this.m_Layer;
            }
        }
        
        public AlgorithmFlowElement(FlowInterfaceControl control, StorageLayer l)
        {
            this.m_Control = control;
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
        }

        public AlgorithmFlowElement(FlowInterfaceControl control, IAlgorithm algorithm)
            : this(control, new StorageLayer { Algorithm = algorithm })
        {
        }
        
        private void RefreshImageSync()
        {
            try
            {
                if (this.m_PerformanceThread != null)
                    this.m_PerformanceThread.Abort();
            }
            catch (Exception e)
            {
            }
            if (this.ProcessingDisabled)
            {
                Bitmap b = new Bitmap(this.ImageWidth, this.ImageHeight);
                Graphics g = Graphics.FromImage(b);
                g.Clear(Color.White);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString("Disabled", SystemFonts.DefaultFont, SystemBrushes.WindowText, new PointF(0, 0));
                this.m_RealBitmap = b;
                this.m_Control.Invalidate(this.Region.Apply(this.m_Control.Zoom));
                return;
            }
            this.m_RealBitmap = AlgorithmFlowImageGeneration.RegenerateImageForLayer(this.m_Layer,
                TemporaryCrapBecauseIDidntReallyDesignThingsVeryWell.X,
                TemporaryCrapBecauseIDidntReallyDesignThingsVeryWell.Y,
                TemporaryCrapBecauseIDidntReallyDesignThingsVeryWell.Z,
                64, 64, 64);
            this.m_Control.Invalidate(this.InvalidatingRegion.Apply(this.m_Control.Zoom));
            this.m_PerformanceThread = new Thread(() =>
                {
                    Thread.Sleep(1000);
                    this.PerformMeasurements();
                    this.m_Control.Invoke(new Action(() =>
                        {
                            this.m_Control.Invalidate(this.InvalidatingRegion.Apply(this.m_Control.Zoom));
                        }));
                });
            this.m_PerformanceThread.Start();
        }

        private Thread m_PerformanceThread = null;

        private void PerformMeasurements()
        {
            // Settings.
            var iterations = 1000;
            var warningLimit = 100000; // 0.1s
            var badLimit = 300000; // 0.3s

            // Perform conversions.
            var runtime = StorageAccess.ToRuntime(this.m_Layer);
            IGenerator compiled = null;
            try
            {
                compiled = StorageAccess.ToCompiled(runtime);
            }
            catch (Exception)
            {
                // Failed to compile layer.
            }

            // First check how long it takes for the runtime layer to do 1000 operations of 8x8x8.
            var runtimeStart = DateTime.Now;
            for (var i = 0; i < iterations; i++)
                runtime.GenerateData(0, 0, 0, 8, 8, 8);
            var runtimeEnd = DateTime.Now;

            // Now check how long it takes the compiled layer to do 1000 operations of 8x8x8.
            var compiledStart = DateTime.Now;
            if (compiled != null)
                for (var i = 0; i < iterations; i++)
                    compiled.GenerateData(0, 0, 0, 8, 8, 8);
            var compiledEnd = DateTime.Now;

            // Determine the per-operation cost.
            var runtimeCost = runtimeEnd - runtimeStart;
            var compiledCost = compiledEnd - compiledStart;
            var runtime탎 = Math.Round((runtimeCost.TotalMilliseconds / iterations) * 1000, 0); // Microseconds.
            var compiled탎 = Math.Round((compiledCost.TotalMilliseconds / iterations) * 1000, 0);

            // Define colors and determine values.
            var okay = new SolidBrush(Color.LightGreen);
            var warning = new SolidBrush(Color.Orange);
            var bad = new SolidBrush(Color.IndianRed);
            var runtimeColor = okay;
            var compiledColor = okay;
            if (runtime탎 > warningLimit) runtimeColor = warning;
            if (compiled탎 > warningLimit) compiledColor = warning;
            if (runtime탎 > badLimit) runtimeColor = bad;
            if (compiled탎 > badLimit) compiledColor = bad;

            // Draw performance measurements.
            var bitmap = new Bitmap(128, 32);
            var graphics = Graphics.FromImage(bitmap);
            var font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            graphics.Clear(Color.Black);
            graphics.DrawString("Runtime: " + runtime탎 + "탎", font, runtimeColor, new PointF(0, 0));
            if (compiled != null)
                graphics.DrawString("Compiled: " + compiled탎 + "탎", font, compiledColor, new PointF(0, 16));
            else
                graphics.DrawString("Unable to compile.", font, bad, new PointF(0, 16));
            if (this.m_AdditionalInformation != null)
                this.m_AdditionalInformation.Dispose();
            this.m_AdditionalInformation = bitmap;
        }

        private int[] ParentsIndexOf(StorageLayer find)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < this.m_Layer.Inputs.Length; i++)
                if (this.m_Layer.Inputs[i] == find)
                    result.Add(i);
            return result.ToArray();
        }
        
        public override void SetDeserializationData(FlowInterfaceControl control)
        {
            this.m_Control = control;
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
            else
            {
                IEnumerable<AlgorithmFlowElement> lfe = this.m_Control.Elements
                    .Where(v => v is AlgorithmFlowElement)
                    .Select(v => v as AlgorithmFlowElement)
                    .Where(v => v.m_Layer.Inputs == null ? false : v.m_Layer.Inputs.Contains(this.m_Layer));
                
                // TODO: Probably can be moved into LINQ query above.
                List<FlowConnector> fll = new List<FlowConnector>();
                foreach (AlgorithmFlowElement el in lfe)
                {
                    for (int i = 0; i < el.m_InputConnectors.Count; i++)
                    {
                        if ((el.m_InputConnectors[i] as AlgorithmFlowConnector).ConnectedTo.Contains(this.m_OutputConnectors[0]))
                        {
                            fll.Add(el.m_InputConnectors[i]);
                        }
                    }
                }
                return fll.ToArray();
            }
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
                foreach (FlowConnector t in targets)
                {
                    (t.Owner as AlgorithmFlowElement).SetConnectorsForLayer(
                        t as AlgorithmFlowConnector,
                        new AlgorithmFlowConnector[] { connector },
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
                foreach (FlowConnector fc in children)
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
            this.RefreshImageSync();
        }
        
        #region Overridden Properties
        
        public override List<FlowConnector> InputConnectors
        {
            get
            {
                return this.m_InputConnectors;
            }
        }
        
        public override List<FlowConnector> OutputConnectors
        {
            get
            {
                return this.m_OutputConnectors;
            }
        }
        
        #endregion
    }
}

