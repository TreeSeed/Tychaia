// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace TychaiaWorldGenViewerAlgorithm
{
    partial class AnalyseForm
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
            this.c_AnalysisToPerformList = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.c_AddAnalysisButton = new System.Windows.Forms.Button();
            this.c_RemoveAnalysisButton = new System.Windows.Forms.Button();
            this.c_AnalysisProperties = new System.Windows.Forms.PropertyGrid();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.c_GoAnalysisButton = new System.Windows.Forms.Button();
            this.c_AnalysisAddOptionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_AnalysisToPerformList
            // 
            this.c_AnalysisToPerformList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.c_AnalysisToPerformList.FormattingEnabled = true;
            this.c_AnalysisToPerformList.IntegralHeight = false;
            this.c_AnalysisToPerformList.Location = new System.Drawing.Point(12, 12);
            this.c_AnalysisToPerformList.Name = "c_AnalysisToPerformList";
            this.c_AnalysisToPerformList.Size = new System.Drawing.Size(341, 287);
            this.c_AnalysisToPerformList.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.Controls.Add(this.c_AddAnalysisButton);
            this.flowLayoutPanel1.Controls.Add(this.c_RemoveAnalysisButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 305);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(341, 27);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // c_AddAnalysisButton
            // 
            this.c_AddAnalysisButton.Location = new System.Drawing.Point(3, 3);
            this.c_AddAnalysisButton.Name = "c_AddAnalysisButton";
            this.c_AddAnalysisButton.Size = new System.Drawing.Size(75, 23);
            this.c_AddAnalysisButton.TabIndex = 0;
            this.c_AddAnalysisButton.Text = "Add";
            this.c_AddAnalysisButton.UseVisualStyleBackColor = true;
            this.c_AddAnalysisButton.Click += new System.EventHandler(this.c_AddAnalysisButton_Click);
            // 
            // c_RemoveAnalysisButton
            // 
            this.c_RemoveAnalysisButton.Location = new System.Drawing.Point(84, 3);
            this.c_RemoveAnalysisButton.Name = "c_RemoveAnalysisButton";
            this.c_RemoveAnalysisButton.Size = new System.Drawing.Size(75, 23);
            this.c_RemoveAnalysisButton.TabIndex = 1;
            this.c_RemoveAnalysisButton.Text = "Remove";
            this.c_RemoveAnalysisButton.UseVisualStyleBackColor = true;
            // 
            // c_AnalysisProperties
            // 
            this.c_AnalysisProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.c_AnalysisProperties.Location = new System.Drawing.Point(359, 12);
            this.c_AnalysisProperties.Name = "c_AnalysisProperties";
            this.c_AnalysisProperties.Size = new System.Drawing.Size(223, 287);
            this.c_AnalysisProperties.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.c_GoAnalysisButton);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(359, 305);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(223, 27);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // c_GoAnalysisButton
            // 
            this.c_GoAnalysisButton.Location = new System.Drawing.Point(126, 3);
            this.c_GoAnalysisButton.Name = "c_GoAnalysisButton";
            this.c_GoAnalysisButton.Size = new System.Drawing.Size(94, 23);
            this.c_GoAnalysisButton.TabIndex = 0;
            this.c_GoAnalysisButton.Text = "Run Analysis";
            this.c_GoAnalysisButton.UseVisualStyleBackColor = true;
            // 
            // c_AnalysisAddOptionsMenu
            // 
            this.c_AnalysisAddOptionsMenu.Name = "c_AnalysisAddOptionsMenu";
            this.c_AnalysisAddOptionsMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // AnalyseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 344);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.c_AnalysisProperties);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.c_AnalysisToPerformList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AnalyseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analyse Algorithm";
            this.Load += new System.EventHandler(this.AnalysisForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox c_AnalysisToPerformList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button c_AddAnalysisButton;
        private System.Windows.Forms.Button c_RemoveAnalysisButton;
        private System.Windows.Forms.PropertyGrid c_AnalysisProperties;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button c_GoAnalysisButton;
        private System.Windows.Forms.ContextMenuStrip c_AnalysisAddOptionsMenu;
    }
}
