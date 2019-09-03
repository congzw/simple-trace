using System;
using System.Windows.Forms;
using Common;
using Demo.WinApp.UI;

namespace Demo.WinApp
{
    public partial class TempForm : Form
    {
        public TempForm()
        {
            InitializeComponent();
            MyInitializeComponent();
        }
        private void MyInitializeComponent()
        {
            this.txtMessage.ScrollBars = ScrollBars.Vertical;
            AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });

            Ctrl = new TempFormCtrl();
        }

        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }

        public TempFormCtrl Ctrl { get; set; }
        private void btnRun_Click(object sender, EventArgs e)
        {
            Ctrl.TestMyDictionary();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }
    }
}
