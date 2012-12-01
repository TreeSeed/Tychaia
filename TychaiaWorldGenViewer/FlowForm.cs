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
        private int m_Seed = 8937457;

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
                DataContractSerializer x = new DataContractSerializer(typeof(FlowInterfaceControl.ListFlowElement), SerializableTypes);
                FlowInterfaceControl.ListFlowElement config = null;
                using (FileStream fstream = new FileStream(ofd.FileName, FileMode.Open))
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fstream, new XmlDictionaryReaderQuotas() { MaxDepth = 1000 }))
                    config = x.ReadObject(reader, true) as FlowInterfaceControl.ListFlowElement;
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

        private void c_GeneralAddZoomMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerZoom(null)
                )
            );
        }

        private void c_GeneralAddStoreResultMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerStoreResult(null)
                )
            );
        }

        private void c_BiomeAddScatterBiomeMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerRandomBiome(null)
                )
            );
        }

        private void c_GeneralAddInitialPerlinMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerInitialPerlin(this.m_Seed)
                )
            );
        }

        private void c_3DGeneralAddInitialPerlinMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new Layer3DInitialPerlin(this.m_Seed)
                )
            );
        }

        private void c_GeneralAddInitialVoronoiMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerInitialVoronoi(this.m_Seed)
                )
            );
        }

        private void c_LandAddInitialLandMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerInitialLand(this.m_Seed)
                )
            );
        }

        private void c_LandAddExtendLandMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerExtendLand(null)
               )
            );
        }

        private void c_GeneralAddInvertMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerInvert(null)
               )
            );
        }

        private void c_GeneralAddMixVoronoiMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerVoronoiMixdown(null, null)
               )
            );
        }

        private void c_LandAddDeriveTerrainMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerDeriveTerrain(null, null)
               )
            );
        }

        private void c_LandAddMixTerrainWithPerlinMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerTerrainMixdown(null, null)
               )
            );
        }

        private void c_GeneralAddPerformMathMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerPerformMath(null, null)
               )
            );
        }

        private void c_BiomeAddSecondaryBiomeMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerSecondaryBiome(null, null, null, null)
               )
            );
        }

        private void c_GeneralAddAutoMixVoronoiMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerAutoVoronoiMixdown(this.m_Seed)
                )
            );
        }

        private void c_TownsAddScatterTownsMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerInitialTowns(this.m_Seed)
                )
            );
        }

        private void c_TownsAddEraseTownsOverOceanMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerEraseTowns(null, null)
               )
            );
        }

        private void c_TerrainAddForm3DTerrainMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DFormTerrain(null, null)
               )
            );
        }

        private void c_LandAddMixOreWithVoronoiMixdownMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerOreMixdown(null, null)
               )
            );
        }

        private void c_GeneralAddCopyResultMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerCopyResult(null)
               )
            );
        }

        private void c_TownsAddDetermineViabilityMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerTownSelection(null, null, null, null, null)
               )
            );
        }

        private void extendTownsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerExtendTowns(null, null)
               )
            );
        }

        private void c_TownsAddMixTownsWithViabilityMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerDecayTowns(null, null)
               )
            );
        }

        private void c_3DGeneralStoreResultMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DStoreResult(null)
               )
            );
        }

        private void c_LandAddZoomTownCentersMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new LayerZoomTownCenters(null)
               )
            );
        }
        
        private void buildingPlacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new Layer3DBuildingPlacer(null, null, null)
                )
            );
        }

        private void c_LandAddZoomTerrainMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerZoomWaterDistance(null)
                )
            );
        }

        private void c_3DTerrainAddEdgeDetectionMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DEdgeDetection(null)
               )
            );
        }

        private void c_3DTownsAddFormCityBiomesMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DFormCityBiomes(null, null, null, null, null)
               )
            );
        }

        private void c_3DGeneralAddInitialVoronoiMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DInitialVoronoi(this.m_Seed)
               )
            );
        }

        private void c_GeneralAddInitialOctaveMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new LayerInitialOctave(this.m_Seed)
                )
            );
        }

        private void c_3DGeneralAddInitialOctaveMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
                new LayerFlowElement(
                    this.c_FlowInterfaceControl,
                    new Layer3DInitialOctave(this.m_Seed)
                )
            );
        }

        private void c_3DGeneralAddPerformMathMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
               new LayerFlowElement(
                   this.c_FlowInterfaceControl,
                   new Layer3DPerformMath(null, null)
               )
            );
        }

        private void fillSecondaryCityBiomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.c_FlowInterfaceControl.AddElementAtMouse(
   new LayerFlowElement(
       this.c_FlowInterfaceControl,
       new Layer3DFillSecondaryCityBiomes(null)
   )
);
        }
    }
}
