using System;
using System.Threading;
using System.Windows.Forms;
using Common;
using SimpleTrace.Server.Init;

namespace SimpleTrace.Server
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var myContainer = MyContainer.Instance;
            myContainer.Init(new Startup().ConfigureServices);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            //var form = myContainer.GetService<DemoForm>();
            var form = myContainer.GetService<ServiceManageForm>();
            if (form == null)
            {
                MessageBox.Show(@"Fail to create entry form!");
                return;
            }
            Application.Run(form);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.threadexception
            //ThreadException event to handle UI thread exceptions
            //UnhandledException event to handle non-UI thread exceptions.
            //UnhandledException cannot prevent an application from terminating

            if (e.Exception is Exception ex)
            {
                var logger = MyContainer.Instance.GetService<ISimpleLogFactory>().GetOrCreate(null);
                logger.LogEx(ex);
            }
        }
    }
}
