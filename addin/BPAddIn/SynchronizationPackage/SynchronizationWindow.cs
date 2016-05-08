using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BPAddIn.SynchronizationPackage
{
    [Guid("DBE55F7B-A312-4BCB-B43F-3587B20C0571")]
    [ComVisible(true)]
    public partial class SynchronizationWindow : UserControl
    {
        public SynchronizationWindow()
        {
            InitializeComponent();
        }

        public void addToList(string change)
        {
            listBox.BeginInvoke((MethodInvoker)delegate()
            {
                listBox.Items.Add(change);
            });
        }

        public void removeList()
        {
            listBox.BeginInvoke((MethodInvoker)delegate()
            {
                for (int i = listBox.Items.Count - 1; i >= 0; i--)
                {
                    listBox.Items.RemoveAt(i);
                }
            });
        }
    }
}
