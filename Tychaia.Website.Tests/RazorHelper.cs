using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc.Razor;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;
using Moq;
using System.Web.Routing;

public class RazorViewExecutionResult
{
    public string Text { get; set; }
    public IList<string> SectionNames { get; private set; }

    public RazorViewExecutionResult()
    {
        SectionNames = new List<string>();
    }
}

public static class RazorHelper<T>
{
    public static RazorViewExecutionResult GenerateAndExecuteTemplate(string templateName, T model)
    {
        return GenerateAndExecuteTemplate(templateName, model, null, null);
    }

    public static RazorViewExecutionResult GenerateAndExecuteTemplate
        (string templateName, T model, HttpContextBase httpContext)
    {
        return GenerateAndExecuteTemplate(templateName, model, httpContext, null);
    }

    public static RazorViewExecutionResult GenerateAndExecuteTemplate
        (string templateName, T model, HttpContextBase httpContext, Action<WebViewPage<T>> modifyViewBag)
    {
        var view = File.ReadAllText(templateName);
        var template = RazorHelper<T>.GenerateTemplate(view);
        if (modifyViewBag != null)
            modifyViewBag(template);
        var result = RazorHelper<T>.ExecuteTemplate(template, model, httpContext);
        Console.WriteLine(result.Text);
        return result;
    }

    private static RazorTemplateEngine SetupRazorEngine()
    {
        // Set up the hosting environment (filename here is only used to trick RazorTemplateEngine)
        var host = new MvcWebPageRazorHost("~/test.cshtml", System.Environment.CurrentDirectory);

        host.NamespaceImports.Add("System.Web.Mvc.Html");
        host.NamespaceImports.Add("Tychaia.Website");
        // TODO: add your namespaces here

        // Create the template engine using this host
        return new RazorTemplateEngine(host);
    }

    public static RazorViewExecutionResult ExecuteTemplate(WebViewPage<T> viewPage, T model)
    {
        return ExecuteTemplate(viewPage, model, null);
    }

