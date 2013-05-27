using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;
using Tychaia.Globals;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class FlowForm : Form
    {
        public FlowForm()
        {
            InitializeComponent();
        }

        #region Loading and Saving

        private string m_LastSavePath = null;

        private void c_LoadConfigurationButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "XML Files|*.xml",
                CheckFileExists = true,
                CheckPathExists = true
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StorageLayer[] layers;
                try
                {
                    // Load from file.
                    using (var stream = new StreamReader(openFileDialog.FileName))
                        layers = StorageAccess.LoadStorage(stream);
                    if (layers == null)
                    {
                        MessageBox.Show(this, "Unable to load configuration file.", "Configuration invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Unable to load configuration file.", "Configuration invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                          .Select(v => new AlgorithmFlowElement(this.c_FlowInterfaceControl, v) { X = v.EditorX, Y = v.EditorY })
                );

                /*foreach (var el in layers)
                {
                    el.SetDeserializationData(this.c_FlowInterfaceControl);
                    this.c_FlowInterfaceControl.Elements.Add(el);
                }
                foreach (FlowElement el in config)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
                this.c_FlowInterfaceControl.Invalidate();*/
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

                    MessageBox.Show(this, "Save successful.", "Configuration saved.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Save failure.", ex.Message + "\r\n" + ex.StackTrace, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void c_SaveConfigurationAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "XML Files|*.xml",
                CheckPathExists = true
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.m_LastSavePath = sfd.FileName;
                this.c_SaveConfigurationButton.Enabled = true;
                this.c_SaveConfigurationButton_Click(this, new EventArgs());
            }
        }

        #endregion

        #region Flow Interface Control

        private void c_FlowInterfaceControl_MouseWheel(object sender, MouseEventArgs e)
        {
            this.c_FlowInterfaceControl.Pan(-e.X, -e.Y);
            this.c_FlowInterfaceControl.Zoom /= (float)Math.Pow(2, -e.Delta / 120);
            this.c_FlowInterfaceControl.Pan(e.X, e.Y);
            this.c_ZoomStatus.Text = (this.c_FlowInterfaceControl.Zoom * 100.0f).ToString() + "%";
        }

        private void c_FlowInterfaceControl_ElementsInQueueCountChanged(object sender, Redpoint.FlowGraph.FlowInterfaceControl.ElementsInQueueCountChangedEventArgs e)
        {
            this.c_QueueStatus.Text = e.Count.ToString() + " elements in queue.";
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
                this.c_DisableProcessingMenuItem.Checked = this.c_FlowInterfaceControl.SelectedElement.ProcessingDisabled;
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

            var ef = new ExportForm(this.c_FlowInterfaceControl.SelectedElement);
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

            RenameDialog renameDialog = new RenameDialog(this.c_FlowInterfaceControl.SelectedElement.Name);
            if (renameDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            TemporaryCrapBecauseIDidNotReallyDesignThingsVeryWell.X = (int)this.c_XNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_YNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            TemporaryCrapBecauseIDidNotReallyDesignThingsVeryWell.Y = (int)this.c_YNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_ZNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            TemporaryCrapBecauseIDidNotReallyDesignThingsVeryWell.Z = (int)this.c_ZNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (var el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        #endregion

        #region Menu Generation

        private struct SelectedType
        {
            public string Name;
            public FlowCategory Category;
            public FlowMajorCategory MajorCategory;
            public Type Type;
        }

        private void CreateDynamicLayer(Type t)
        {
            ConstructorInfo[] cis = t.GetConstructors();
            if (cis.Length == 0)
                MessageBox.Show("Unable to create specified layer; no available constructor!");
            ConstructorInfo ci = cis[0];
            var lo = new List<object>();
            for (int ii = 0; ii < ci.GetParameters().Length; ii++)
                lo.Add(null);
            object o = ci.Invoke(lo.ToArray());
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new AlgorithmFlowElement(
                this.c_FlowInterfaceControl,
                o as IAlgorithm
            )
            );
        }

        private void CreateMenuItems(ContextMenuStrip menu)
        {
            // Get list of layer types.
            List<Type> types = new List<Type>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (typeof(IAlgorithm).IsAssignableFrom(t))
                        types.Add(t);

            // For each of those layer types, find ones that have
            // FlowDesignerName, FlowDesignerCategory and FlowDesignerMajorCategory attributes.
            List<SelectedType> selectedTypes = new List<SelectedType>();
            foreach (var t in types)
            {
                bool foundName = false;
                bool foundCategory = false;
                string currentName = "<unknown>";
                FlowCategory currentCategory = FlowCategory.Undefined;
                FlowMajorCategory currentMajorCategory = FlowMajorCategory.Undefined;
                object[] o = t.GetCustomAttributes(true);
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
                    selectedTypes.Add(new SelectedType { Name = currentName, MajorCategory = currentMajorCategory, Category = currentCategory, Type = t });
            }

            // Sort selected types into bins.
            selectedTypes.OrderBy(v => v.Name);
            menu.Items.Add(new ToolStripMenuItem("Tychaia World Generator") { Enabled = false });

            foreach (FlowMajorCategory m in Enum.GetValues(typeof(FlowMajorCategory)))
            {
                bool cont = false;

                foreach (var t in selectedTypes)
                    if (t.MajorCategory.ToString() == m.ToString())
                    {
                        cont = true;
                        break;
                    }

                if (cont)
                {
                    menu.Items.Add("-");
                    menu.Items.Add(new ToolStripMenuItem(FlowDesignerMajorCategoryAttribute.GetDescription(m) + ":") { Enabled = false });

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
                                cm.DropDownItems.Add(new ToolStripMenuItem(t.Name, null, (sender, ev) =>
                                {
                                    this.CreateDynamicLayer(tempt.Type);
                                }, "c_" + t.Name));
                            }
                        }
                        if (cont)
                        {
                            menu.Items.Add(cm);
                        }
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

        #endregion
    }
}
