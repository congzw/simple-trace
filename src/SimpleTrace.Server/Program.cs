using System;
using System.Windows.Forms;
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
            //Application.Run(new MainForm());
            Application.Run(myContainer.GetService<ServicesForm>());
        }
    }
}
