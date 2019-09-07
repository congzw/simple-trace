using System;
using Common;

namespace SimpleTrace.Server.Demos
{
    public interface IFooService : IDisposable
    {
    }

    public class FooService : IFooService
    {
        public FooService()
        {
            DemoReport.ReportCreate(this);
        }

        public void Dispose()
        {
            DemoReport.ReportDispose(this);
        }
    }

    public class BarService : IDisposable
    {
        private readonly IFooService _child;
        public BarService(IFooService child)
        {
            DemoReport.ReportCreate(this);
            _child = child;
        }

        public void Dispose()
        {
            DemoReport.ReportDispose(this);
        }
    }

    public class DemoReport
    {
        public static void ReportDispose(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var theType = instance.GetType();
            var info = string.Format("          {0}:{1} Disposed >>", theType.Name, instance.GetHashCode());
            var logger = SimpleLogSingleton<DemoReport>.Instance.Logger;
            logger.LogInfo(info);
        }

        public static void ReportCreate(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var theType = instance.GetType();
            var info = string.Format("<<{0}:{1} Created", theType.Name, instance.GetHashCode());
            var logger = SimpleLogSingleton<DemoReport>.Instance.Logger;
            logger.LogInfo(info);
        }
    }
}
