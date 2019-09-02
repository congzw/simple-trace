using System;

namespace SimpleTrace.TraceClients.ApiProxy
{
    public class ApiProxyContext
    {
        public static IClientTracerApiProxy Current => ClientTracerApiProxySmartWrapper.Resolve();
    }

    public class ApiProxyInit
    {
        public static Action<IClientTracerApiProxy, TimeSpan?, Func<DateTime>> Reset = ResetForWrapper;
        private static void ResetForWrapper(IClientTracerApiProxy apiProxy, TimeSpan? checkApiStatusInterval, Func<DateTime> getDateNow)
        {
            var clientTracerApiProxySmartWrapper = ApiProxyContext.Current as ClientTracerApiProxySmartWrapper;
            if (clientTracerApiProxySmartWrapper == null)
            {
                throw new InvalidOperationException("this reset method is for ClientTracerApiProxySmartWrapper instance, but Current is not instance of it!");
            }

            clientTracerApiProxySmartWrapper.Reset(apiProxy, checkApiStatusInterval, getDateNow);
        }
    }
}