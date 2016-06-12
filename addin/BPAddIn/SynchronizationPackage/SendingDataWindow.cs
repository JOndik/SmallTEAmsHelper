using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace BPAddIn.SynchronizationPackage
{
    public partial class SendingDataWindow : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private SynchronizationService synchronizationService;
        private EA.Repository repository;

        public SendingDataWindow(EA.Repository repository)
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            this.synchronizationService = new SynchronizationService(repository);
            this.repository = repository;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnStart.Visible = false;
            btnBack.Visible = false;
            lbSend.Visible = false;
            lbWait.Visible = true;            
            lbWait.Text = "Prosím, počkajte. Prebieha posielanie dát o modeli.";
            lbWait.Refresh();

            synchronizationService.sendDataAboutModel(this, repository);
        }

        public void setVisible(bool visible)
        {
            lbWait.BeginInvoke((MethodInvoker)delegate() { lbWait.Text = "Posielanie dát bolo úspešne dokončené."; });
            btnConfirm.BeginInvoke((MethodInvoker)delegate() { btnConfirm.Visible = visible; });
        }

        public void showMessage()
        {
            lbWait.BeginInvoke((MethodInvoker)delegate() { lbWait.Text = "Počas posielania dát sa vyskytol problém s internetovým pripojením."; });
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
