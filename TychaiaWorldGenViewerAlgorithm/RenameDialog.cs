// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Windows.Forms;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class RenameDialog : Form
    {
        public RenameDialog(string initial)
        {
            this.InitializeComponent();
            this.c_NameTextBox.Text = initial;
        }

        public new string Name
        {
            get { return this.c_NameTextBox.Text; }
            set { this.c_NameTextBox.Text = value; }
        }

        private void c_OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void c_CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RenameDialog_Shown(object sender, EventArgs e)
        {
            this.c_NameTextBox.Focus();
            this.c_NameTextBox.SelectAll();
        }
    }
}
