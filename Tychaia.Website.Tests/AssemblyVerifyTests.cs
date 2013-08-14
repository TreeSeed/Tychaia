// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Xunit;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Tychaia.Website.Tests
{
    public class AssemblyVerifyTests
    {
        [Fact]
        public void EnsureConsistentAssemblyVersions()
        {
            var basePath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "../../..")).FullName;
            var libPath = new DirectoryInfo(Path.Combine(basePath, "Libraries/Microsoft")).FullName;
            var allAssemblies = RecursiveDirectorySearch(basePath, "*.dll").ToList();
            var assemblyVersions = new Dictionary<string, string>();
            foreach (var dll in new DirectoryInfo(libPath).GetFiles("*.dll"))
            {
                foreach (var entry in allAssemblies)
                {
                    if (new FileInfo(entry).Name == dll.Name)
                    {
                        // Matches filename, check version.
                        var version = GetAssemblyVersion(dll.FullName);
                        if (!assemblyVersions.ContainsKey(dll.Name))
                        {
                            assemblyVersions[dll.Name] = version;
                        }
                        else 
                        {
                            Assert.True(
                                assemblyVersions[dll.Name] == version,
                                entry.Substring(basePath.Length + 1) +
                                " is version " +
                                version +
                                ", but needs to be " +
                                assemblyVersions[dll.Name]
                            );
                        }
                    }
                }
            }
        }

        private static string GetAssemblyVersion(string path)
        {
            var value = string.Empty;
            var attributeFileVersionType = typeof(AssemblyFileVersionAttribute);
            var attributeVersionType = typeof(AssemblyFileVersionAttribute);
            var assembly = AssemblyDefinition.ReadAssembly(path);
            return assembly.FullName;
        }

        private static IEnumerable<string> RecursiveDirectorySearch(string root, string pattern)
        {
            var dir = new DirectoryInfo(root);
            foreach (var d in dir.GetDirectories())
            {
                foreach (var x in RecursiveDirectorySearch(d.FullName, pattern))
                    yield return x;
            }
            foreach (var f in dir.GetFiles(pattern))
            {
                yield return f.FullName;
            }
        }
    }
}

