// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace Tychaia.Website
{
    public class DependencyControllerFactory : DefaultControllerFactory
    {
        private IKernel m_Kernel;
    
        public DependencyControllerFactory(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }
    
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var constructors = controllerType.GetConstructors();
            if (constructors.Length == 0)
                throw new InvalidOperationException(
                    "Unable to find a public constructor for " + controllerType.FullName);
            var firstConstructor = constructors.First();
            var parameters = firstConstructor.GetParameters();
            var arguments = new object[parameters.Length];

            // Use IoC to resolve each of the arguments.
            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = this.m_Kernel.Get(parameters[i].ParameterType);

            var controller = Activator.CreateInstance(controllerType, arguments) as Controller;
            return controller;
        }
    }
}
