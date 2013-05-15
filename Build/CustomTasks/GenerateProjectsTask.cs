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
            
            var generator = new ProjectGenerator(this.RootPath, this.Platform);
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

        public ProjectGenerator(string rootPath, string platform)
        {
            this.m_RootPath = rootPath;
            this.m_Platform = platform;
        }

        public void Load(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
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
            
            // Work out what path to save at.
            var projectDoc = this.m_ProjectDocuments.First(
              x => x.DocumentElement.Attributes["Name"].Value == project);
            var path = Path.Combine(
                this.m_RootPath,
                projectDoc.DocumentElement.Attributes["Path"].Value,
                projectDoc.DocumentElement.Attributes["Name"].Value + "." +
                this.m_Platform + ".csproj");
            path = new FileInfo(path).FullName;
            var input = this.CreateInputFor(project, this.m_Platform);
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

        private XmlDocument CreateInputFor(string project, string platform)
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
            generation.AppendChild(projectName);
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
            using(var xmlReader = xDocument.CreateReader())
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

