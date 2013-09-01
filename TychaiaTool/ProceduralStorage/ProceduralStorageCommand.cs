// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using ManyConsole;
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class ProceduralStorageCommand : ConsoleCommand
    {
        private readonly IConfigurationHelper m_ConfigurationHelper;
        private readonly IStorageAccess m_StorageAccess;

        private IProceduralConfiguration m_Configuration;
        private string m_ConfigurationName;

        public ProceduralStorageCommand(
            IConfigurationHelper configurationHelper,
            IStorageAccess storageAccess)
        {
            this.m_ConfigurationHelper = configurationHelper;
            this.m_StorageAccess = storageAccess;

            this.IsCommand("test-storage", "Test the world configuration storage");
            this.m_ConfigurationHelper.Setup(this, x => this.m_ConfigurationName = x);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            this.m_Configuration = this.m_ConfigurationHelper.Validate(this.m_ConfigurationName);
            return this.m_Configuration == null
                ? 1
                : base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }

        public override int Run(string[] remainingArguments)
        {
            var runtime = this.m_Configuration.GetConfiguration() as RuntimeLayer;
            if (runtime == null)
            {
                Console.WriteLine("Unable to store compiled configuration.");
                return 1;
            }

            StorageLayer[] storage = null;
            Console.WriteLine("Storing...");
            using (var writer = new StreamWriter("TestWorldConfig.xml", false))
                this.m_StorageAccess.SaveStorage(
                    new StorageLayer[] { this.m_StorageAccess.FromRuntime(runtime) }, writer);

            Console.WriteLine("Loading...");
            using (var reader = new StreamReader("TestWorldConfig.xml"))
                storage = this.m_StorageAccess.LoadStorage(reader);
            foreach (var l in storage)
            {
                Console.WriteLine(l.Algorithm.GetType().FullName);
            }

            return 0;
        }
    }
}

