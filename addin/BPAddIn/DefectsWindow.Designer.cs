using BPAddInTry;
using System.Windows.Forms;

namespace BPAddIn
{
    partial class DefectsWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Rule selectedError;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zvyrazniVDiagrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opravChybuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(706, 173);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zvyrazniVDiagrameToolStripMenuItem,
            this.opravChybuToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
            // 
            // zvyrazniVDiagrameToolStripMenuItem
            // 
            this.zvyrazniVDiagrameToolStripMenuItem.Name = "zvyrazniVDiagrameToolStripMenuItem";
            this.zvyrazniVDiagrameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.zvyrazniVDiagrameToolStripMenuItem.Text = "Zvyrazni v diagrame";
            this.zvyrazniVDiagrameToolStripMenuItem.Click += new System.EventHandler(this.zvyrazniVDiagrameToolStripMenuItem_Click);
            // 
            // opravChybuToolStripMenuItem
            // 
            this.opravChybuToolStripMenuItem.Name = "opravChybuToolStripMenuItem";
            this.opravChybuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.opravChybuToolStripMenuItem.Text = "Oprav chybu";
            this.opravChybuToolStripMenuItem.Click += new System.EventHandler(this.opravChybuToolStripMenuItem_Click);
            // 
            // DefectsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox1);
            this.Name = "DefectsWindow";
            this.Size = new System.Drawing.Size(706, 177);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem zvyrazniVDiagrameToolStripMenuItem;
        private ToolStripMenuItem opravChybuToolStripMenuItem;
    }
}
