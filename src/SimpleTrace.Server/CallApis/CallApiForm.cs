using System.Windows.Forms;
using Common;

namespace SimpleTrace.Server.CallApis
{
    public partial class CallApiForm : Form
    {
        public CallApiFormCtrl Ctrl { get; }

        public CallApiForm(CallApiFormCtrl ctrl)
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

        private async void btnLoad_Click(object sender, System.EventArgs e)
        {
            var clientSpans = await Ctrl.ReadClientSpanEntities();
            AsyncMessageHelper.SafeUpdateUi("ReadSpans: " + clientSpans.Count);
            await Ctrl.SendApiSpans(clientSpans);
            AsyncMessageHelper.SafeUpdateUi("LoadSpans: " + clientSpans.Count);
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            this.txtMessage.Clear();
        }
    }
}
