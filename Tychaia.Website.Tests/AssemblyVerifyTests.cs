using System;
using Xunit;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var assemblyVersions = new Dictionary<string, Version>();
            foreach (var dll in new DirectoryInfo(libPath).GetFiles("*.dll"))
            {
                foreach (var entry in allAssemblies)
                {
                    if (new FileInfo(entry).Name == dll.Name)
                    {
                        // Matches filename, check version.
                        var assembly = Assembly.ReflectionOnlyLoadFrom(entry);
                        if (!assemblyVersions.ContainsKey(dll.Name))
                        {
                            assemblyVersions[dll.Name] = assembly.GetName().Version;
                        }
                        else
                        {
                            Assert.True(
                                assemblyVersions[dll.Name].ToString() == assembly.GetName().Version.ToString(),
                                entry.Substring(basePath.Length + 1) +
                                " is version " +
                                assembly.GetName().Version +
                                ", but needs to be " +
                                assemblyVersions[dll.Name]
                            );
                        }
                    }
                }
            }
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

