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
            var appHelper = AppHelper.Instance;
            appHelper.InitContainer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            appHelper.HandleGlobalException();

            var form = appHelper.CreateEntryForm();
            if (form == null)
            {
                MessageBox.Show(@"Fail to create entry form!");
                return;
            }
            Application.Run(form);
        }
    }
}
