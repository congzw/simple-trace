using System;
using System.Windows.Forms;
using SimpleTrace.Server.Init;

namespace SimpleTrace.Server
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var appMain = AppMain.Instance;
            appMain.InitContainer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            appMain.HandleGlobalException();

            if (appMain.NeedRunAsAdmin() && !appMain.IsRunAsAdmin())
            {
                var message = string.Format("{0}{1}{2}", "服务的安装、卸载需要管理员身份！", Environment.NewLine,
                    "请尝试使用右键，然后以管理员身份运行此程序！");
                MessageBox.Show(message);
                return;
            }

            var form = appMain.CreateEntryForm();
            if (form == null)
            {
                MessageBox.Show(@"Fail to create entry form!");
                return;
            }
            Application.Run(form);
        }
    }
}
