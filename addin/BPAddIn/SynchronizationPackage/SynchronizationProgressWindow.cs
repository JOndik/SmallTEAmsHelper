using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BPAddIn.SynchronizationPackage
{
    public partial class SynchronizationProgressWindow : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private SynchronizationService synchronizationService;
        private EA.Repository repository;

        public SynchronizationProgressWindow(EA.Repository repository)
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
            this.Close();
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            btnBegin.Visible = false;
            btnBack.Visible = false;
            lbSync.Visible = true;
            lbSync.Refresh();     

            synchronizationService.checkConnectionForSynchronization(repository);

            lbSync.Text = "Synchronizácia prebehla úspešne.";
            btnOK.Visible = true;
            BPAddIn.changesAllowed = true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
