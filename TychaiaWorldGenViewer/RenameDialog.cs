using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TychaiaWorldGenViewer
{
    public partial class RenameDialog : Form
    {
        public new string Name
        {
            get
            {
                return this.c_NameTextBox.Text;
            }
            set
            {
                this.c_NameTextBox.Text = value;
            }
        }

        public RenameDialog(string initial)
        {
            InitializeComponent();
            this.c_NameTextBox.Text = initial;
        }

        private void c_OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void c_CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void RenameDialog_Shown(object sender, EventArgs e)
        {
            this.c_NameTextBox.Focus();
            this.c_NameTextBox.SelectAll();
        }
    }
}
