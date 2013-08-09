// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ninject;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class FlowForm : Form, IRenderingLocationProvider, ICurrentWorldSeedProvider
    {
        private readonly FlowProcessingPipeline m_FlowProcessingPipeline;
        private ToolStripItem m_PerformanceTestStart;
        private int m_PerformanceResultsLeftToCalculate;

        private long m_X;
        private long m_Y;
        private long m_Z;

        public FlowForm(IKernel kernel, Lazy<FlowProcessingPipeline> flowProcessingPipeline)
        {
            // TODO: Expose this in the UI.
            this.Seed = 0xDEADBEEF;

            this.InitializeComponent();
            kernel.Bind<IRenderingLocationProvider>().ToMethod(context => this);
            kernel.Bind<ICurrentWorldSeedProvider>().ToMethod(context => this);
            this.m_FlowProcessingPipeline = flowProcessingPipeline.Value;
            if (this.m_FlowProcessingPipeline == null)
                throw new Exception("IFlowProcessingPipeline is not of type FlowProcessingPipeline.");
            this.m_FlowProcessingPipeline.FormConnect(this);
            this.CreateAnalysisActions();
            this.UpdateStatusArea();
        }

        public long Seed { get; private set; }

        #region Analysis Actions

        public void CreateAnalysisActions()
        {
            this.c_ToolStrip.Items.Add("-");
            this.m_PerformanceTestStart =
                this.c_ToolStrip.Items.Add(
                    null,
                    ResourceHelper.GetImageResource("TychaiaWorldGenViewerAlgorithm.time.png"),
                    (sender, e) =>
                    {
                        foreach (var element in this.c_FlowInterfaceControl.Elements
                            .Where(x => x is AlgorithmFlowElement)
                            .Cast<AlgorithmFlowElement>())
                        {
                            element.RequestPerformanceStatistics();
                            this.m_PerformanceResultsLeftToCalculate++;
                        }
                        this.UpdateStatusArea();
                    });
        }

        #endregion

        #region Threaded Responses

        private void UpdateStatusArea()
        {
            if (this.m_PerformanceResultsLeftToCalculate > 0)
                this.c_QueueStatus.Text =
                    this.m_PerformanceResultsLeftToCalculate +
                    @" items left for performance testing.";
            else
                this.c_QueueStatus.Text = @"No performance test in progress.";
            this.m_PerformanceTestStart.Enabled =
                this.c_FlowInterfaceControl.Elements.Count != 0 &&
                this.m_PerformanceResultsLeftToCalculate == 0;
        }

        private void c_Timer_OnTick(object sender, EventArgs e)
        {
            this.m_FlowProcessingPipeline.FormCheck();
        }

        public void OnGenerateRuntimeBitmapStart(StorageLayer layer, Bitmap bitmap)
        {
            var element = this.c_FlowInterfaceControl.Elements
                .Where(x => x is AlgorithmFlowElement)
                .Cast<AlgorithmFlowElement>()
                .FirstOrDefault(x => x.Layer == layer);
            if (element == null)
                return;
            element.UpdateBitmaps(null, null, bitmap);
        }

        public void OnGenerateRuntimeBitmapResponse(StorageLayer layer, Bitmap bitmap)
        {
            var element = this.c_FlowInterfaceControl.Elements
                .Where(x => x is AlgorithmFlowElement)
                .Cast<AlgorithmFlowElement>()
                .FirstOrDefault(x => x.Layer == layer);
            if (element == null)
                return;
            element.ClearBitmaps();
            element.UpdateBitmaps(bitmap, null, null);
        }

        public void OnGeneratePerformanceResultsStart(StorageLayer layer, Bitmap bitmap)
        {
            var element = this.c_FlowInterfaceControl.Elements
                .Where(x => x is AlgorithmFlowElement)
                .Cast<AlgorithmFlowElement>()
                .FirstOrDefault(x => x.Layer == layer);
            if (element == null)
                return;
            element.UpdateBitmaps(null, null, bitmap);
        }

        public void OnGeneratePerformanceResultsResponse(
            StorageLayer layer,
            Bitmap resultsBitmap,
            Bitmap compiledBitmap)
        {
            var element = this.c_FlowInterfaceControl.Elements
                .Where(x => x is AlgorithmFlowElement)
                .Cast<AlgorithmFlowElement>()
                .FirstOrDefault(x => x.Layer == layer);
            if (element == null)
                return;
            element.UpdateBitmaps(null, compiledBitmap, resultsBitmap);
            this.m_PerformanceResultsLeftToCalculate--;
            this.UpdateStatusArea();
        }

        #endregion

        #region Loading and Saving

        private string m_LastSavePath;

        private void c_LoadConfigurationButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"XML Files|*.xml",
                CheckFileExists = true,
                CheckPathExists = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StorageLayer[] layers;
                try
                {
                    // Load from file.
                    using (var stream = new StreamReader(openFileDialog.FileName))
                        layers = StorageAccess.LoadStorage(stream);
                    if (layers == null)
                    {
                        MessageBox.Show(this, @"Unable to load configuration file.", @"Configuration invalid.",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(this, @"Unable to load configuration file.", @"Configuration invalid.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Reset state.
                this.c_FlowInterfaceControl.Elements.Clear();
                this.c_FlowInterfaceControl.Invalidate();
                this.m_LastSavePath = openFileDialog.FileName;
                this.c_SaveConfigurationButton.Enabled = true;

                // Create algorithm flow elements.
                this.c_FlowInterfaceControl.Elements.AddRange(
                    layers.Where(v => v != null)
                        .Select(
                            v =>
                                new AlgorithmFlowElement(this.c_FlowInterfaceControl, this.m_FlowProcessingPipeline, v)
                                {
                                    X = v.EditorX,
                                    Y = v.EditorY
                                })
                    );
                this.UpdateStatusArea();
            }
        }

        private void c_SaveConfigurationButton_Click(object sender, EventArgs e)
        {
            if (this.m_LastSavePath == null)
                this.c_SaveConfigurationAsButton.PerformClick();
            else
            {
                // Convert to storage layers.
                var layers = new List<StorageLayer>();
                foreach (var element in this.c_FlowInterfaceControl.Elements)
                {
                    var algorithmElement = element as AlgorithmFlowElement;
                    if (algorithmElement == null)
                        continue;

                    algorithmElement.Layer.EditorX = element.X;
                    algorithmElement.Layer.EditorY = element.Y;

                    layers.Add(algorithmElement.Layer);
                }

                // Save the layers.
                try
                {
                    var memory = new MemoryStream();
                    using (var writer = new StreamWriter(memory))
                    {
                        StorageAccess.SaveStorage(layers.ToArray(), writer);
                        memory.Seek(0, SeekOrigin.Begin);
                        using (var file = new StreamWriter(this.m_LastSavePath, false))
                            memory.CopyTo(file.BaseStream);
                    }

                    MessageBox.Show(this, @"Save successful.", @"Configuration saved.", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, @"Save failure.", ex.Message + @"\r\n" + ex.StackTrace, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void c_SaveConfigurationAsButton_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = @"XML Files|*.xml",
                CheckPathExists = true
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.m_LastSavePath = sfd.FileName;
                this.c_SaveConfigurationButton.Enabled = true;
                this.c_SaveConfigurationButton_Click(this, new EventArgs());
            }
        }

        #endregion

        #region Flow Interface Control

        public long X
        {
            get { return this.m_X; }
        }

        public long Y
        {
            get { return this.m_Y; }
        }

        public long Z
        {
            get { return this.m_Z; }
        }

        private void c_FlowInterfaceControl_MouseWheel(object sender, MouseEventArgs e)
        {
            this.c_FlowInterfaceControl.Pan(-e.X, -e.Y);
            this.c_FlowInterfaceControl.Zoom /= (float) Math.Pow(2, -e.Delta / 120f);
            this.c_FlowInterfaceControl.Pan(e.X, e.Y);
            this.c_ZoomStatus.Text = (this.c_FlowInterfaceControl.Zoom * 100.0f) + @"%";
        }

        private void c_FlowInterfaceControl_SelectedElementChanged(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
            {
                this.c_LayerInspector.SelectedObject = null;
                this.c_ExportSelectedMenuItem.Enabled = false;
                this.c_AnalyseSelectedMenuItem.Enabled = false;
                this.c_TraceSelectedMenuItem.Enabled = false;
                this.c_RenameSelectedMenuItem.Enabled = false;
                this.c_DisableProcessingMenuItem.Enabled = false;
                this.c_DisableProcessingMenuItem.Checked = false;
            }
            else
            {
                this.c_LayerInspector.SelectedObject = this.c_FlowInterfaceControl.SelectedElement.GetObjectToInspect();
                this.c_ExportSelectedMenuItem.Enabled = true;
                this.c_AnalyseSelectedMenuItem.Enabled = true;
                this.c_TraceSelectedMenuItem.Enabled = true;
                this.c_DeleteSelectedMenuItem.Enabled = true;
                this.c_RenameSelectedMenuItem.Enabled = true;
                this.c_DisableProcessingMenuItem.Enabled = true;
                this.c_DisableProcessingMenuItem.Checked =
                    this.c_FlowInterfaceControl.SelectedElement.ProcessingDisabled;
            }
        }

        private void c_LayerInspector_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.SelectedElement.ObjectPropertyUpdated();
        }

        private void c_ExportSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            var ef = new ExportForm(this, this.c_FlowInterfaceControl.SelectedElement);
            ef.Show();
        }

        private void c_AnalyseSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            var af = new AnalyseForm(this.c_FlowInterfaceControl.SelectedElement);
            af.Show();
        }

        private void c_TraceSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            var tf = new TraceForm(this.c_FlowInterfaceControl.SelectedElement);
            tf.ShowDialog();
        }

        private void c_DeleteSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            var selectedElement = this.c_FlowInterfaceControl.SelectedElement;
            this.c_FlowInterfaceControl.SelectedElement = null;
            this.c_FlowInterfaceControl.Elements.Remove(selectedElement);
            this.c_FlowInterfaceControl.Invalidate();
        }

        private void c_RenameSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            var renameDialog = new RenameDialog(this.c_FlowInterfaceControl.SelectedElement.Name);
            if (renameDialog.ShowDialog() == DialogResult.OK)
            {
                this.c_FlowInterfaceControl.SelectedElement.Name = renameDialog.Name;
                this.c_FlowInterfaceControl.Invalidate();
            }
        }

        private void c_DisableProcessingMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            this.c_FlowInterfaceControl.SelectedElement.ProcessingDisabled =
                !this.c_FlowInterfaceControl.SelectedElement.ProcessingDisabled;
            this.c_FlowInterfaceControl.Invalidate();
        }

        private void c_XNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            this.m_X = (long) this.c_XNumericUpDown.Value;
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_YNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            this.m_Y = (long) this.c_YNumericUpDown.Value;
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_ZNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            this.m_Z = (long) this.c_ZNumericUpDown.Value;
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        #endregion

        #region Menu Generation

        private void CreateDynamicLayer(Type t)
        {
            var cis = t.GetConstructors();
            if (cis.Length == 0)
                MessageBox.Show(@"Unable to create specified layer; no available constructor!");
            var ci = cis[0];
            var lo = new List<object>();
            for (var ii = 0; ii < ci.GetParameters().Length; ii++)
                lo.Add(null);
            var o = ci.Invoke(lo.ToArray());
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new AlgorithmFlowElement(
                    this.c_FlowInterfaceControl,
                    this.m_FlowProcessingPipeline,
                    o as IAlgorithm));
        }

        private void CreateMenuItems(ContextMenuStrip menu)
        {
            // Get list of layer types.
            var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         where typeof(IAlgorithm).IsAssignableFrom(type)
                         select type).ToList();

            // For each of those layer types, find ones that have
            // FlowDesignerName, FlowDesignerCategory and FlowDesignerMajorCategory attributes.
            var selectedTypes = new List<SelectedType>();
            foreach (var t in types)
            {
                var foundName = false;
                var foundCategory = false;
                var currentName = "<unknown>";
                var currentCategory = FlowCategory.Undefined;
                var currentMajorCategory = FlowMajorCategory.Undefined;
                var o = t.GetCustomAttributes(true);
                foreach (var a in o)
                {
                    if (a is FlowDesignerNameAttribute)
                    {
                        currentName = (a as FlowDesignerNameAttribute).Name;
                        foundName = true;
                    }
                    if (a is FlowDesignerCategoryAttribute)
                    {
                        currentCategory = (a as FlowDesignerCategoryAttribute).Category;
                        foundCategory = true;
                    }
                    if (a is FlowDesignerMajorCategoryAttribute)
                    {
                        currentMajorCategory = (a as FlowDesignerMajorCategoryAttribute).MajorCategory;
                    }
                }
                if (foundName && foundCategory)
                    selectedTypes.Add(new SelectedType
                    {
                        Name = currentName,
                        MajorCategory = currentMajorCategory,
                        Category = currentCategory,
                        Type = t
                    });
            }

            // Sort selected types into bins.
            selectedTypes = selectedTypes.OrderBy(v => v.Name).ToList();
            menu.Items.Add(new ToolStripMenuItem("Tychaia World Generator") { Enabled = false });

            foreach (FlowMajorCategory m in Enum.GetValues(typeof(FlowMajorCategory)))
            {
                var cont = selectedTypes.Any(t => t.MajorCategory.ToString() == m.ToString());
                if (!cont)
                    continue;

                menu.Items.Add("-");
                menu.Items.Add(new ToolStripMenuItem(FlowDesignerMajorCategoryAttribute.GetDescription(m) + ":")
                {
                    Enabled = false
                });

                foreach (FlowCategory c in Enum.GetValues(typeof(FlowCategory)))
                {
                    cont = false;
                    var cm = new ToolStripMenuItem(FlowDesignerCategoryAttribute.GetDescription(c));
                    foreach (var t in selectedTypes)
                    {
                        if (t.MajorCategory.ToString() == m.ToString() && t.Category.ToString() == c.ToString())
                        {
                            cont = true;
                            var tempt = t;
                            cm.DropDownItems.Add(new ToolStripMenuItem(t.Name, null,
                                (sender, ev) => this.CreateDynamicLayer(tempt.Type), "c_" + t.Name));
                        }
                    }
                    if (cont)
                    {
                        menu.Items.Add(cm);
                    }
                }
            }

            // Add other options.
            if (selectedTypes.Count > 0)
                menu.Items.Add("-");
            menu.Items.Add(this.c_DisableProcessingMenuItem);
            menu.Items.Add(this.c_ExportSelectedMenuItem);
            menu.Items.Add(this.c_AnalyseSelectedMenuItem);
            menu.Items.Add(this.c_TraceSelectedMenuItem);
            menu.Items.Add(this.c_RenameSelectedMenuItem);
            menu.Items.Add(this.c_DeleteSelectedMenuItem);
        }

        private struct SelectedType
        {
            public FlowCategory Category;
            public FlowMajorCategory MajorCategory;
            public string Name;
            public Type Type;
        }

        #endregion
    }
}