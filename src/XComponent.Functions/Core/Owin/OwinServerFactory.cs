using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Owin.Hosting;
using XComponent.Functions.Core.Exceptions;

namespace XComponent.Functions.Core.Owin
{
    internal static class OwinServerFactory
    {
        class OwinServerCounter
        {
            private int _refCounter;

            public OwinServer OwinServer { get; set; }

            public void AddRef()
            {
                Interlocked.Increment(ref _refCounter);
            }

            public int Release()
            {
                return Interlocked.Decrement(ref _refCounter);
            }

            public int Port { get; set; }
        }

        private static readonly List<OwinServerCounter> OwinServers = new List<OwinServerCounter>();

        public static OwinServer CreateOwinServer(Uri url)
        {
            lock (OwinServers)
            {
                if (OwinServers.All(e => e.Port != url.Port))
                {
                    OwinServer owinServer = new OwinServer();
                    try
                    {
                        owinServer.InitService(url.ToString());
                        OwinServers.Add(new OwinServerCounter() { OwinServer = owinServer, Port = url.Port});
                    }
                    catch (Exception e)
                    {
                        throw new FunctionsFactoryException("Owin server initialization failed", e);
                    }
                }

                var owinserverCounter = OwinServers.First(e=> e.Port == url.Port);
                owinserverCounter.AddRef();
                return owinserverCounter.OwinServer;
            }
        }

        public static void UnRegisterOwinServer(OwinServer owinServer)
        {
            lock (OwinServers)
            {
                var owinserverCounter = OwinServers.FirstOrDefault(e => e.OwinServer == owinServer);
                if (owinserverCounter != null)
                {
                    var count = owinserverCounter.Release();
                    if (count == 0)
                    {
                        owinServer.Dispose();
                        OwinServers.Remove(owinserverCounter);
                    }
                }
            }
        }
    }
}
