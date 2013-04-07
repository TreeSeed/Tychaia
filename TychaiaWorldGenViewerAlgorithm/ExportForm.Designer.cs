namespace TychaiaWorldGenViewerAlgorithm
{
    partial class ExportForm
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
            this.c_RenderBox = new System.Windows.Forms.PictureBox();
            this.c_Timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.c_RenderBox)).BeginInit();
            this.SuspendLayout();
            // 
            // c_RenderBox
            // 
            this.c_RenderBox.BackColor = System.Drawing.Color.Black;
            this.c_RenderBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c_RenderBox.Location = new System.Drawing.Point(0, 0);
            this.c_RenderBox.Name = "c_RenderBox";
            this.c_RenderBox.Size = new System.Drawing.Size(1008, 986);
            this.c_RenderBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.c_RenderBox.TabIndex = 0;
            this.c_RenderBox.TabStop = false;
            // 
            // c_Timer
            // 
            this.c_Timer.Interval = 10;
            this.c_Timer.Tick += new System.EventHandler(this.c_Timer_Tick);
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 986);
            this.Controls.Add(this.c_RenderBox);
            this.MaximumSize = new System.Drawing.Size(1024, 9000);
            this.Name = "ExportForm";
            this.Text = "Export Layer";
            ((System.ComponentModel.ISupportInitialize)(this.c_RenderBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox c_RenderBox;
        private System.Windows.Forms.Timer c_Timer;
    }
}