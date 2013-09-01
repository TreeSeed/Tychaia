// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;

namespace TychaiaTool
{
    public class DefaultConfigurationHelper : IConfigurationHelper
    {
        private readonly IProceduralConfiguration[] m_Configurations;

        public DefaultConfigurationHelper(
            IProceduralConfiguration[] configurations)
        {
            this.m_Configurations = configurations;
        }

        public void Setup(ManyConsole.ConsoleCommand command, Action<string> assign)
        {
            var list = "";
            foreach (var config in this.m_Configurations)
            {
                var name = config.GetType().Name.ToLower();
                name = name.Substring(0, name.Length - "ProceduralConfiguration".Length);
                list += "\n * " + name;
            }
            command.HasOption(
                "config=",
                "The procedural configuration to use: " + list,
                assign);
        }

        public IProceduralConfiguration Validate(string assigned)
        {
            if (assigned == null)
                return this.m_Configurations.First();

            foreach (var config in this.m_Configurations)
            {
                var name = config.GetType().Name.ToLower();
                name = name.Substring(0, name.Length - "ProceduralConfiguration".Length);
                if (assigned == name)
                    return config;
            }

            Console.WriteLine("Invalid configuration profile.");
            return null;
        }
    }
}

