using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Demo.WinApp.UI;
using SimpleTrace.Common;

namespace Demo.WinApp
{
    public partial class TraceClientsForm : Form
    {
        public TraceClientsForm()
        {
            InitializeComponent();
            MyInitializeComponent();
        }
        //public AsyncUiHelper AsyncMessageHelper { get; set; }
        public AsyncUiHelperForMessageEventBus AsyncMessageHelper { get; set; }

        public TraceClientsFormCtrl Ctrl { get; set; }

        private void MyInitializeComponent()
        {
            this.checkAutoLine.Checked = true;
            this.checkAutoDate.Checked = true;

            this.cbxCount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxCount.Items.Add(1);
            this.cbxCount.Items.Add(2);
            this.cbxCount.Items.Add(5);
            for (int i = 1; i <= 10; i++)
            {
                this.cbxCount.Items.Add(i * 10);
            }
            this.cbxCount.SelectedIndex = 0;

            this.cbxInterval.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxInterval.Items.Add(10);
            this.cbxInterval.Items.Add(20);
            this.cbxInterval.Items.Add(50);
            this.cbxInterval.Items.Add(10 * 10);
            this.cbxInterval.Items.Add(20 * 10);
            this.cbxInterval.Items.Add(50 * 10);
            this.cbxInterval.Items.Add(10 * 100);
            this.cbxInterval.Items.Add(20 * 100);
            this.cbxInterval.Items.Add(50 * 100);

            this.cbxInterval.SelectedIndex = 0;

            AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });
            //AsyncMessageHelper = this.txtMessage.CreateAsyncUiHelper(message => { this.txtMessage.AppendText(message); });

            Ctrl = new TraceClientsFormCtrl();

        }

        private void TraceClientsForm_Load(object sender, EventArgs e)
        {

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            AsyncMessageHelper.AutoAppendLine = this.checkAutoLine.Checked;
            AsyncMessageHelper.WithDatePrefix = this.checkAutoDate.Checked;
            AsyncMessageHelper.SafeUpdateUi("CallTraceApi()");
            var args = GetCallTraceApiArgs();
            await Ctrl.CallTraceApi(args);
            //StartAsyncMessageDemo(args.Count, args.Interval);
        }

        private CallTraceApiArgs GetCallTraceApiArgs()
        {
            var callTraceApiArgs = new CallTraceApiArgs();
            callTraceApiArgs.Count = (int)this.cbxCount.SelectedItem;
            callTraceApiArgs.IntervalMs = (int)this.cbxInterval.SelectedItem;
            return callTraceApiArgs;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            StopAsyncMessageDemo();
            Task.Delay(100).Wait();
            this.txtMessage.Clear();
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            var queueInfo = await Ctrl.QueryQueue();
            this.txtMessage.Text = queueInfo.ToJson(true);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var queueInfo = await Ctrl.QueryQueue();
            await Ctrl.SaveQueue(queueInfo);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

        }

        #region demo for async
        
        private bool _processing = false;
        private int _messageIndex = 0;

        private void StartAsyncMessageDemo(int count , int interval)
        {
            _messageIndex = 0;
            _processing = true;
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(interval)).Wait();
                    if (!_processing)
                    {
                        break;
                    }
                    _messageIndex++;

                    AsyncMessageHelper.SafeUpdateUi("message " + _messageIndex);
                }
                _processing = false;
            });
        }

        private void StopAsyncMessageDemo()
        {
            _processing = false;
            this.txtMessage.Text = @"-- stopped and cleared! --";
            Task.Delay(300).Wait();
            this.txtMessage.Clear();
        }

        #endregion
    }
}
