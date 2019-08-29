using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace Demo.WinApp
{
    public partial class TraceClientsForm : Form
    {
        public AsyncUiHelper FormHelper { get; set; }

        public TraceClientsForm()
        {
            InitializeComponent();
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.cbxAutoLine.Checked = true;
            this.cbxAutoDate.Checked = true;

            this.cbxCount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxCount.Items.Add(1);
            this.cbxCount.Items.Add(2);
            this.cbxCount.Items.Add(5);
            for (int i = 1; i <= 10; i++)
            {
                this.cbxCount.Items.Add(i * 10);
            }
            this.cbxCount.SelectedIndex = 0;

            this.cbxSeconds.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxSeconds.Items.Add(1);
            this.cbxSeconds.Items.Add(2);
            this.cbxSeconds.Items.Add(5);
            this.cbxSeconds.Items.Add(10);
            this.cbxSeconds.Items.Add(30);
            this.cbxSeconds.Items.Add(60);
            this.cbxSeconds.SelectedIndex = 0;
            
            var asyncFormHelper = AsyncUiHelper.Create(this.txtMessage, message => { this.txtMessage.AppendText(message); });
            FormHelper = asyncFormHelper;
        }

        private void TraceClientsForm_Load(object sender, EventArgs e)
        {

        }

        private bool _processing = false;
        private int _messageIndex = 0;
        private void btnStart_Click(object sender, EventArgs e)
        {
            FormHelper.AutoAppendLine = this.cbxAutoLine.Checked;
            FormHelper.WithDatePrefix = this.cbxAutoDate.Checked;

            _messageIndex = 0;
            _processing = true;
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Task.Delay(200).Wait();
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            _processing = false;
            this.txtMessage.Text = @"-- stopped and cleared! --";
            Task.Delay(300).Wait();
            this.txtMessage.Clear();
        }
    }
}
