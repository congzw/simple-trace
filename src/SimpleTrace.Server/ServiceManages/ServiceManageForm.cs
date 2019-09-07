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

        private async void btnRead_Click(object sender, System.EventArgs e)
        {
            await Ctrl.ToDo();
            AsyncMessageHelper.SafeUpdateUi("ToDo");
        }

        private async void btnLoad_Click(object sender, System.EventArgs e)
        {
            await Ctrl.ToDo();
            AsyncMessageHelper.SafeUpdateUi("ToDo");
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            this.txtMessage.Clear();
        }
    }
}
