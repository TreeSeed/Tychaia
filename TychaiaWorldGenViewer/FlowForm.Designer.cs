namespace TychaiaWorldGenViewer
{
    partial class FlowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowForm));
            this.c_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.c_ZoomStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.c_LayerInspector = new System.Windows.Forms.PropertyGrid();
            this.c_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.c_2DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddInitialPerlinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddPerlinMathMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.c_GeneralAddInitialVoronoiMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddMixVoronoiMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddAutoMixVoronoiMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.c_GeneralAddZoomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddSmoothMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddRemapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddInvertMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddNormalizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddDenormalizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_Seperator2MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.c_GeneralAddCopyResultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_GeneralAddStoreResultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddInitialLandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddExtendLandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddDeriveTerrainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddMixTerrainWithPerlinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_BiomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_BiomeAddScatterBiomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_BiomeAddSecondaryBiomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RainfallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RainfallAddInitialRainfallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RainfallAddMixRainfallWithBiomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TemperatureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TemperatureAddInitialTemperature = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TemperatureAddMixTemperatureWithBiomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RiversMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RiversAddSimulateFlowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RiversAddPoolLakesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RiversAddPoolOceanMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsAddScatterTownsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsAddEraseTownsOverOceanMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsAddDetermineViabilityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsAddMixTownsWithViabilityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TownsAddSimulateRundownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extendTownsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_LandAddZoomTownCentersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildingPlacerPlacerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_FamilyTreesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.c_3DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_3DGeneralMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_3DGeneralStoreResultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_3DTerrainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_3DTerrainAddForm3DTerrainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.c_RenameSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_DeleteSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.c_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.c_LoadConfigurationButton = new System.Windows.Forms.ToolStripButton();
            this.c_SaveConfigurationButton = new System.Windows.Forms.ToolStripButton();
            this.c_SaveConfigurationAsButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.c_YLabel = new System.Windows.Forms.Label();
            this.c_XLabel = new System.Windows.Forms.Label();
            this.c_XNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.c_YNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.c_FlowInterfaceControl = new TychaiaWorldGenViewer.Flow.FlowInterfaceControl();
            this.c_StatusStrip.SuspendLayout();
            this.c_ContextMenuStrip.SuspendLayout();
            this.c_ToolStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c_XNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_YNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // c_StatusStrip
            // 
            this.c_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_ZoomStatus});
            this.c_StatusStrip.Location = new System.Drawing.Point(0, 475);
            this.c_StatusStrip.Name = "c_StatusStrip";
            this.c_StatusStrip.Size = new System.Drawing.Size(890, 22);
            this.c_StatusStrip.TabIndex = 2;
            this.c_StatusStrip.Text = "statusStrip1";
            // 
            // c_ZoomStatus
            // 
            this.c_ZoomStatus.Name = "c_ZoomStatus";
            this.c_ZoomStatus.Size = new System.Drawing.Size(35, 17);
            this.c_ZoomStatus.Text = "100%";
            // 
            // c_LayerInspector
            // 
            this.c_LayerInspector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_LayerInspector.Location = new System.Drawing.Point(3, 30);
            this.c_LayerInspector.Name = "c_LayerInspector";
            this.c_LayerInspector.Size = new System.Drawing.Size(194, 417);
            this.c_LayerInspector.TabIndex = 3;
            this.c_LayerInspector.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.c_LayerInspector_PropertyValueChanged);
            // 
            // c_ContextMenuStrip
            // 
            this.c_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_2DToolStripMenuItem,
            this.c_GeneralMenuItem,
            this.c_LandMenuItem,
            this.c_BiomeMenuItem,
            this.c_RainfallMenuItem,
            this.c_TemperatureMenuItem,
            this.c_RiversMenuItem,
            this.c_TownsMenuItem,
            this.c_FamilyTreesMenuItem,
            this.toolStripMenuItem6,
            this.c_3DToolStripMenuItem,
            this.c_3DGeneralMenuItem,
            this.c_3DTerrainMenuItem,
            this.toolStripMenuItem4,
            this.c_RenameSelectedMenuItem,
            this.c_DeleteSelectedMenuItem});
            this.c_ContextMenuStrip.Name = "contextMenuStrip1";
            this.c_ContextMenuStrip.Size = new System.Drawing.Size(165, 346);
            // 
            // c_2DToolStripMenuItem
            // 
            this.c_2DToolStripMenuItem.Enabled = false;
            this.c_2DToolStripMenuItem.Name = "c_2DToolStripMenuItem";
            this.c_2DToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_2DToolStripMenuItem.Text = "2D:";
            // 
            // c_GeneralMenuItem
            // 
            this.c_GeneralMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_GeneralAddInitialPerlinMenuItem,
            this.c_GeneralAddPerlinMathMenuItem,
            this.toolStripMenuItem1,
            this.c_GeneralAddInitialVoronoiMenuItem,
            this.c_GeneralAddMixVoronoiMenuItem,
            this.c_GeneralAddAutoMixVoronoiMenuItem,
            this.toolStripMenuItem3,
            this.c_GeneralAddZoomMenuItem,
            this.c_GeneralAddSmoothMenuItem,
            this.c_GeneralAddRemapMenuItem,
            this.c_GeneralAddInvertMenuItem,
            this.c_GeneralAddNormalizeMenuItem,
            this.c_GeneralAddDenormalizeMenuItem,
            this.c_Seperator2MenuItem,
            this.c_GeneralAddCopyResultMenuItem,
            this.c_GeneralAddStoreResultMenuItem});
            this.c_GeneralMenuItem.Name = "c_GeneralMenuItem";
            this.c_GeneralMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_GeneralMenuItem.Text = "General";
            // 
            // c_GeneralAddInitialPerlinMenuItem
            // 
            this.c_GeneralAddInitialPerlinMenuItem.Name = "c_GeneralAddInitialPerlinMenuItem";
            this.c_GeneralAddInitialPerlinMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddInitialPerlinMenuItem.Text = "Initial Perlin";
            this.c_GeneralAddInitialPerlinMenuItem.Click += new System.EventHandler(this.c_GeneralAddInitialPerlinMenuItem_Click);
            // 
            // c_GeneralAddPerlinMathMenuItem
            // 
            this.c_GeneralAddPerlinMathMenuItem.Name = "c_GeneralAddPerlinMathMenuItem";
            this.c_GeneralAddPerlinMathMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddPerlinMathMenuItem.Text = "Perform Perlin Math";
            this.c_GeneralAddPerlinMathMenuItem.Click += new System.EventHandler(this.c_GeneralAddPerlinMathMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // c_GeneralAddInitialVoronoiMenuItem
            // 
            this.c_GeneralAddInitialVoronoiMenuItem.Name = "c_GeneralAddInitialVoronoiMenuItem";
            this.c_GeneralAddInitialVoronoiMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddInitialVoronoiMenuItem.Text = "Initial Voronoi";
            this.c_GeneralAddInitialVoronoiMenuItem.Click += new System.EventHandler(this.c_GeneralAddInitialVoronoiMenuItem_Click);
            // 
            // c_GeneralAddMixVoronoiMenuItem
            // 
            this.c_GeneralAddMixVoronoiMenuItem.Name = "c_GeneralAddMixVoronoiMenuItem";
            this.c_GeneralAddMixVoronoiMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddMixVoronoiMenuItem.Text = "Mix Voronoi";
            this.c_GeneralAddMixVoronoiMenuItem.Click += new System.EventHandler(this.c_GeneralAddMixVoronoiMenuItem_Click);
            // 
            // c_GeneralAddAutoMixVoronoiMenuItem
            // 
            this.c_GeneralAddAutoMixVoronoiMenuItem.Name = "c_GeneralAddAutoMixVoronoiMenuItem";
            this.c_GeneralAddAutoMixVoronoiMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddAutoMixVoronoiMenuItem.Text = "Auto Mix Voronoi";
            this.c_GeneralAddAutoMixVoronoiMenuItem.Click += new System.EventHandler(this.c_GeneralAddAutoMixVoronoiMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(178, 6);
            // 
            // c_GeneralAddZoomMenuItem
            // 
            this.c_GeneralAddZoomMenuItem.Name = "c_GeneralAddZoomMenuItem";
            this.c_GeneralAddZoomMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddZoomMenuItem.Text = "Zoom";
            this.c_GeneralAddZoomMenuItem.Click += new System.EventHandler(this.c_GeneralAddZoomMenuItem_Click);
            // 
            // c_GeneralAddSmoothMenuItem
            // 
            this.c_GeneralAddSmoothMenuItem.Enabled = false;
            this.c_GeneralAddSmoothMenuItem.Name = "c_GeneralAddSmoothMenuItem";
            this.c_GeneralAddSmoothMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddSmoothMenuItem.Text = "Smooth";
            // 
            // c_GeneralAddRemapMenuItem
            // 
            this.c_GeneralAddRemapMenuItem.Enabled = false;
            this.c_GeneralAddRemapMenuItem.Name = "c_GeneralAddRemapMenuItem";
            this.c_GeneralAddRemapMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddRemapMenuItem.Text = "Remap";
            // 
            // c_GeneralAddInvertMenuItem
            // 
            this.c_GeneralAddInvertMenuItem.Name = "c_GeneralAddInvertMenuItem";
            this.c_GeneralAddInvertMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddInvertMenuItem.Text = "Invert";
            this.c_GeneralAddInvertMenuItem.Click += new System.EventHandler(this.c_GeneralAddInvertMenuItem_Click);
            // 
            // c_GeneralAddNormalizeMenuItem
            // 
            this.c_GeneralAddNormalizeMenuItem.Enabled = false;
            this.c_GeneralAddNormalizeMenuItem.Name = "c_GeneralAddNormalizeMenuItem";
            this.c_GeneralAddNormalizeMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddNormalizeMenuItem.Text = "Normalize";
            // 
            // c_GeneralAddDenormalizeMenuItem
            // 
            this.c_GeneralAddDenormalizeMenuItem.Enabled = false;
            this.c_GeneralAddDenormalizeMenuItem.Name = "c_GeneralAddDenormalizeMenuItem";
            this.c_GeneralAddDenormalizeMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddDenormalizeMenuItem.Text = "Denormalize";
            // 
            // c_Seperator2MenuItem
            // 
            this.c_Seperator2MenuItem.Name = "c_Seperator2MenuItem";
            this.c_Seperator2MenuItem.Size = new System.Drawing.Size(178, 6);
            // 
            // c_GeneralAddCopyResultMenuItem
            // 
            this.c_GeneralAddCopyResultMenuItem.Name = "c_GeneralAddCopyResultMenuItem";
            this.c_GeneralAddCopyResultMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddCopyResultMenuItem.Text = "Copy Result";
            this.c_GeneralAddCopyResultMenuItem.Click += new System.EventHandler(this.c_GeneralAddCopyResultMenuItem_Click);
            // 
            // c_GeneralAddStoreResultMenuItem
            // 
            this.c_GeneralAddStoreResultMenuItem.Name = "c_GeneralAddStoreResultMenuItem";
            this.c_GeneralAddStoreResultMenuItem.Size = new System.Drawing.Size(181, 22);
            this.c_GeneralAddStoreResultMenuItem.Text = "Store Result";
            this.c_GeneralAddStoreResultMenuItem.Click += new System.EventHandler(this.c_GeneralAddStoreResultMenuItem_Click);
            // 
            // c_LandMenuItem
            // 
            this.c_LandMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_LandAddInitialLandMenuItem,
            this.c_LandAddExtendLandMenuItem,
            this.c_LandAddDeriveTerrainMenuItem,
            this.c_LandAddMixTerrainWithPerlinMenuItem,
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem});
            this.c_LandMenuItem.Name = "c_LandMenuItem";
            this.c_LandMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_LandMenuItem.Text = "Land";
            // 
            // c_LandAddInitialLandMenuItem
            // 
            this.c_LandAddInitialLandMenuItem.Name = "c_LandAddInitialLandMenuItem";
            this.c_LandAddInitialLandMenuItem.Size = new System.Drawing.Size(238, 22);
            this.c_LandAddInitialLandMenuItem.Text = "Initial Land";
            this.c_LandAddInitialLandMenuItem.Click += new System.EventHandler(this.c_LandAddInitialLandMenuItem_Click);
            // 
            // c_LandAddExtendLandMenuItem
            // 
            this.c_LandAddExtendLandMenuItem.Name = "c_LandAddExtendLandMenuItem";
            this.c_LandAddExtendLandMenuItem.Size = new System.Drawing.Size(238, 22);
            this.c_LandAddExtendLandMenuItem.Text = "Extend Land";
            this.c_LandAddExtendLandMenuItem.Click += new System.EventHandler(this.c_LandAddExtendLandMenuItem_Click);
            // 
            // c_LandAddDeriveTerrainMenuItem
            // 
            this.c_LandAddDeriveTerrainMenuItem.Name = "c_LandAddDeriveTerrainMenuItem";
            this.c_LandAddDeriveTerrainMenuItem.Size = new System.Drawing.Size(238, 22);
            this.c_LandAddDeriveTerrainMenuItem.Text = "Derive Terrain";
            this.c_LandAddDeriveTerrainMenuItem.Click += new System.EventHandler(this.c_LandAddDeriveTerrainMenuItem_Click);
            // 
            // c_LandAddMixTerrainWithPerlinMenuItem
            // 
            this.c_LandAddMixTerrainWithPerlinMenuItem.Name = "c_LandAddMixTerrainWithPerlinMenuItem";
            this.c_LandAddMixTerrainWithPerlinMenuItem.Size = new System.Drawing.Size(238, 22);
            this.c_LandAddMixTerrainWithPerlinMenuItem.Text = "Mix Terrain with Perlin";
            this.c_LandAddMixTerrainWithPerlinMenuItem.Click += new System.EventHandler(this.c_LandAddMixTerrainWithPerlinMenuItem_Click);
            // 
            // c_LandAddMixOreWithVoronoiMixdownMenuItem
            // 
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem.Name = "c_LandAddMixOreWithVoronoiMixdownMenuItem";
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem.Size = new System.Drawing.Size(238, 22);
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem.Text = "Mix Ore with Voronoi Mixdown";
            this.c_LandAddMixOreWithVoronoiMixdownMenuItem.Click += new System.EventHandler(this.c_LandAddMixOreWithVoronoiMixdownMenuItem_Click);
            // 
            // c_BiomeMenuItem
            // 
            this.c_BiomeMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_BiomeAddScatterBiomeMenuItem,
            this.c_BiomeAddSecondaryBiomeMenuItem});
            this.c_BiomeMenuItem.Name = "c_BiomeMenuItem";
            this.c_BiomeMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_BiomeMenuItem.Text = "Biome";
            // 
            // c_BiomeAddScatterBiomeMenuItem
            // 
            this.c_BiomeAddScatterBiomeMenuItem.Name = "c_BiomeAddScatterBiomeMenuItem";
            this.c_BiomeAddScatterBiomeMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_BiomeAddScatterBiomeMenuItem.Text = "Scatter Primary Biomes";
            this.c_BiomeAddScatterBiomeMenuItem.Click += new System.EventHandler(this.c_BiomeAddScatterBiomeMenuItem_Click);
            // 
            // c_BiomeAddSecondaryBiomeMenuItem
            // 
            this.c_BiomeAddSecondaryBiomeMenuItem.Name = "c_BiomeAddSecondaryBiomeMenuItem";
            this.c_BiomeAddSecondaryBiomeMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_BiomeAddSecondaryBiomeMenuItem.Text = "Form Secondary Biomes";
            this.c_BiomeAddSecondaryBiomeMenuItem.Click += new System.EventHandler(this.c_BiomeAddSecondaryBiomeMenuItem_Click);
            // 
            // c_RainfallMenuItem
            // 
            this.c_RainfallMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_RainfallAddInitialRainfallMenuItem,
            this.c_RainfallAddMixRainfallWithBiomeMenuItem});
            this.c_RainfallMenuItem.Enabled = false;
            this.c_RainfallMenuItem.Name = "c_RainfallMenuItem";
            this.c_RainfallMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_RainfallMenuItem.Text = "Rainfall";
            // 
            // c_RainfallAddInitialRainfallMenuItem
            // 
            this.c_RainfallAddInitialRainfallMenuItem.Name = "c_RainfallAddInitialRainfallMenuItem";
            this.c_RainfallAddInitialRainfallMenuItem.Size = new System.Drawing.Size(198, 22);
            this.c_RainfallAddInitialRainfallMenuItem.Text = "Initial Rainfall";
            // 
            // c_RainfallAddMixRainfallWithBiomeMenuItem
            // 
            this.c_RainfallAddMixRainfallWithBiomeMenuItem.Name = "c_RainfallAddMixRainfallWithBiomeMenuItem";
            this.c_RainfallAddMixRainfallWithBiomeMenuItem.Size = new System.Drawing.Size(198, 22);
            this.c_RainfallAddMixRainfallWithBiomeMenuItem.Text = "Mix Rainfall with Biome";
            // 
            // c_TemperatureMenuItem
            // 
            this.c_TemperatureMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_TemperatureAddInitialTemperature,
            this.c_TemperatureAddMixTemperatureWithBiomeMenuItem});
            this.c_TemperatureMenuItem.Enabled = false;
            this.c_TemperatureMenuItem.Name = "c_TemperatureMenuItem";
            this.c_TemperatureMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_TemperatureMenuItem.Text = "Temperature";
            // 
            // c_TemperatureAddInitialTemperature
            // 
            this.c_TemperatureAddInitialTemperature.Name = "c_TemperatureAddInitialTemperature";
            this.c_TemperatureAddInitialTemperature.Size = new System.Drawing.Size(227, 22);
            this.c_TemperatureAddInitialTemperature.Text = "Initial Temperature";
            // 
            // c_TemperatureAddMixTemperatureWithBiomeMenuItem
            // 
            this.c_TemperatureAddMixTemperatureWithBiomeMenuItem.Name = "c_TemperatureAddMixTemperatureWithBiomeMenuItem";
            this.c_TemperatureAddMixTemperatureWithBiomeMenuItem.Size = new System.Drawing.Size(227, 22);
            this.c_TemperatureAddMixTemperatureWithBiomeMenuItem.Text = "Mix Temperature with Biome";
            // 
            // c_RiversMenuItem
            // 
            this.c_RiversMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_RiversAddSimulateFlowMenuItem,
            this.c_RiversAddPoolLakesMenuItem,
            this.c_RiversAddPoolOceanMenuItem});
            this.c_RiversMenuItem.Enabled = false;
            this.c_RiversMenuItem.Name = "c_RiversMenuItem";
            this.c_RiversMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_RiversMenuItem.Text = "Rivers";
            // 
            // c_RiversAddSimulateFlowMenuItem
            // 
            this.c_RiversAddSimulateFlowMenuItem.Name = "c_RiversAddSimulateFlowMenuItem";
            this.c_RiversAddSimulateFlowMenuItem.Size = new System.Drawing.Size(148, 22);
            this.c_RiversAddSimulateFlowMenuItem.Text = "Simulate Flow";
            // 
            // c_RiversAddPoolLakesMenuItem
            // 
            this.c_RiversAddPoolLakesMenuItem.Name = "c_RiversAddPoolLakesMenuItem";
            this.c_RiversAddPoolLakesMenuItem.Size = new System.Drawing.Size(148, 22);
            this.c_RiversAddPoolLakesMenuItem.Text = "Pool Lakes";
            // 
            // c_RiversAddPoolOceanMenuItem
            // 
            this.c_RiversAddPoolOceanMenuItem.Name = "c_RiversAddPoolOceanMenuItem";
            this.c_RiversAddPoolOceanMenuItem.Size = new System.Drawing.Size(148, 22);
            this.c_RiversAddPoolOceanMenuItem.Text = "Pool Ocean";
            // 
            // c_TownsMenuItem
            // 
            this.c_TownsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_TownsAddScatterTownsMenuItem,
            this.c_TownsAddEraseTownsOverOceanMenuItem,
            this.c_TownsAddDetermineViabilityMenuItem,
            this.c_TownsAddMixTownsWithViabilityMenuItem,
            this.c_TownsAddSimulateRundownMenuItem,
            this.extendTownsToolStripMenuItem,
            this.c_LandAddZoomTownCentersMenuItem,
            this.buildingPlacerPlacerToolStripMenuItem});
            this.c_TownsMenuItem.Name = "c_TownsMenuItem";
            this.c_TownsMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_TownsMenuItem.Text = "Towns";
            // 
            // c_TownsAddScatterTownsMenuItem
            // 
            this.c_TownsAddScatterTownsMenuItem.Name = "c_TownsAddScatterTownsMenuItem";
            this.c_TownsAddScatterTownsMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_TownsAddScatterTownsMenuItem.Text = "Scatter Towns";
            this.c_TownsAddScatterTownsMenuItem.Click += new System.EventHandler(this.c_TownsAddScatterTownsMenuItem_Click);
            // 
            // c_TownsAddEraseTownsOverOceanMenuItem
            // 
            this.c_TownsAddEraseTownsOverOceanMenuItem.Name = "c_TownsAddEraseTownsOverOceanMenuItem";
            this.c_TownsAddEraseTownsOverOceanMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_TownsAddEraseTownsOverOceanMenuItem.Text = "Erase Towns over Ocean";
            this.c_TownsAddEraseTownsOverOceanMenuItem.Click += new System.EventHandler(this.c_TownsAddEraseTownsOverOceanMenuItem_Click);
            // 
            // c_TownsAddDetermineViabilityMenuItem
            // 
            this.c_TownsAddDetermineViabilityMenuItem.Name = "c_TownsAddDetermineViabilityMenuItem";
            this.c_TownsAddDetermineViabilityMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_TownsAddDetermineViabilityMenuItem.Text = "Determine Town Type";
            this.c_TownsAddDetermineViabilityMenuItem.Click += new System.EventHandler(this.c_TownsAddDetermineViabilityMenuItem_Click);
            // 
            // c_TownsAddMixTownsWithViabilityMenuItem
            // 
            this.c_TownsAddMixTownsWithViabilityMenuItem.Name = "c_TownsAddMixTownsWithViabilityMenuItem";
            this.c_TownsAddMixTownsWithViabilityMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_TownsAddMixTownsWithViabilityMenuItem.Text = "Mix Towns with Viability";
            this.c_TownsAddMixTownsWithViabilityMenuItem.Click += new System.EventHandler(this.c_TownsAddMixTownsWithViabilityMenuItem_Click);
            // 
            // c_TownsAddSimulateRundownMenuItem
            // 
            this.c_TownsAddSimulateRundownMenuItem.Name = "c_TownsAddSimulateRundownMenuItem";
            this.c_TownsAddSimulateRundownMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_TownsAddSimulateRundownMenuItem.Text = "Simulate Rundown";
            // 
            // extendTownsToolStripMenuItem
            // 
            this.extendTownsToolStripMenuItem.Name = "extendTownsToolStripMenuItem";
            this.extendTownsToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.extendTownsToolStripMenuItem.Text = "Extend Towns";
            this.extendTownsToolStripMenuItem.Click += new System.EventHandler(this.extendTownsToolStripMenuItem_Click);
            // 
            // c_LandAddZoomTownCentersMenuItem
            // 
            this.c_LandAddZoomTownCentersMenuItem.Name = "c_LandAddZoomTownCentersMenuItem";
            this.c_LandAddZoomTownCentersMenuItem.Size = new System.Drawing.Size(202, 22);
            this.c_LandAddZoomTownCentersMenuItem.Text = "Zoom Town Centers";
            this.c_LandAddZoomTownCentersMenuItem.Click += new System.EventHandler(this.c_LandAddZoomTownCentersMenuItem_Click);
            // 
            // buildingPlacerPlacerToolStripMenuItem
            // 
            this.buildingPlacerPlacerToolStripMenuItem.Name = "buildingPlacerPlacerToolStripMenuItem";
            this.buildingPlacerPlacerToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.buildingPlacerPlacerToolStripMenuItem.Text = "Building Lister";
            this.buildingPlacerPlacerToolStripMenuItem.Click += new System.EventHandler(this.buildingPlacerPlacerToolStripMenuItem_Click);
            // 
            // c_FamilyTreesMenuItem
            // 
            this.c_FamilyTreesMenuItem.Enabled = false;
            this.c_FamilyTreesMenuItem.Name = "c_FamilyTreesMenuItem";
            this.c_FamilyTreesMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_FamilyTreesMenuItem.Text = "Family Trees";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(161, 6);
            // 
            // c_3DToolStripMenuItem
            // 
            this.c_3DToolStripMenuItem.Enabled = false;
            this.c_3DToolStripMenuItem.Name = "c_3DToolStripMenuItem";
            this.c_3DToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_3DToolStripMenuItem.Text = "3D:";
            // 
            // c_3DGeneralMenuItem
            // 
            this.c_3DGeneralMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_3DGeneralStoreResultMenuItem});
            this.c_3DGeneralMenuItem.Name = "c_3DGeneralMenuItem";
            this.c_3DGeneralMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_3DGeneralMenuItem.Text = "General";
            // 
            // c_3DGeneralStoreResultMenuItem
            // 
            this.c_3DGeneralStoreResultMenuItem.Name = "c_3DGeneralStoreResultMenuItem";
            this.c_3DGeneralStoreResultMenuItem.Size = new System.Drawing.Size(136, 22);
            this.c_3DGeneralStoreResultMenuItem.Text = "Store Result";
            this.c_3DGeneralStoreResultMenuItem.Click += new System.EventHandler(this.c_3DGeneralStoreResultMenuItem_Click);
            // 
            // c_3DTerrainMenuItem
            // 
            this.c_3DTerrainMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_3DTerrainAddForm3DTerrainMenuItem});
            this.c_3DTerrainMenuItem.Name = "c_3DTerrainMenuItem";
            this.c_3DTerrainMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_3DTerrainMenuItem.Text = "Terrain";
            // 
            // c_3DTerrainAddForm3DTerrainMenuItem
            // 
            this.c_3DTerrainAddForm3DTerrainMenuItem.Name = "c_3DTerrainAddForm3DTerrainMenuItem";
            this.c_3DTerrainAddForm3DTerrainMenuItem.Size = new System.Drawing.Size(159, 22);
            this.c_3DTerrainAddForm3DTerrainMenuItem.Text = "Form 3D Terrain";
            this.c_3DTerrainAddForm3DTerrainMenuItem.Click += new System.EventHandler(this.c_TerrainAddForm3DTerrainMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(161, 6);
            // 
            // c_RenameSelectedMenuItem
            // 
            this.c_RenameSelectedMenuItem.Enabled = false;
            this.c_RenameSelectedMenuItem.Name = "c_RenameSelectedMenuItem";
            this.c_RenameSelectedMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_RenameSelectedMenuItem.Text = "Rename Selected";
            this.c_RenameSelectedMenuItem.Click += new System.EventHandler(this.c_RenameSelectedMenuItem_Click);
            // 
            // c_DeleteSelectedMenuItem
            // 
            this.c_DeleteSelectedMenuItem.Enabled = false;
            this.c_DeleteSelectedMenuItem.Name = "c_DeleteSelectedMenuItem";
            this.c_DeleteSelectedMenuItem.Size = new System.Drawing.Size(164, 22);
            this.c_DeleteSelectedMenuItem.Text = "Delete Selected";
            this.c_DeleteSelectedMenuItem.Click += new System.EventHandler(this.c_DeleteSelectedMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(6, 6);
            // 
            // c_ToolStrip
            // 
            this.c_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_LoadConfigurationButton,
            this.c_SaveConfigurationButton,
            this.c_SaveConfigurationAsButton});
            this.c_ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.c_ToolStrip.Name = "c_ToolStrip";
            this.c_ToolStrip.Size = new System.Drawing.Size(890, 25);
            this.c_ToolStrip.TabIndex = 4;
            this.c_ToolStrip.Text = "toolStrip1";
            // 
            // c_LoadConfigurationButton
            // 
            this.c_LoadConfigurationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.c_LoadConfigurationButton.Image = ((System.Drawing.Image)(resources.GetObject("c_LoadConfigurationButton.Image")));
            this.c_LoadConfigurationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.c_LoadConfigurationButton.Name = "c_LoadConfigurationButton";
            this.c_LoadConfigurationButton.Size = new System.Drawing.Size(23, 22);
            this.c_LoadConfigurationButton.Text = "Load Configuration";
            this.c_LoadConfigurationButton.Click += new System.EventHandler(this.c_LoadConfigurationButton_Click);
            // 
            // c_SaveConfigurationButton
            // 
            this.c_SaveConfigurationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.c_SaveConfigurationButton.Enabled = false;
            this.c_SaveConfigurationButton.Image = ((System.Drawing.Image)(resources.GetObject("c_SaveConfigurationButton.Image")));
            this.c_SaveConfigurationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.c_SaveConfigurationButton.Name = "c_SaveConfigurationButton";
            this.c_SaveConfigurationButton.Size = new System.Drawing.Size(23, 22);
            this.c_SaveConfigurationButton.Text = "Save Configuration";
            this.c_SaveConfigurationButton.Click += new System.EventHandler(this.c_SaveConfigurationButton_Click);
            // 
            // c_SaveConfigurationAsButton
            // 
            this.c_SaveConfigurationAsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.c_SaveConfigurationAsButton.Image = ((System.Drawing.Image)(resources.GetObject("c_SaveConfigurationAsButton.Image")));
            this.c_SaveConfigurationAsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.c_SaveConfigurationAsButton.Name = "c_SaveConfigurationAsButton";
            this.c_SaveConfigurationAsButton.Size = new System.Drawing.Size(23, 22);
            this.c_SaveConfigurationAsButton.Text = "Save Configuration As...";
            this.c_SaveConfigurationAsButton.Click += new System.EventHandler(this.c_SaveConfigurationAsButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.c_LayerInspector, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(690, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 450);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.c_YLabel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_XLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_XNumericUpDown, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_YNumericUpDown, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 27);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // c_YLabel
            // 
            this.c_YLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_YLabel.Location = new System.Drawing.Point(103, 5);
            this.c_YLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.c_YLabel.Name = "c_YLabel";
            this.c_YLabel.Size = new System.Drawing.Size(19, 19);
            this.c_YLabel.TabIndex = 3;
            this.c_YLabel.Text = "Y:";
            // 
            // c_XLabel
            // 
            this.c_XLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_XLabel.Location = new System.Drawing.Point(3, 5);
            this.c_XLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.c_XLabel.Name = "c_XLabel";
            this.c_XLabel.Size = new System.Drawing.Size(19, 19);
            this.c_XLabel.TabIndex = 0;
            this.c_XLabel.Text = "X:";
            // 
            // c_XNumericUpDown
            // 
            this.c_XNumericUpDown.Location = new System.Drawing.Point(28, 3);
            this.c_XNumericUpDown.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.c_XNumericUpDown.Minimum = new decimal(new int[] {
            6000,
            0,
            0,
            -2147483648});
            this.c_XNumericUpDown.Name = "c_XNumericUpDown";
            this.c_XNumericUpDown.Size = new System.Drawing.Size(69, 20);
            this.c_XNumericUpDown.TabIndex = 1;
            this.c_XNumericUpDown.ValueChanged += new System.EventHandler(this.c_XNumericUpDown_ValueChanged);
            // 
            // c_YNumericUpDown
            // 
            this.c_YNumericUpDown.Location = new System.Drawing.Point(128, 3);
            this.c_YNumericUpDown.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.c_YNumericUpDown.Minimum = new decimal(new int[] {
            6000,
            0,
            0,
            -2147483648});
            this.c_YNumericUpDown.Name = "c_YNumericUpDown";
            this.c_YNumericUpDown.Size = new System.Drawing.Size(69, 20);
            this.c_YNumericUpDown.TabIndex = 2;
            this.c_YNumericUpDown.ValueChanged += new System.EventHandler(this.c_YNumericUpDown_ValueChanged);
            // 
            // c_FlowInterfaceControl
            // 
            this.c_FlowInterfaceControl.ContextMenuStrip = this.c_ContextMenuStrip;
            this.c_FlowInterfaceControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_FlowInterfaceControl.Location = new System.Drawing.Point(0, 25);
            this.c_FlowInterfaceControl.Name = "c_FlowInterfaceControl";
            this.c_FlowInterfaceControl.SelectedElement = null;
            this.c_FlowInterfaceControl.Size = new System.Drawing.Size(890, 472);
            this.c_FlowInterfaceControl.TabIndex = 0;
            this.c_FlowInterfaceControl.Zoom = 1F;
            this.c_FlowInterfaceControl.SelectedElementChanged += new System.EventHandler(this.c_FlowInterfaceControl_SelectedElementChanged);
            this.c_FlowInterfaceControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.c_FlowInterfaceControl_MouseWheel);
            // 
            // FlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 497);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.c_StatusStrip);
            this.Controls.Add(this.c_FlowInterfaceControl);
            this.Controls.Add(this.c_ToolStrip);
            this.Name = "FlowForm";
            this.Text = "Tychaia World Experimentation Tool";
            this.c_StatusStrip.ResumeLayout(false);
            this.c_StatusStrip.PerformLayout();
            this.c_ContextMenuStrip.ResumeLayout(false);
            this.c_ToolStrip.ResumeLayout(false);
            this.c_ToolStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c_XNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_YNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Flow.FlowInterfaceControl c_FlowInterfaceControl;
        private System.Windows.Forms.StatusStrip c_StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel c_ZoomStatus;
        private System.Windows.Forms.PropertyGrid c_LayerInspector;
        private System.Windows.Forms.ContextMenuStrip c_ContextMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem c_BiomeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_BiomeAddScatterBiomeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RainfallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RainfallAddInitialRainfallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RainfallAddMixRainfallWithBiomeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TemperatureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TemperatureAddInitialTemperature;
        private System.Windows.Forms.ToolStripMenuItem c_TemperatureAddMixTemperatureWithBiomeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsAddScatterTownsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsAddDetermineViabilityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsAddMixTownsWithViabilityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsAddSimulateRundownMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_FamilyTreesMenuItem;
        private System.Windows.Forms.ToolStrip c_ToolStrip;
        private System.Windows.Forms.ToolStripButton c_LoadConfigurationButton;
        private System.Windows.Forms.ToolStripButton c_SaveConfigurationButton;
        private System.Windows.Forms.ToolStripButton c_SaveConfigurationAsButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label c_YLabel;
        private System.Windows.Forms.Label c_XLabel;
        private System.Windows.Forms.NumericUpDown c_XNumericUpDown;
        private System.Windows.Forms.NumericUpDown c_YNumericUpDown;
        private System.Windows.Forms.ToolStripMenuItem c_RiversMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RiversAddSimulateFlowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RiversAddPoolLakesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RiversAddPoolOceanMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddInitialLandMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddExtendLandMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddDeriveTerrainMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddMixTerrainWithPerlinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_BiomeAddSecondaryBiomeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem c_DeleteSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TownsAddEraseTownsOverOceanMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_2DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddInitialPerlinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddPerlinMathMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddInitialVoronoiMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddMixVoronoiMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddAutoMixVoronoiMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddZoomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddSmoothMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddRemapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddInvertMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddNormalizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddDenormalizeMenuItem;
        private System.Windows.Forms.ToolStripSeparator c_Seperator2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddStoreResultMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem c_3DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_3DTerrainMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_3DTerrainAddForm3DTerrainMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RenameSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddMixOreWithVoronoiMixdownMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_GeneralAddCopyResultMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extendTownsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_3DGeneralMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_3DGeneralStoreResultMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_LandAddZoomTownCentersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildingPlacerPlacerToolStripMenuItem;
    }
}