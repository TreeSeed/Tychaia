using System;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.Routing;
using System.Linq;
using Tychaia.Globals;
using Ninject;

namespace Tychaia.Website
{
    public class DependencyControllerFactory : DefaultControllerFactory
    {
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
                arguments[i] = IoC.Kernel.Get(parameters[i].ParameterType);

            var controller = Activator.CreateInstance(controllerType, arguments) as Controller;
            return controller;
        }
    }
}

