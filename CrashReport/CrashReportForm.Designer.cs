// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace CrashReport
{
    partial class CrashReportForm
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
            this.c_RestartButton = new System.Windows.Forms.Button();
            this.c_ErrorLabel = new System.Windows.Forms.Label();
            this.c_RestartLabel = new System.Windows.Forms.Label();
            this.c_LinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // c_RestartButton
            // 
            this.c_RestartButton.Location = new System.Drawing.Point(344, 65);
            this.c_RestartButton.Name = "c_RestartButton";
            this.c_RestartButton.Size = new System.Drawing.Size(75, 23);
            this.c_RestartButton.TabIndex = 0;
            this.c_RestartButton.Text = "Close";
            this.c_RestartButton.UseVisualStyleBackColor = true;
            this.c_RestartButton.Click += new System.EventHandler(this.c_RestartButton_Click);
            // 
            // c_ErrorLabel
            // 
            this.c_ErrorLabel.AutoSize = true;
            this.c_ErrorLabel.Location = new System.Drawing.Point(12, 14);
            this.c_ErrorLabel.Name = "c_ErrorLabel";
            this.c_ErrorLabel.Size = new System.Drawing.Size(407, 13);
            this.c_ErrorLabel.TabIndex = 2;
            this.c_ErrorLabel.Text = "We\'ve automatically created a bug report, you can view it by following the link b" +
    "elow.";
            // 
            // c_RestartLabel
            // 
            this.c_RestartLabel.AutoSize = true;
            this.c_RestartLabel.Location = new System.Drawing.Point(12, 70);
            this.c_RestartLabel.Name = "c_RestartLabel";
            this.c_RestartLabel.Size = new System.Drawing.Size(199, 13);
            this.c_RestartLabel.TabIndex = 3;
            this.c_RestartLabel.Text = "Press the button below to close Tychaia.";
            // 
            // c_LinkLabel
            // 
            this.c_LinkLabel.AutoSize = true;
            this.c_LinkLabel.Location = new System.Drawing.Point(12, 42);
            this.c_LinkLabel.Name = "c_LinkLabel";
            this.c_LinkLabel.Size = new System.Drawing.Size(89, 13);
            this.c_LinkLabel.TabIndex = 4;
            this.c_LinkLabel.TabStop = true;
            this.c_LinkLabel.Text = "An error occured.";
            this.c_LinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.c_LinkLabel_LinkClicked);
            // 
            // CrashReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 105);
            this.Controls.Add(this.c_LinkLabel);
            this.Controls.Add(this.c_RestartLabel);
            this.Controls.Add(this.c_ErrorLabel);
            this.Controls.Add(this.c_RestartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tychaia has encountered an error.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_RestartButton;
        private System.Windows.Forms.Label c_ErrorLabel;
        private System.Windows.Forms.Label c_RestartLabel;
        private System.Windows.Forms.LinkLabel c_LinkLabel;
    }
}