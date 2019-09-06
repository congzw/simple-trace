using System;
using System.Windows.Forms;
using Common;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server
{
    public partial class ServicesForm : Form
    {
        public ServicesForm(ServicesFormCtrl servicesFormCtrl)
        {
            Ctrl = servicesFormCtrl;
            InitializeComponent();
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.txtMessage.ScrollBars = ScrollBars.Vertical;
            AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });
        }

        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }


        public ServicesFormCtrl Ctrl { get; set; }

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

        private void btnRun_Click(object sender, EventArgs e)
        {
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtMessage.Clear();
        }
    }
}
