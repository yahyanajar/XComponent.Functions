using System;
using System.Linq;
using Microsoft.Owin.Hosting;
using XComponent.Functions.WebApi;

namespace XComponent.Functions.Core.Owin
{
    internal class OwinServer: IDisposable
    {
        private IDisposable _webServer;
 
        public void InitService(string url)
        {
            if (_webServer == null)
            {
                //add assembly resolver because of an ILRepack issue
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                _webServer = WebApp.Start<Startup>(url);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(e => e.FullName.Contains("XComponent.Functions.Core"));
            return assembly;
        }

        public void Dispose()
        {
            _webServer?.Dispose();
        }
    }
}
