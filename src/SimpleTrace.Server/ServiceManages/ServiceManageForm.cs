﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SimpleTrace.Common;
using SimpleTrace.Server.CallApis;
using SimpleTrace.Server.Init;

namespace SimpleTrace.Server.ServiceManages
{
    public partial class ServiceManageForm : Form
    {
        public ServiceManageFormCtrl Ctrl { get; }

        public ServiceManageForm(ServiceManageFormCtrl ctrl)
        {
            Ctrl = ctrl;
            InitializeComponent();
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.txtMessage.ScrollBars = ScrollBars.Vertical;
            AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });
        }

        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }

        private void ServiceManageForm_Load(object sender, System.EventArgs e)
        {
            var serviceInfo = Ctrl.LoadWindowServiceInfo();
            var formatConfig = Ctrl.FormatConfig(serviceInfo);
            Ctrl.Init(serviceInfo);
            this.txtConfig.Text = formatConfig;
        }

        private void btnGetStatus_Click(object sender, System.EventArgs e)
        {
            //var instanceLogger = SimpleLogSingleton<ServiceManageForm>.Instance.Logger;
            //var fullExePath = Path.GetFullPath(AppDomain.CurrentDomain.Combine(@"DemoFoo\Demo.Foo.exe"));
            //instanceLogger.LogInfo(fullExePath); 
            //var isRun = Process.GetProcessesByName("Demo.Foo.exe").Length > 0;
            //var isRun2 = Process.GetProcessesByName("Demo.Foo").Length > 0;
            //instanceLogger.LogInfo("Demo.Foo.exe isRunning ? " + isRun); //KO
            //instanceLogger.LogInfo("Demo.Foo isRunning ? " + isRun2); //OK

            var tryGetStatus = Ctrl.TheController.TryGetStatus();
            MessageBox.Show(tryGetStatus.Message);
        }


        private void btnInstall_Click(object sender, System.EventArgs e)
        {
            var messageResult = Ctrl.TheController.TryInstall();
            MessageBox.Show(messageResult.Message);
        }

        private void btnUninstall_Click(object sender, System.EventArgs e)
        {
            var messageResult = Ctrl.TheController.TryUninstall();
            MessageBox.Show(messageResult.Message);
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            var messageResult = Ctrl.TheController.TryStart();
            MessageBox.Show(messageResult.Message);
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            var messageResult = Ctrl.TheController.TryStop();
            MessageBox.Show(messageResult.Message);
        }
        private void btnClear_Click(object sender, System.EventArgs e)
        {
            this.txtMessage.Clear();
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            using (var theForm = MyContainer.Instance.GetService<CallApiForm>())
            {
                this.Hide();
                theForm.StartPosition = FormStartPosition.CenterScreen;
                theForm.ShowDialog(this);
                this.Show();
            }
        }
    }
}
