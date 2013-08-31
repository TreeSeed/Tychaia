// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Web;
using Ninject;
using Tychaia.ProceduralGeneration;

namespace MakeMeAWorld
{
    public class Global : HttpApplication
    {
        public IKernel Kernel { get; private set; }
    
        protected virtual void Application_Start(Object sender, EventArgs e)
        {
            this.Kernel = new StandardKernel();
            this.Kernel.Load<TychaiaProceduralGenerationIoCModule>();
        }
		
        protected virtual void Session_Start(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Application_BeginRequest(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Application_EndRequest(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Application_Error(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Session_End(Object sender, EventArgs e)
        {
        }
		
        protected virtual void Application_End(Object sender, EventArgs e)
        {
        }
    }
}

