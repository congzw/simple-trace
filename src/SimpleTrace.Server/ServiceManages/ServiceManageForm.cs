using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

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
    }
}
