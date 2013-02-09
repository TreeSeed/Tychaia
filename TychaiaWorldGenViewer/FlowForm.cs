using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tychaia.ProceduralGeneration;
using TychaiaWorldGenViewer.Flow;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace TychaiaWorldGenViewer
{
    public partial class FlowForm : Form
    {
        public FlowForm()
        {
            InitializeComponent();
        }

        #region Loading and Saving

        private static Type[] SerializableTypes = null;

        static FlowForm()
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            List<Type> types = new List<Type> {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(LayerFlowConnector),
                typeof(LayerFlowElement),
            };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Layer).IsAssignableFrom(t))
                        types.Add(t);
            FlowForm.SerializableTypes = types.ToArray();
        }

        private string m_LastSavePath = null;

        private void c_LoadConfigurationButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "XML Files|*.xml",
                CheckFileExists = true,
                CheckPathExists = true
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.c_FlowInterfaceControl.Elements.Clear();
                this.c_FlowInterfaceControl.Invalidate();
                if (config == null)
                {
                    MessageBox.Show(this, "Unable to load configuration file.", "Configuration invalid.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.m_LastSavePath = ofd.FileName;
                this.c_SaveConfigurationButton.Enabled = true;
                foreach (FlowElement el in config)
                {
                    el.SetDeserializationData(this.c_FlowInterfaceControl);
                    this.c_FlowInterfaceControl.Elements.Add(el);
                }
                foreach (FlowElement el in config)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
                this.c_FlowInterfaceControl.Invalidate();
            }
        }

        private void c_SaveConfigurationButton_Click(object sender, EventArgs e)
        {
            if (this.m_LastSavePath == null)
                this.c_SaveConfigurationAsButton.PerformClick();
            else
            {
                DataContractSerializer x = new DataContractSerializer(
                    typeof(FlowInterfaceControl.ListFlowElement),
                    SerializableTypes,
                    Int32.MaxValue,
                    false,
                    true,
                    null);
                FlowInterfaceControl.ListFlowElement config = new FlowInterfaceControl.ListFlowElement();
                foreach (FlowElement el in this.c_FlowInterfaceControl.Elements)
                    config.Add(el);
                using (FileStream writer = new FileStream(this.m_LastSavePath, FileMode.Create))
                    x.WriteObject(writer, config);
                MessageBox.Show(this, "Save successful.", "Configuration saved.", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                DataContractSerializer x = new DataContractSerializer(
                    typeof(FlowInterfaceControl.ListFlowElement),
                    SerializableTypes,
                    Int32.MaxValue,
                    false,
                    true,
                    null);
                FlowInterfaceControl.ListFlowElement config = new FlowInterfaceControl.ListFlowElement();
                foreach (FlowElement el in this.c_FlowInterfaceControl.Elements)
                    config.Add(el);
                using (FileStream writer = new FileStream(sfd.FileName, FileMode.Create))
                    x.WriteObject(writer, config);
                MessageBox.Show(this, "Save successful.", "Configuration saved.", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void c_FlowInterfaceControl_SelectedElementChanged(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
            {
                this.c_LayerInspector.SelectedObject = null;
                this.c_ExportSelectedMenuItem.Enabled = false;
                this.c_DeleteSelectedMenuItem.Enabled = false;
                this.c_RenameSelectedMenuItem.Enabled = false;
                this.c_DisableProcessingMenuItem.Enabled = false;
                this.c_DisableProcessingMenuItem.Checked = false;
            }
            else
            {
                this.c_LayerInspector.SelectedObject = this.c_FlowInterfaceControl.SelectedElement.GetObjectToInspect();
                this.c_ExportSelectedMenuItem.Enabled = true;
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

            ExportForm ef = new ExportForm(this.c_FlowInterfaceControl.SelectedElement);
            ef.Show();
        }

        private void c_DeleteSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            FlowElement fe = this.c_FlowInterfaceControl.SelectedElement;
            this.c_FlowInterfaceControl.SelectedElement = null;
            this.c_FlowInterfaceControl.Elements.Remove(fe);
            this.c_FlowInterfaceControl.Invalidate();
        }

        private void c_RenameSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.c_FlowInterfaceControl.SelectedElement == null)
                return;

            RenameDialog re = new RenameDialog(this.c_FlowInterfaceControl.SelectedElement.Name);
            if (re.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.c_FlowInterfaceControl.SelectedElement.Name = re.Name;
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
            LayerFlowImageGeneration.X = (int)this.c_XNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (FlowElement el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_YNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            LayerFlowImageGeneration.Y = (int)this.c_YNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (FlowElement el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        private void c_ZNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            LayerFlowImageGeneration.Z = (int)this.c_ZNumericUpDown.Value;
            if (this.c_FlowInterfaceControl.SelectedElement != null)
                this.c_FlowInterfaceControl.PushForReprocessing(this.c_FlowInterfaceControl.SelectedElement);
            foreach (FlowElement el in this.c_FlowInterfaceControl.Elements)
                if (el != this.c_FlowInterfaceControl.SelectedElement)
                    this.c_FlowInterfaceControl.PushForReprocessing(el);
        }

        #endregion
    }
}
