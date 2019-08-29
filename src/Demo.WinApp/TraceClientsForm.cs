using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Demo.WinApp.UI;

namespace Demo.WinApp
{
    public partial class TraceClientsForm : Form
    {
        public TraceClientsForm()
        {
            InitializeComponent();
            MyInitializeComponent();
        }
        public AsyncUiHelper FormHelper { get; set; }

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
            this.cbxInterval.Items.Add(1);
            this.cbxInterval.Items.Add(2);
            this.cbxInterval.Items.Add(5);
            this.cbxInterval.Items.Add(10);
            this.cbxInterval.Items.Add(30);
            this.cbxInterval.Items.Add(60);
            this.cbxInterval.SelectedIndex = 0;
            
            var asyncFormHelper = AsyncUiHelper.Create(this.txtMessage, message => { this.txtMessage.AppendText(message); });
            FormHelper = asyncFormHelper;

            Ctrl = new TraceClientsFormCtrl();
        }

        private void TraceClientsForm_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            FormHelper.AutoAppendLine = this.checkAutoLine.Checked;
            FormHelper.WithDatePrefix = this.checkAutoDate.Checked;
            var args = GetCallTraceApiArgs();

            //StartAsyncMessageDemo(args.Count, args.Interval);

            var result = Ctrl.CallTraceApi(args);
            FormHelper.SafeUpdateUi(result.Message);

        }

        private CallTraceApiArgs GetCallTraceApiArgs()
        {
            var callTraceApiArgs = new CallTraceApiArgs();
            callTraceApiArgs.Count = (int)this.cbxCount.SelectedItem;
            callTraceApiArgs.Interval = (int)this.cbxInterval.SelectedItem;
            return callTraceApiArgs;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //StopAsyncMessageDemo();
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
                    Task.Delay(TimeSpan.FromSeconds(interval)).Wait();
                    if (!_processing)
                    {
                        break;
                    }
                    _messageIndex++;

                    FormHelper.SafeUpdateUi("message " + _messageIndex);
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
