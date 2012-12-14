using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration
{
    public class Descriptions
    {
        // Basically a class that will have random generations for anything that doesn't quite fit. 
        private static Random r = new Random();

        public string[] ResistPrefix
        {
            get { return new string[] { "Resist", "Protection" }; }
        }
    }
}
