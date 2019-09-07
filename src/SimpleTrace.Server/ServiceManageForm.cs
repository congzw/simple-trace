using System.Windows.Forms;
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
        }

        private void btnRead_Click(object sender, System.EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {

        }
    }
}
