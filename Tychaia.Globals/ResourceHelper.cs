// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Linq;

namespace Tychaia.Globals
{
    public static class ResourceHelper
    {
        public static Image GetImageResource(string name)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                select assembly.GetManifestResourceStream(name)
                into stream
                where stream != null
                select new Bitmap(stream)).FirstOrDefault();
        }
    }
}
