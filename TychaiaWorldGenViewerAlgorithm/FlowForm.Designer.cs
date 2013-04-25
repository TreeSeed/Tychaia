using System.Collections.Generic;
using System;
using Tychaia.ProceduralGeneration;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
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
            this.c_QueueStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.c_LayerInspector = new System.Windows.Forms.PropertyGrid();
            this.c_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.c_DisableProcessingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_ExportSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_TraceSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_AnalyseSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_RenameSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.c_DeleteSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.c_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.c_LoadConfigurationButton = new System.Windows.Forms.ToolStripButton();
            this.c_SaveConfigurationButton = new System.Windows.Forms.ToolStripButton();
            this.c_SaveConfigurationAsButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.c_ZNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.c_YLabel = new System.Windows.Forms.Label();
            this.c_XLabel = new System.Windows.Forms.Label();
            this.c_XNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.c_YNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.c_ZLabel = new System.Windows.Forms.Label();
            this.c_FlowInterfaceControl = new FlowInterfaceControl();
            this.c_StatusStrip.SuspendLayout();
            this.c_ContextMenuStrip.SuspendLayout();
            this.c_ToolStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c_ZNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_XNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_YNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // c_StatusStrip
            // 
            this.c_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.c_ZoomStatus, this.c_QueueStatus});
            this.c_StatusStrip.Location = new System.Drawing.Point(0, 587);
            this.c_StatusStrip.Name = "c_StatusStrip";
            this.c_StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.c_StatusStrip.Size = new System.Drawing.Size(1187, 25);
            this.c_StatusStrip.TabIndex = 2;
            this.c_StatusStrip.Text = "statusStrip1";
            // 
            // c_ZoomStatus
            // 
            this.c_ZoomStatus.Name = "c_ZoomStatus";
            this.c_ZoomStatus.Size = new System.Drawing.Size(45, 20);
            this.c_ZoomStatus.Text = "100%";
            // 
            // c_QueueStatus
            // 
            this.c_QueueStatus.Name = "c_QueueStatus";
            this.c_QueueStatus.Size = new System.Drawing.Size(200, 20);
            this.c_QueueStatus.Text = "";
            // 
            // c_LayerInspector
            // 
            this.c_LayerInspector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_LayerInspector.Location = new System.Drawing.Point(4, 37);
            this.c_LayerInspector.Margin = new System.Windows.Forms.Padding(4);
            this.c_LayerInspector.Name = "c_LayerInspector";
            this.c_LayerInspector.Size = new System.Drawing.Size(259, 521);
            this.c_LayerInspector.TabIndex = 3;
            this.c_LayerInspector.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.c_LayerInspector_PropertyValueChanged);
            // 
            // c_ContextMenuStrip
            // 
            this.CreateMenuItems(this.c_ContextMenuStrip);
            this.c_ContextMenuStrip.Name = "contextMenuStrip1";
            this.c_ContextMenuStrip.Size = new System.Drawing.Size(203, 446);
            // 
            // c_DisableProcessingMenuItem
            // 
            this.c_DisableProcessingMenuItem.Enabled = false;
            this.c_DisableProcessingMenuItem.Name = "c_DisableProcessingMenuItem";
            this.c_DisableProcessingMenuItem.Size = new System.Drawing.Size(202, 24);
            this.c_DisableProcessingMenuItem.Text = "Disable Processing";
            this.c_DisableProcessingMenuItem.Click += new System.EventHandler(this.c_DisableProcessingMenuItem_Click);
            // 
            // c_ExportSelectedMenuItem
            // 
            this.c_ExportSelectedMenuItem.Enabled = false;
            this.c_ExportSelectedMenuItem.Name = "c_ExportSelectedMenuItem";
            this.c_ExportSelectedMenuItem.Size = new System.Drawing.Size(202, 24);
            this.c_ExportSelectedMenuItem.Text = "Export Selected";
            this.c_ExportSelectedMenuItem.Click += new System.EventHandler(this.c_ExportSelectedMenuItem_Click);
            // 
            // c_AnalyseSelectedMenuItem
            // 
            this.c_AnalyseSelectedMenuItem.Enabled = false;
            this.c_AnalyseSelectedMenuItem.Name = "c_AnalyseSelectedMenuItem";
            this.c_AnalyseSelectedMenuItem.Size = new System.Drawing.Size(202, 24);
            this.c_AnalyseSelectedMenuItem.Text = "Analyse Selected";
            this.c_AnalyseSelectedMenuItem.Click += new System.EventHandler(this.c_AnalyseSelectedMenuItem_Click);
            // 
            // c_TraceSelectedMenuItem
            // 
            this.c_TraceSelectedMenuItem.Enabled = false;
            this.c_TraceSelectedMenuItem.Name = "c_TraceSelectedMenuItem";
            this.c_TraceSelectedMenuItem.Size = new System.Drawing.Size(202, 24);
            this.c_TraceSelectedMenuItem.Text = "Trace Selected";
            this.c_TraceSelectedMenuItem.Click += new System.EventHandler(this.c_TraceSelectedMenuItem_Click);
            // 
            // c_RenameSelectedMenuItem
            // 
            this.c_RenameSelectedMenuItem.Enabled = false;
            this.c_RenameSelectedMenuItem.Name = "c_RenameSelectedMenuItem";
            this.c_RenameSelectedMenuItem.Size = new System.Drawing.Size(202, 24);
            this.c_RenameSelectedMenuItem.Text = "Rename Selected";
            this.c_RenameSelectedMenuItem.Click += new System.EventHandler(this.c_RenameSelectedMenuItem_Click);
            // 
            // c_DeleteSelectedMenuItem
            // 
            this.c_DeleteSelectedMenuItem.Enabled = false;
            this.c_DeleteSelectedMenuItem.Name = "c_DeleteSelectedMenuItem";
            this.c_DeleteSelectedMenuItem.Size = new System.Drawing.Size(202, 24);
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
            this.c_ToolStrip.Size = new System.Drawing.Size(1187, 25);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(920, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(267, 562);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.c_ZNumericUpDown, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_YLabel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_XLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_XNumericUpDown, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_YNumericUpDown, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.c_ZLabel, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(267, 33);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // c_ZNumericUpDown
            // 
            this.c_ZNumericUpDown.Location = new System.Drawing.Point(213, 4);
            this.c_ZNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.c_ZNumericUpDown.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.c_ZNumericUpDown.Minimum = new decimal(new int[] {
            6000,
            0,
            0,
            -2147483648});
            this.c_ZNumericUpDown.Name = "c_ZNumericUpDown";
            this.c_ZNumericUpDown.Size = new System.Drawing.Size(49, 22);
            this.c_ZNumericUpDown.TabIndex = 5;
            this.c_ZNumericUpDown.ValueChanged += new System.EventHandler(this.c_ZNumericUpDown_ValueChanged);
            // 
            // c_YLabel
            // 
            this.c_YLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_YLabel.Location = new System.Drawing.Point(95, 6);
            this.c_YLabel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 4);
            this.c_YLabel.Name = "c_YLabel";
            this.c_YLabel.Size = new System.Drawing.Size(25, 23);
            this.c_YLabel.TabIndex = 3;
            this.c_YLabel.Text = "Y:";
            // 
            // c_XLabel
            // 
            this.c_XLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_XLabel.Location = new System.Drawing.Point(4, 6);
            this.c_XLabel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 4);
            this.c_XLabel.Name = "c_XLabel";
            this.c_XLabel.Size = new System.Drawing.Size(25, 23);
            this.c_XLabel.TabIndex = 0;
            this.c_XLabel.Text = "X:";
            // 
            // c_XNumericUpDown
            // 
            this.c_XNumericUpDown.Location = new System.Drawing.Point(37, 4);
            this.c_XNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
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
            this.c_XNumericUpDown.Size = new System.Drawing.Size(49, 22);
            this.c_XNumericUpDown.TabIndex = 1;
            this.c_XNumericUpDown.ValueChanged += new System.EventHandler(this.c_XNumericUpDown_ValueChanged);
            // 
            // c_YNumericUpDown
            // 
            this.c_YNumericUpDown.Location = new System.Drawing.Point(128, 4);
            this.c_YNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
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
            this.c_YNumericUpDown.Size = new System.Drawing.Size(49, 22);
            this.c_YNumericUpDown.TabIndex = 2;
            this.c_YNumericUpDown.ValueChanged += new System.EventHandler(this.c_YNumericUpDown_ValueChanged);
            // 
            // c_ZLabel
            // 
            this.c_ZLabel.Location = new System.Drawing.Point(186, 6);
            this.c_ZLabel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 4);
            this.c_ZLabel.Name = "c_ZLabel";
            this.c_ZLabel.Size = new System.Drawing.Size(19, 23);
            this.c_ZLabel.TabIndex = 4;
            this.c_ZLabel.Text = "Z:";
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
            this.c_FlowInterfaceControl.ElementsInQueueCountChanged += new FlowInterfaceControl.ElementsInQueueCountChangedHandler(this.c_FlowInterfaceControl_ElementsInQueueCountChanged);
            this.c_FlowInterfaceControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.c_FlowInterfaceControl_MouseWheel);
            // 
            // FlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 612);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.c_StatusStrip);
            this.Controls.Add(this.c_FlowInterfaceControl);
            this.Controls.Add(this.c_ToolStrip);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FlowForm";
            this.Text = "Tychaia World Experimentation Tool";
            this.c_StatusStrip.ResumeLayout(false);
            this.c_StatusStrip.PerformLayout();
            this.c_ContextMenuStrip.ResumeLayout(false);
            this.c_ToolStrip.ResumeLayout(false);
            this.c_ToolStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c_ZNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_XNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_YNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowInterfaceControl c_FlowInterfaceControl;
        private System.Windows.Forms.StatusStrip c_StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel c_ZoomStatus;
        private System.Windows.Forms.ToolStripStatusLabel c_QueueStatus;
        private System.Windows.Forms.PropertyGrid c_LayerInspector;
        private System.Windows.Forms.ContextMenuStrip c_ContextMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
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
        private System.Windows.Forms.ToolStripMenuItem c_DeleteSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_RenameSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_DisableProcessingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_ExportSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_TraceSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c_AnalyseSelectedMenuItem;
        private System.Windows.Forms.NumericUpDown c_ZNumericUpDown;
        private System.Windows.Forms.Label c_ZLabel;
    }
}