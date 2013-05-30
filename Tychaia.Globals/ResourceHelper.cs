using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tychaia.Globals
{
    public static class ResourceHelper
    {
        public static Image GetImageResource(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var stream = assembly.GetManifestResourceStream(name);
                if (stream != null)
                    return new Bitmap(stream);
            }
            return null;
        }
    }
}
