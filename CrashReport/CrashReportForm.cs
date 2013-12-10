// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrashReport
{
    public partial class CrashReportForm : Form
    {
        public CrashReportForm(string uri)
        {
            InitializeComponent();
            c_LinkLabel.Text = uri;
        }

        private void c_RestartButton_Click(object sender, EventArgs e)
        {
            // This will close this window, CrashReporter will reopen Tychaia.
            this.Close();
        }

        private void c_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(c_LinkLabel.Text);
            this.Close();
        }
    }
}
