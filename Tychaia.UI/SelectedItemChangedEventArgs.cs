//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.UI
{
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public TreeItem Item { get; set; }

        public SelectedItemChangedEventArgs(TreeItem item)
        {
            this.Item = item;
        }
    }
}

