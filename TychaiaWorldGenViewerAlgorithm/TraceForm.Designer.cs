// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace TychaiaWorldGenViewerAlgorithm
{
    partial class TraceForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.c_TraceScrollbar = new System.Windows.Forms.HScrollBar();
            this.c_PerformTrace = new System.Windows.Forms.Button();
            this.c_TraceProgress = new System.Windows.Forms.ProgressBar();
            this.c_TraceImage = new System.Windows.Forms.PictureBox();
            this.c_OnlyComparisonDataCheckBox = new System.Windows.Forms.CheckBox();
            this.c_GenerateGIF = new System.Windows.Forms.CheckBox();
            this.c_FormZoomSize = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.c_TraceImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(387, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "This tool allows you to diagnose continuity issues with algorithms by tracing eac" +
    "h step the generation.";
            // 
            // c_TraceScrollbar
            // 
            this.c_TraceScrollbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.c_TraceScrollbar.Enabled = false;
            this.c_TraceScrollbar.LargeChange = 1;
            this.c_TraceScrollbar.Location = new System.Drawing.Point(15, 111);
            this.c_TraceScrollbar.Maximum = 10;
            this.c_TraceScrollbar.Name = "c_TraceScrollbar";
            this.c_TraceScrollbar.Size = new System.Drawing.Size(384, 22);
            this.c_TraceScrollbar.TabIndex = 1;
            this.c_TraceScrollbar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.c_TraceScrollbar_Scroll);
            // 
            // c_PerformTrace
            // 
            this.c_PerformTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.c_PerformTrace.Location = new System.Drawing.Point(316, 75);
            this.c_PerformTrace.Name = "c_PerformTrace";
            this.c_PerformTrace.Size = new System.Drawing.Size(83, 24);
            this.c_PerformTrace.TabIndex = 2;
            this.c_PerformTrace.Text = "Perform Trace";
            this.c_PerformTrace.UseVisualStyleBackColor = true;
            this.c_PerformTrace.Click += new System.EventHandler(this.c_PerformTrace_Click);
            // 
            // c_TraceProgress
            // 
            this.c_TraceProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.c_TraceProgress.Location = new System.Drawing.Point(15, 75);
            this.c_TraceProgress.Name = "c_TraceProgress";
            this.c_TraceProgress.Size = new System.Drawing.Size(295, 24);
            this.c_TraceProgress.TabIndex = 3;
            // 
            // c_TraceImage
            // 
            this.c_TraceImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.c_TraceImage.Enabled = false;
            this.c_TraceImage.Location = new System.Drawing.Point(15, 136);
            this.c_TraceImage.Name = "c_TraceImage";
            this.c_TraceImage.Size = new System.Drawing.Size(384, 384);
            this.c_TraceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.c_TraceImage.TabIndex = 4;
            this.c_TraceImage.TabStop = false;
            // 
            // c_OnlyComparisonDataCheckBox
            // 
            this.c_OnlyComparisonDataCheckBox.AutoSize = true;
            this.c_OnlyComparisonDataCheckBox.Location = new System.Drawing.Point(15, 47);
            this.c_OnlyComparisonDataCheckBox.Name = "c_OnlyComparisonDataCheckBox";
            this.c_OnlyComparisonDataCheckBox.Size = new System.Drawing.Size(171, 17);
            this.c_OnlyComparisonDataCheckBox.TabIndex = 5;
            this.c_OnlyComparisonDataCheckBox.Text = "Only comparison data (full tiles)";
            this.c_OnlyComparisonDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // c_GenerateGIF
            // 
            this.c_GenerateGIF.AutoSize = true;
            this.c_GenerateGIF.Location = new System.Drawing.Point(192, 47);
            this.c_GenerateGIF.Name = "c_GenerateGIF";
            this.c_GenerateGIF.Size = new System.Drawing.Size(90, 17);
            this.c_GenerateGIF.TabIndex = 6;
            this.c_GenerateGIF.Text = "Generate GIF";
            this.c_GenerateGIF.UseVisualStyleBackColor = true;
            // 
            // c_FormZoomSize
            // 
            this.c_FormZoomSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.c_FormZoomSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_FormZoomSize.FormattingEnabled = true;
            this.c_FormZoomSize.Location = new System.Drawing.Point(316, 45);
            this.c_FormZoomSize.Name = "c_FormZoomSize";
            this.c_FormZoomSize.Size = new System.Drawing.Size(83, 21);
            this.c_FormZoomSize.TabIndex = 7;
            this.c_FormZoomSize.SelectedIndexChanged += new System.EventHandler(this.c_FormZoomSize_SelectedIndexChanged);
            // 
            // TraceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 532);
            this.Controls.Add(this.c_FormZoomSize);
            this.Controls.Add(this.c_GenerateGIF);
            this.Controls.Add(this.c_OnlyComparisonDataCheckBox);
            this.Controls.Add(this.c_TraceImage);
            this.Controls.Add(this.c_TraceProgress);
            this.Controls.Add(this.c_PerformTrace);
            this.Controls.Add(this.c_TraceScrollbar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TraceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trace Algorithm";
            ((System.ComponentModel.ISupportInitialize)(this.c_TraceImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HScrollBar c_TraceScrollbar;
        private System.Windows.Forms.Button c_PerformTrace;
        private System.Windows.Forms.ProgressBar c_TraceProgress;
        private System.Windows.Forms.PictureBox c_TraceImage;
        private System.Windows.Forms.CheckBox c_OnlyComparisonDataCheckBox;
        private System.Windows.Forms.CheckBox c_GenerateGIF;
        private System.Windows.Forms.ComboBox c_FormZoomSize;
    }
}
