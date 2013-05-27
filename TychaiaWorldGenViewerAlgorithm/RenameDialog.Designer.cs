namespace TychaiaWorldGenViewerAlgorithm
{
    partial class RenameDialog
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.c_CancelButton = new System.Windows.Forms.Button();
            this.c_OKButton = new System.Windows.Forms.Button();
            this.c_NameTextBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter a new name for this flow layer:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.c_CancelButton);
            this.flowLayoutPanel1.Controls.Add(this.c_OKButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(106, 52);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(180, 25);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // c_CancelButton
            // 
            this.c_CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.c_CancelButton.Location = new System.Drawing.Point(127, 3);
            this.c_CancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.c_CancelButton.Name = "c_CancelButton";
            this.c_CancelButton.Size = new System.Drawing.Size(53, 23);
            this.c_CancelButton.TabIndex = 1;
            this.c_CancelButton.Text = "Cancel";
            this.c_CancelButton.UseVisualStyleBackColor = true;
            this.c_CancelButton.Click += new System.EventHandler(this.c_CancelButton_Click);
            // 
            // c_OKButton
            // 
            this.c_OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.c_OKButton.Location = new System.Drawing.Point(68, 3);
            this.c_OKButton.Name = "c_OKButton";
            this.c_OKButton.Size = new System.Drawing.Size(53, 23);
            this.c_OKButton.TabIndex = 0;
            this.c_OKButton.Text = "OK";
            this.c_OKButton.UseVisualStyleBackColor = true;
            this.c_OKButton.Click += new System.EventHandler(this.c_OKButton_Click);
            // 
            // c_NameTextBox
            // 
            this.c_NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.c_NameTextBox.Location = new System.Drawing.Point(15, 26);
            this.c_NameTextBox.Name = "c_NameTextBox";
            this.c_NameTextBox.Size = new System.Drawing.Size(271, 20);
            this.c_NameTextBox.TabIndex = 2;
            // 
            // RenameDialog
            // 
            this.AcceptButton = this.c_OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.c_CancelButton;
            this.ClientSize = new System.Drawing.Size(298, 89);
            this.Controls.Add(this.c_NameTextBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename Flow Action";
            this.Shown += new System.EventHandler(this.RenameDialog_Shown);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button c_CancelButton;
        private System.Windows.Forms.Button c_OKButton;
        private System.Windows.Forms.TextBox c_NameTextBox;
    }
}