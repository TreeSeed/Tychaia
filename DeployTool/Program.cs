// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DeployTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var target = args[0];
            var filter = FileFilterParser.Parse(".pvdeploy", GetRecursiveFilesInCwd());

            using (var file = new FileStream(target, FileMode.Create))
            {
                using (var writer = new ZipOutputStream(file))
                {
                    foreach (var kv in filter)
                    {
                        var fi = new FileInfo(kv.Key);
                        var entry = new ZipEntry(kv.Value);
                        entry.DateTime = fi.LastWriteTime;
                        entry.Size = fi.Length;
                        writer.PutNextEntry(entry);
                        using (var streamReader = File.OpenRead(kv.Key))
                        {
                            streamReader.CopyTo(writer);
                        }

                        writer.CloseEntry();
                    }

                    writer.Close();
                }
            }
        }

        private static IEnumerable<string> GetRecursiveFilesInCwd(string path = null)
        {
            if (path == null)
                path = Environment.CurrentDirectory;
            var current = new DirectoryInfo(path);

            foreach (var di in current.GetDirectories())
                foreach (string s in GetRecursiveFilesInCwd(path + "/" + di.Name))
                    yield return (di.Name + "/" + s).Trim('/');
            foreach (var fi in current.GetFiles())
                yield return fi.Name;
        }
    }
}
