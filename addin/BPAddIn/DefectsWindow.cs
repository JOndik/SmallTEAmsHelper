using System.Windows.Forms;
using System.Runtime.InteropServices;
using BPAddInTry;
using System;
using BPAddIn.Rules;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{

    [Guid("f2273d31-26ab-42f1-bbdc-9c687adca936")]
    [ComVisible(true)]
    public partial class DefectsWindow : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectsWindow"/> class.
        /// </summary>
        public DefectsWindow()
        {
            InitializeComponent();
        }

        public int isHidden(string ruleGUID)
        {
            for (int i = listBox2.Items.Count - 1; i >= 0; i--)
            {
                try {
                    if (((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).GUID == ruleGUID)
                    {
                        return 1;
                    }
                }
                catch (Exception ex) { }
            }

            return 0;
        }

        public void addToList(object objekt)
        {
            listBox1.BeginInvoke((MethodInvoker)delegate () {
                for (int i = listBox2.Items.Count - 1; i >= 0; i--)
                {
                    if (((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID ||
                            ((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).name == ((RuleEntry)((ListBoxObject)objekt).Object).name)
                    {
                        return;
                    }
                }

                for (int i = listBox1.Items.Count - 1; i >= 0; i--)
                {
                    if (((RuleEntry)((ListBoxObject)listBox1.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID ||
                            ((RuleEntry)((ListBoxObject)listBox1.Items[i]).Object).name == ((RuleEntry)((ListBoxObject)objekt).Object).name)
                    {
                        listBox1.Items.RemoveAt(i);
                        listBox1.Items.Insert(i, objekt);
                        return;
                    }
                }

                listBox1.Items.Add(objekt);
            });

            updateTabNames();
        }

        public void addToHiddenList(object objekt)
        {
            listBox2.BeginInvoke((MethodInvoker)delegate () {
                for (int i = listBox2.Items.Count - 1; i >= 0; i--)
                {
                    if (((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID ||
                            ((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).name == ((RuleEntry)((ListBoxObject)objekt).Object).name)
                    {
                        listBox2.Items.RemoveAt(i);
                        listBox2.Items.Insert(i, objekt);
                        return;
                    }
                }

                listBox2.Items.Add(objekt);
            });

            updateTabNames();
        }

        public void removeFromList(object objekt)
        {
            listBox1.BeginInvoke((MethodInvoker)delegate () {
                for (int i = listBox1.Items.Count - 1; i >= 0; i--)
                {
                    try {
                        if (((RuleEntry)((ListBoxObject)listBox1.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID ||
                            ((RuleEntry)((ListBoxObject)listBox1.Items[i]).Object).name == ((RuleEntry)((ListBoxObject)objekt).Object).name)
                        {
                            listBox1.Items.RemoveAt(i);
                        }
                    }
                    catch (Exception ex) { }
                }
            });

            updateTabNames();
        }

        public void removeFromHiddenList(object objekt)
        {
            listBox2.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = listBox2.Items.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID ||
                            ((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).name == ((RuleEntry)((ListBoxObject)objekt).Object).name)
                        {
                            listBox2.Items.RemoveAt(i);
                        }
                    }
                    catch (Exception ex) { }
                }
            });
        }

        public void removeFromListSpecial(object objekt)
        {
            listBox1.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = listBox1.Items.Count - 1; i >= 0; i--)
                {
                    if (((ListBoxObject)listBox1.Items[i]).Object == ((RuleEntry) objekt))
                    {
                        listBox1.Items.RemoveAt(i);
                    }
                }
            });

            listBox2.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = listBox2.Items.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (((RuleEntry)((ListBoxObject)listBox2.Items[i]).Object).GUID == ((RuleEntry)((ListBoxObject)objekt).Object).GUID)
                        {
                            listBox2.Items.RemoveAt(i);
                        }
                    }
                    catch (Exception ex) { }
                }
            });

            updateTabNames();
        }

        private void zvyrazniVDiagrameToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try {
                if (selectedError.rule.elementType == "Class")
                {
                    ElementWrapper elementWrapper = new ElementWrapper(selectedError.model, selectedError.model.getWrappedModel().GetElementByGuid(selectedError.elGUID));
                    elementWrapper.select();
                    elementWrapper.selectInCurrentDiagram();
                }

            }
            catch (Exception ex) { }
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
                    selectedError = (RuleEntry)item.Object;
                }
            }
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var idx = listBox2.IndexFromPoint(e.Location);
                if (idx >= 0 && listBox2.GetItemRectangle(idx).Contains(e.Location))
                {
                    listBox2.SelectedIndex = idx;
                    contextMenuStrip2.Show(listBox2, e.Location);
                    ListBoxObject item = (ListBoxObject)listBox2.SelectedItem;
                    selectedError = (RuleEntry)item.Object;
                }
            }
        }

        private void opravChybuToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (selectedError.rule.correct(selectedError.element, selectedError.model))
                {
                    removeFromList(selectedError.listBoxObject);
                    removeFromHiddenList(selectedError.listBoxObject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void skryChybuToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            addToHiddenList(selectedError.listBoxObject);
            removeFromList(selectedError.listBoxObject);
        }

        private void zobrazChybuToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            removeFromHiddenList(selectedError.listBoxObject);
            addToList(selectedError.listBoxObject);
        }

        private void updateTabNames()
        {
            tabControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                tabControl1.TabPages[0].Name = "Chyby (" + listBox1.Items.Count + ")";
                tabControl1.TabPages[1].Name = "Skryté chyby (" + listBox2.Items.Count + ")";
            });
        }
    }
}
