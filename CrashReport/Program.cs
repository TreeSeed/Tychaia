// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CrashReport
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Definines the Path
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tychaia.lock");

            if (File.Exists(path))
                DeleteLock();

            // Create tychaia.lock file
            using (var Writer = new StreamWriter(path))
            {
                Writer.WriteLine("Lock");
            }

            CollectSystemInfo.GetSystemInfo();
            // Test
            //try
            //{
            //    throw new NotImplementedException();
            //}
            //catch(Exception e)
            //{
            //    CrashReporter.Record(e);
            //}
            
            // Make sure it is deleted after test
            DeleteLock();
        }

        public static void DeleteLock()
        {
            // Definines the Path
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tychaia.lock");

            // Delete tychaia.lock file
            File.Delete(path);
        }
    }
}
