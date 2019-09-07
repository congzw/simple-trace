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
