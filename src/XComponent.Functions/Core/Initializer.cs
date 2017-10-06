using System;
using System.Linq;
using Microsoft.Owin.Hosting;
using XComponent.Functions.WebApi;

namespace XComponent.Functions.Core
{
    internal class Initializer: IDisposable
    {
        private IDisposable _webServer;

        public void InitService(FunctionsProtocol protocol, string host = "127.0.0.1", int port = 9756)
        {
            if (_webServer == null)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                _webServer = WebApp.Start<Startup>(protocol.ToString() + "://" + host + ":" + port);
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