    public static RazorViewExecutionResult ExecuteTemplate(WebViewPage<T> viewPage, T model, HttpContextBase httpContext)
    {
        var result = new RazorViewExecutionResult();

        // Warning: lots of mocking below

        // mock HTTP state objects
        var context = new Mock<HttpContextBase>();
        context.Setup(x => x.Items).Returns(new Dictionary<object, object>());
        var response = new Mock<HttpResponseBase>();
        response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>()))
            .Returns(new Func<string, string>(x => x));
        context.Setup(x => x.Response).Returns(response.Object);
        var request = new Mock<HttpRequestBase>();
        context.Setup(x => x.Request).Returns(request.Object);
        request.Setup(x => x.ApplicationPath).Returns("/");
        request.Setup(x => x.IsLocal).Returns(true);
        var requestRouteContext = new RequestContext(context.Object, new RouteData());
        // mock page view context
        var view = new Mock<ViewContext>();
        view.Setup(x => x.HttpContext).Returns(context.Object);
        var viewMock = new Mock<IView>();
        view.Setup(x => x.View).Returns(viewMock.Object);
        view.Setup(x => x.TempData).Returns(new TempDataDictionary());
        view.Setup(x => x.ViewData).Returns(new ViewDataDictionary<T>(model));
        var viewDataContainer = new Mock<IViewDataContainer>();
        // mock view data used by the page
        viewDataContainer.Setup(c => c.ViewData).Returns(new ViewDataDictionary<T>(model));
        // mock html helper
        var html = new Mock<HtmlHelper<T>>(view.Object, viewDataContainer.Object);
        var urlHelper = new Mock<UrlHelper>(requestRouteContext);
        // mock viewengine (for partial views fake resolution)
        var viewEngine = new Mock<IViewEngine>();
        viewEngine.Setup(x => x.FindPartialView(
            It.IsAny<ControllerContext>(),
            It.IsAny<string>(),
            It.IsAny<bool>()))
            .Returns(new ViewEngineResult(viewMock.Object, viewEngine.Object));
        using (var tw = new StringWriter())
        using (var twNull = new StringWriter()) // this writer is used to discard unnecessary results
        {
            view.Setup(x => x.Writer).Returns(tw);

            // inject mocked context
            viewPage.Context = httpContext;

            var pageContext = new WebPageContext(context: context.Object, page: null, model: null);
            viewPage.ViewContext = view.Object;
            if (model != null)
                viewPage.ViewData = new ViewDataDictionary<T>(model);

            viewPage.Html = html.Object;
            viewPage.Url = urlHelper.Object;

            // insert mocked viewEngine for fake partial views resolution
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngine.Object);

            // prepare view for execution, discard all generated results
            viewPage.PushContext(pageContext, twNull);
            viewPage.ExecutePageHierarchy(pageContext, twNull);
            // inject textwriter
            viewPage.OutputStack.Push(tw);
            // execute compiled page
            viewPage.Execute();

            // find all sections and render them too
            var dynMethod = pageContext.GetType().GetProperty("SectionWritersStack", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (Stack<Dictionary<string, SectionWriter>>)dynMethod.GetValue(pageContext, null);
            foreach (var section in res.Peek())
            {
                section.Value();
                result.SectionNames.Add(section.Key);
            }

            result.Text = tw.ToString();
            return result;
        }
    }

    public static WebViewPage<T> GenerateTemplate(string input)
    {
        WebViewPage<T> result = null;
        var _engine = SetupRazorEngine();

        // Generate code for the template
        GeneratorResults razorResult = null;
        var layout = new Regex("Layout = .*");
        input = layout.Replace(input, string.Empty); // layouts cannot be rendered in unit test environment
        using (var rdr = new StringReader(input))
        {
            razorResult = _engine.GenerateCode(rdr);
        }

        var codeProvider = new CSharpCodeProvider();

        // generate C# code
        using (var sw = new StringWriter())
        {
            codeProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
        }

        var assemblyName = typeof(RazorHelper<>).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\");
        var windows = true;
        if (assemblyName[1] != ':' || assemblyName[2] != '\\')
        {
            assemblyName = typeof(RazorHelper<>).Assembly.CodeBase.Replace("file://", "");
            windows = false;
        }
        var compParams = new CompilerParameters(new string[] { assemblyName });
        compParams.GenerateInMemory = true;
        compParams.ReferencedAssemblies.Add("System.dll");
        compParams.ReferencedAssemblies.Add("System.Core.dll");
        compParams.ReferencedAssemblies.Add("System.Net.dll");
        compParams.ReferencedAssemblies.Add("System.Web.dll");
        if (windows)
        {
            compParams.ReferencedAssemblies.Add(@"..\..\..\Tychaia.Website\bin\System.Web.WebPages.dll");
            compParams.ReferencedAssemblies.Add(@"..\..\..\Tychaia.Website\bin\System.Web.Mvc.dll");
        }
        else
        {
            compParams.ReferencedAssemblies.Add("System.Web.WebPages.dll");
            compParams.ReferencedAssemblies.Add("System.Web.Mvc.dll");
        }
        compParams.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
        compParams.ReferencedAssemblies.Add("Tychaia.Website.dll");
        compParams.ReferencedAssemblies.Add("Argotic.Core.dll");
        compParams.ReferencedAssemblies.Add("Argotic.Common.dll");
        compParams.ReferencedAssemblies.Add("Argotic.Extensions.dll");
        compParams.IncludeDebugInformation = true;

        // Compile the generated code into an assembly
        var results = codeProvider.CompileAssemblyFromDom(
            compParams,
            razorResult.GeneratedCode);

        if (results.Errors.HasErrors)
        {
            var err = results.Errors.OfType<CompilerError>().First(ce => !ce.IsWarning);
            throw new HttpCompileException(String.Format("Error Compiling Template: ({0}, {1}) {2}",
                                          err.Line, err.Column, err.ErrorText));
        }

        // Load the assembly
        var asm = results.CompiledAssembly;
        if (asm == null)
        {
            throw new HttpCompileException("Error loading template assembly");
        }

        // Get the template type
        var typ = asm.GetType("ASP._Page_test_cshtml"); // remember the fake filename?
        if (typ == null)
        {
            throw new HttpCompileException(string.Format(
                "Could not find type ASP._Page_test_cshtml " +
                "in assembly {0}", asm.FullName));
        }

        result = Activator.CreateInstance(typ) as WebViewPage<T>;
        if (result == null)
        {
            throw new HttpCompileException(
                "Could not construct RazorOutput.Template or " +
                "it does not inherit from ASP._Page_test_cshtml.  " +
                "Check the type parameter T of RazorHelper<T> " +
                "matches the type of the model used by the view.");
        }

        return result;
    }
}
