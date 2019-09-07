using System.Windows.Forms;
using Common;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server
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
            var clientSpans = await Ctrl.ReadClientSpanEntities();
            AsyncMessageHelper.SafeUpdateUi("ReadSpans: " + clientSpans.Count);
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {

        }
    }
}
