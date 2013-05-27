using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Tychaia.CustomTasks
{
    public class GenerateProjectsTask : Task
    {
        [Required]
        public string SourcePath
        {
            get;
            set;
        }

        [Required]
        public string RootPath
        {
            get;
            set;
        }

        [Required]
        public string Platform
        {
            get;
            set;
        }

        [Required]
        public ITaskItem[] Definitions
        {
            get;
            set;
        }

        public override bool Execute()
        {
            this.Log.LogMessage(
                "Starting generation of projects for " + this.Platform);

            var generator = new ProjectGenerator(
                this.RootPath,
                this.Platform,
                this.Log);
            foreach (var definition in this.Definitions)
            {
                this.Log.LogMessage("Loading: " + definition);
                generator.Load(Path.Combine(
                    this.SourcePath,
                    definition + ".definition"));
            }
            foreach (var definition in this.Definitions)
            {
                this.Log.LogMessage("Generating: " + definition);
                generator.Generate(definition.ToString());
            }

            var solution = Path.Combine(
                this.RootPath,
                "Tychaia." + this.Platform + ".sln");
            this.Log.LogMessage("Generating: (solution)");
            generator.GenerateSolution(solution);

            this.Log.LogMessage(
                "Generation complete.");

            return true;
        }
    }

    public class CleanProjectsTask : GenerateProjectsTask
    {
        public override bool Execute()
        {
            this.Log.LogMessage(
                "Starting clean of projects for " + this.Platform);

            foreach (var definition in this.Definitions)
            {
                this.Log.LogMessage("Cleaning: " + definition);
                var projectDoc = new XmlDocument();
                projectDoc.Load(Path.Combine(
                    this.SourcePath,
                    definition + ".definition"));
                if (projectDoc == null ||
                    projectDoc.DocumentElement.Name != "Project")
                    continue;
                var path = Path.Combine(
                    this.RootPath,
                    projectDoc.DocumentElement.Attributes["Path"].Value,
                    projectDoc.DocumentElement.Attributes["Name"].Value + "." +
                    this.Platform + ".csproj");
                if (File.Exists(path))
                    File.Delete(path);
            }

            this.Log.LogMessage(
                "Clean complete.");

            return true;
        }
    }

    public class ProjectGenerator
    {
        private string m_RootPath;
        private string m_Platform;
        private List<XmlDocument> m_ProjectDocuments = new List<XmlDocument>();
        private XslCompiledTransform m_ProjectTransform = null;
        private XslCompiledTransform m_SolutionTransform = null;
        private TaskLoggingHelper m_Log;

        public ProjectGenerator(
            string rootPath,
            string platform,
            TaskLoggingHelper log)
        {
            this.m_RootPath = rootPath;
            this.m_Platform = platform;
            this.m_Log = log;
        }

        public void Load(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);

            // If this is a ContentProject, we actually need to generate the
            // full project node from the files that are in the Source folder.
            if (doc.DocumentElement.Name == "ContentProject")
                this.m_ProjectDocuments.Add(GenerateContentProject(doc));
            else
                this.m_ProjectDocuments.Add(doc);

            // Also add a Guid attribute if one doesn't exist.  This makes it
            // easier to define projects.
            if (doc.DocumentElement.Attributes["Guid"] == null)
            {
                doc.DocumentElement.SetAttribute("Guid",
                Guid.NewGuid().ToString().ToUpper());
            }
        }

        public void Generate(string project)
        {
            if (this.m_ProjectTransform == null)
            {
                var resolver = new EmbeddedResourceResolver();
                this.m_ProjectTransform = new XslCompiledTransform();
                this.m_ProjectTransform.Load(
                    "GenerateProject.xslt",
                    XsltSettings.TrustedXslt,
                    resolver
                );
            }

            // Work out what document this is.
            var projectDoc = this.m_ProjectDocuments.First(
                x => x.DocumentElement.Attributes["Name"].Value == project);

            // Check to see if we have a Project node; if not
            // then this is an external or other type of project
            // that we don't process.
            if (projectDoc == null ||
                projectDoc.DocumentElement.Name != "Project")
                return;

            // Work out what path to save at.
            var path = Path.Combine(
                this.m_RootPath,
                projectDoc.DocumentElement.Attributes["Path"].Value,
                projectDoc.DocumentElement.Attributes["Name"].Value + "." +
                this.m_Platform + ".csproj");
            path = new FileInfo(path).FullName;

            // Work out what path the NuGet packages.config might be at.
            var packagesPath = Path.Combine(
                this.m_RootPath,
                projectDoc.DocumentElement.Attributes["Path"].Value,
                "packages.config");

            // Generate the input document.
            var input = this.CreateInputFor(
                project,
                this.m_Platform,
                packagesPath);

            // Transform the input document using the XSLT transform.
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(path, settings))
            {
                this.m_ProjectTransform.Transform(input, writer);
            }

            // Also remove any left over .sln or .userprefs files.
            var slnPath = Path.Combine(
                this.m_RootPath,
                projectDoc.DocumentElement.Attributes["Path"].Value,
                projectDoc.DocumentElement.Attributes["Name"].Value + "." +
                this.m_Platform + ".sln");
            var userprefsPath = Path.Combine(
                this.m_RootPath,
                projectDoc.DocumentElement.Attributes["Path"].Value,
                projectDoc.DocumentElement.Attributes["Name"].Value + "." +
                this.m_Platform + ".userprefs");
            if (File.Exists(slnPath))
                File.Delete(slnPath);
            if (File.Exists(userprefsPath))
                File.Delete(userprefsPath);
        }

        public void GenerateSolution(string solutionPath)
        {
            if (this.m_SolutionTransform == null)
            {
                var resolver = new EmbeddedResourceResolver();
                this.m_SolutionTransform = new XslCompiledTransform();
                this.m_SolutionTransform.Load(
                    "GenerateSolution.xslt",
                    XsltSettings.TrustedXslt,
                    resolver
                );
            }

            var input = this.CreateInputFor(this.m_Platform);
            using (var writer = new StreamWriter(solutionPath))
            {
                this.m_SolutionTransform.Transform(input, null, writer);
            }
        }

        private XmlDocument CreateInputFor(
            string project,
            string platform,
            string packagesPath)
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            var input = doc.CreateElement("Input");
            doc.AppendChild(input);

            var generation = doc.CreateElement("Generation");
            var projectName = doc.CreateElement("ProjectName");
            projectName.AppendChild(doc.CreateTextNode(project));
            var platformName = doc.CreateElement("Platform");
            platformName.AppendChild(doc.CreateTextNode(platform));
            var rootName = doc.CreateElement("RootPath");
            rootName.AppendChild(doc.CreateTextNode(
                new DirectoryInfo(this.m_RootPath).FullName));
            generation.AppendChild(projectName);
            generation.AppendChild(platformName);
            generation.AppendChild(rootName);
            input.AppendChild(generation);

            var nuget = doc.CreateElement("NuGet");
            input.AppendChild(nuget);

            var projects = doc.CreateElement("Projects");
            input.AppendChild(projects);
            foreach (var projectDoc in this.m_ProjectDocuments)
            {
                projects.AppendChild(doc.ImportNode(
                    projectDoc.DocumentElement,
                    true));
            }

            // Also check if there are NuGet packages.config file for
            // this project and if there is, include all of the relevant
            // NuGet package information for referencing the correct DLLs.
            if (File.Exists(packagesPath))
            {
                this.DetectNuGetPackages(
                    packagesPath,
                    doc,
                    nuget);
            }

            return doc;
        }

        private void DetectNuGetPackages(
            string packagesPath,
            XmlDocument document,
            XmlNode nuget)
        {
            // Read the packages document and generate Project nodes for
            // each package that we want.
            var packagesDoc = new XmlDocument();
            packagesDoc.Load(packagesPath);
            var packages = packagesDoc.DocumentElement
                .ChildNodes
                    .Cast<XmlElement>()
                    .Where(x => x.Name == "package")
                    .Select(x => x);
            foreach (var package in packages)
            {
                var id = package.Attributes["id"].Value;
                var version = package.Attributes["version"].Value;

                var packagePath = Path.Combine(
                    this.m_RootPath,
                    "packages",
                    id + "." + version,
                    id + "." + version + ".nuspec");
                var packageDoc = new XmlDocument();
                packageDoc.Load(packagePath);

                var references = packageDoc.DocumentElement
                    .FirstChild
                    .ChildNodes
                    .Cast<XmlElement>()
                    .First(x => x.Name == "references")
                    .ChildNodes
                    .Cast<XmlElement>()
                    .Where(x => x.Name == "reference")
                    .Select(x => x.Attributes["file"].Value)
                    .ToList();

                var clrNames = new[]
                {
                    "",
                    "net40-client",
                    "Net40-client",
                    "net40",
                    "Net40",
                    "net35",
                    "Net35",
                    "net20",
                    "Net20"
                };
                var referenceBasePath = Path.Combine(
                    "packages",
                    id + "." + version,
                    "lib");
                if (!Directory.Exists(
                    Path.Combine(
                    this.m_RootPath,
                    referenceBasePath)))
                    continue;
                foreach (var reference in references)
                {
                    foreach (var clrName in clrNames)
                    {
                        var packageDll = Path.Combine(
                            referenceBasePath,
                            clrName,
                            reference);
                        if (File.Exists(
                            Path.Combine(
                            this.m_RootPath,
                            packageDll)))
                        {
                            var packageReference =
                                document.CreateElement("Package");
                            packageReference.SetAttribute(
                                "Name",
                                id);
                            packageReference.AppendChild(
                                document.CreateTextNode(packageDll
                                .Replace('/', '\\')));
                            nuget.AppendChild(packageReference);
                            break;
                        }
                    }
                }
            }
        }

        private XmlDocument GenerateContentProject(XmlDocument source)
        {
            var sourceFolder = source
                .DocumentElement
                .ChildNodes
                .Cast<XmlElement>()
                .First(x => x.Name == "Source")
                .GetAttribute("Include");
            var originalSourceFolder = sourceFolder;
            sourceFolder = Path.Combine(this.m_RootPath, sourceFolder);

            var allFiles = this.GetListOfFilesInDirectory(sourceFolder);
            this.m_Log.LogMessage(
              "Scanning: " +
              originalSourceFolder +
              " (" + allFiles.Count + " total XNB files)"
              );

            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            var projectNode = doc.CreateElement("ContentProject");
            doc.AppendChild(projectNode);
            projectNode.SetAttribute(
                "Name",
                source.DocumentElement.GetAttribute("Name"));

            foreach (var file in allFiles)
            {
                var fileNode = doc.CreateElement("Compiled");
                var fullPathNode = doc.CreateElement("FullPath");
                var relativePathNode = doc.CreateElement("RelativePath");
                fullPathNode.AppendChild(doc.CreateTextNode(file));
                var index = file.Replace("\\", "/")
                    .LastIndexOf(originalSourceFolder.Replace("\\", "/"));
                var relativePath = "Content\\" + file
                    .Substring(index + originalSourceFolder.Length)
                    .Replace("/", "\\")
                    .Trim('\\');
                relativePathNode.AppendChild(doc.CreateTextNode(relativePath));
                fileNode.AppendChild(fullPathNode);
                fileNode.AppendChild(relativePathNode);
                projectNode.AppendChild(fileNode);
            }

            return doc;
        }

        private List<string> GetListOfFilesInDirectory(string folder)
        {
            var result = new List<string>();
            var directoryInfo = new DirectoryInfo(folder);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                result.AddRange(
                    this.GetListOfFilesInDirectory(directory.FullName));
            }
            foreach (var file in directoryInfo.GetFiles("*.xnb"))
            {
                result.Add(file.FullName);
            }
            return result;
        }

        private XmlDocument CreateInputFor(string platform)
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            var input = doc.CreateElement("Input");
            doc.AppendChild(input);

            var generation = doc.CreateElement("Generation");
            var platformName = doc.CreateElement("Platform");
            platformName.AppendChild(doc.CreateTextNode(platform));
            generation.AppendChild(platformName);
            input.AppendChild(generation);

            var projects = doc.CreateElement("Projects");
            input.AppendChild(projects);
            foreach (var projectDoc in this.m_ProjectDocuments)
            {
                projects.AppendChild(doc.ImportNode(
                    projectDoc.DocumentElement,
                    true));
            }
            return doc;
        }
    }

    public class EmbeddedResourceResolver : XmlUrlResolver
    {
        public override object GetEntity(
            Uri absoluteUri,
            string role,
            Type ofObjectToReturn)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(
                this.GetType(),
                Path.GetFileName(absoluteUri.AbsolutePath));
        }
    }

    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}

