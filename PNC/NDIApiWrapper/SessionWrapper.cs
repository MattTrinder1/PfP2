using NDIXML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDIApiWrapper
{
    public class SessionWrapper : IDisposable
    {
        private bool disposedValue;

        internal ConsoliDataWSSoapClient client;
        string sessionInfo;
        internal PNCScreen connectResponse;
        internal PNCScreen logonResponse;

        public SessionWrapper(string url, string user, string session)
        {
            client = new NDIXML.ConsoliDataWSSoapClient(ConsoliDataWSSoapClient.EndpointConfiguration.ConsoliDataWSSoap12, url);
            connectResponse = client.HostConnect2("", user, session); ;
            logonResponse = client.PNCAutoLogon(connectResponse.Session.SessionInfo);

            sessionInfo = connectResponse.Session.SessionInfo;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                var disconnect = client.HostDisconnect(sessionInfo);
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
         ~SessionWrapper()
         {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
         }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
