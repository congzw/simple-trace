using System;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Common;

namespace SimpleTrace.Server.Init
{
    public class AppMain
    {
        public MyContainer Container { get; set; }
        
        public void InitContainer()
        {
            Container = MyContainer.Instance;
            Container.Init(new Startup().ConfigureServices);
        }

        public static bool IsRunAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void HandleGlobalException()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public Form CreateEntryForm()
        {
            var simpleIniFile = SimpleIni.ResolveFile();
            var items = simpleIniFile.TryLoadIniFileItems("SimpleTrace.ini");
            var value = simpleIniFile.GetValue(items, "Entry", "FormName");
            var entryFormName = value == null ?  string.Empty : value.ToString().Trim();

            if (entryFormName.Equals("CallApiForm", StringComparison.Ordinal))
            {
                return Container.GetService<CallApis.CallApiForm>();
            }

            if (entryFormName.Equals("ServiceManage", StringComparison.Ordinal))
            {
                return Container.GetService<ServiceManages.ServiceManageForm>();
            }
            
            //default
            return Container.GetService<Demos.DemoForm>();
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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            Console.WriteLine(e.ExceptionObject);
        }


        public static AppMain Instance = new AppMain();
    }
}
