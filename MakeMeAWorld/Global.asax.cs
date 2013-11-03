// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Web;
using Ninject;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace MakeMeAWorld
{
    public class Global : HttpApplication
    {
        public static IKernel Kernel { get; private set; }
    
        public void Inject(object t)
        {
            if (Kernel != null)
                Kernel.Inject(t);
            else
            {
                Application_Start(this, new EventArgs());
                Kernel.Inject(t);
            }
        }
    
        protected virtual void Application_Start(Object sender, EventArgs e)
        {
            Kernel = new StandardKernel();
            Kernel.Load<TychaiaProceduralGenerationIoCModule>();
            Kernel.Load<TychaiaGlobalIoCModule>();
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

