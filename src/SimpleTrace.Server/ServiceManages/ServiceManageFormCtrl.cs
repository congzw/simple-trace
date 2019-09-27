using System;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.Common.WindowsServices;

namespace SimpleTrace.Server.ServiceManages
{
    public class ServiceManageFormCtrl
    {
        public WindowServiceInfo LoadWindowServiceInfo()
        {
            //todo read from config
            var serviceInfo = new WindowServiceInfo();
            serviceInfo.ServiceName = "SimpleTraceWs";
            serviceInfo.ServicePath = "SimpleTraceWs.exe";
            serviceInfo.ServiceFriendlyName = "000-SimpleWs";
            return serviceInfo;
        }

        public string FormatConfig(WindowServiceInfo serviceInfo)
        {
            if (serviceInfo == null)
            {
                return string.Empty;
            }
            return MyModelHelper.MakeIniStringExt(serviceInfo, lastSplit: Environment.NewLine);
        }

        public void Init(WindowServiceInfo info)
        {
            ServiceInfo = info;
            TheController = ServiceController.Create(info);
        }

        public WindowServiceInfo ServiceInfo { get; set; }
        public IServiceController TheController { get; set; }

        public Task ToDo()
        {
            SimpleLogSingleton<ServiceManageFormCtrl>.Instance.Logger.LogInfo("TEST ToDo!");
            return Task.FromResult(0);
        }
    }
}
