using System;
using System.Windows.Forms;
using Common;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Server.Demos
{
    public partial class DemoForm : Form
    {
        public DemoForm(DemoFormCtrl demoFormCtrl)
        {
            Ctrl = demoFormCtrl;
            InitializeComponent();
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.txtMessage.ScrollBars = ScrollBars.Vertical;
            AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });
        }

        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }


        public DemoFormCtrl Ctrl { get; set; }

        private void ServicesForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSingleton_Click(object sender, EventArgs e)
        {
            Ctrl.ShowInfos(ServiceLifetime.Singleton);
        }

        private void btnTransient_Click(object sender, EventArgs e)
        {
            Ctrl.ShowInfos(ServiceLifetime.Transient);
        }

        private void btnThrowEx_Click(object sender, EventArgs e)
        {
            Ctrl.ThrowEx("App Ex At: " + DateTime.Now);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtMessage.Clear();
        }
    }
}
