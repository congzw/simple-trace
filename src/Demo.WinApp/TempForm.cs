using System;
using System.Windows.Forms;
using SimpleTrace.Common;
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
            
            InitSimpleLoopTaskCtrl();
        }

        private void InitSimpleLoopTaskCtrl()
        {
            this.cbxLoopSpanSeconds.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxLoopSpanSeconds.Items.Add(1);
            this.cbxLoopSpanSeconds.Items.Add(2);
            this.cbxLoopSpanSeconds.Items.Add(5);
            for (int i = 1; i <= 10; i++)
            {
                this.cbxLoopSpanSeconds.Items.Add(i * 10);
            }
            this.cbxLoopSpanSeconds.SelectedIndex = 0;
            LoopTaskCtrl = new SimpleLoopTaskCtrl();
        }

        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }

        public TempFormCtrl Ctrl { get; set; }

        public SimpleLoopTaskCtrl LoopTaskCtrl { get; set; }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Ctrl.TestMyDictionary();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtMessage.Clear();
        }

        private void btnLoopTaskStart_Click(object sender, EventArgs e)
        {
            var spanSec = (int)this.cbxLoopSpanSeconds.SelectedItem;
            LoopTaskCtrl.Start(TimeSpan.FromSeconds(spanSec));
        }

        private void btnLoopTaskStop_Click(object sender, EventArgs e)
        {
            LoopTaskCtrl.Stop();
        }
    }
}
