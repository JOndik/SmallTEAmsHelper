using System.Windows.Forms;
using System.Runtime.InteropServices;
using BPAddInTry;

namespace BPAddIn
{

    [Guid("f2273d31-26ab-42f1-bbdc-9c687adca936")]
    [ComVisible(true)]
    public partial class DefectsWindow : UserControl
    {

        // functions/properties and events can be added as you like

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectsWindow"/> class.
        /// </summary>
        public DefectsWindow()
        {
            InitializeComponent();
        }

        public void addToList(object objekt)
        {
            listBox1.BeginInvoke((MethodInvoker)delegate () { listBox1.Items.Add(objekt); });
        }

        public void removeFromList(object objekt)
        {
            listBox1.BeginInvoke((MethodInvoker)delegate () { listBox1.Items.Remove(objekt); });
        }

        private void zvyrazniVDiagrameToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //ListBox lb = (ListBox)sender;
            //Rule rule = (Rule)sender;
            selectedError.selectInDiagram();
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var idx = listBox1.IndexFromPoint(e.Location);
                if (idx >= 0 && listBox1.GetItemRectangle(idx).Contains(e.Location))
                {
                    listBox1.SelectedIndex = idx;
                    contextMenuStrip1.Show(listBox1, e.Location);
                    ListBoxObject item = (ListBoxObject)listBox1.SelectedItem;
                    selectedError = (Rule)item.Object;
                }
            }
        }

        private void opravChybuToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            selectedError.correct();
        }
    }
}
