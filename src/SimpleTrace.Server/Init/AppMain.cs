using System;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using SimpleTrace.Common;

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

        public bool IsRunAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public bool NeedRunAsAdmin()
        {
            var simpleIniFile = SimpleIni.ResolveFile();
            var items = simpleIniFile.TryLoadIniFileItems("SimpleTrace.ini");
            var runAsAdminValue = simpleIniFile.GetValue(items, "Entry", "RunAsAdmin");
            var runAsAdmin = runAsAdminValue == null ? "false" : runAsAdminValue.ToString().Trim();

            bool.TryParse(runAsAdmin, out var needRunAsAdmin);
            return needRunAsAdmin;
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
            var entryFormNameValue = simpleIniFile.GetValue(items, "Entry", "FormName");
            var entryFormName = entryFormNameValue == null ? string.Empty : entryFormNameValue.ToString().Trim();

            if (entryFormName.IsSameName(KnownFormNames.CallApiForm))
            {
                return Container.GetService<CallApis.CallApiForm>();
            }

            if (entryFormName.IsSameName(KnownFormNames.ServiceManageForm))
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

    public static class KnownFormNames
    {
        public static string ServiceManageForm = "ServiceManageForm";
        public static string CallApiForm = "CallApiForm";
        public static string DemoForm = "DemoForm";

        public static bool IsSameName(this string name1, string name2)
        {
            if (name1 == null)
            {
                return false;
            }

            if (name2 == null)
            {
                return false;
            }

            return name1.Trim().Equals(name2.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
