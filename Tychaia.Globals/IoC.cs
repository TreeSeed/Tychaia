using Ninject;
using System;
using Ninject.Modules;
using System.Linq;
using System.Collections.Generic;

namespace Tychaia.Globals
{
    public static class IoC
    {
        private static IKernel m_Kernel;

        private static IEnumerable<INinjectModule> GetModules()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   where !assembly.IsDynamic
                   where assembly.FullName.StartsWith("Tychaia.", StringComparison.Ordinal)
                   from type in assembly.GetTypes()
                   where typeof(INinjectModule).IsAssignableFrom(type)
                   where !type.IsAbstract
                   where type.GetConstructor(Type.EmptyTypes) != null
                   select Activator.CreateInstance(type) as INinjectModule;
        }

        /// <summary>
        /// Replaces the IoC kernel.  Should not be used outside unit tests.
        /// </summary>
        public static void ReplaceKernel(IKernel kernel)
        {
            if (m_Kernel == kernel)
                return;
            m_Kernel = kernel;
            m_Kernel.Load(GetModules());
        }

        public static IKernel Kernel
        {
            get
            {
                if (m_Kernel == null)
                {
                    var modules = GetModules().ToArray();
                    foreach (var module in modules)
                    {
                        Console.WriteLine("IoC loaded module " + module.GetType().FullName);
                    }
                    m_Kernel = new StandardKernel(modules);
                }
                return m_Kernel;
            }
        }
    }
}

