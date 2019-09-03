﻿using System;
using System.Windows.Forms;

namespace Demo.WinApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppInit.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TraceClientsForm());
            Application.Run(new TempForm());
        }
    }
}
