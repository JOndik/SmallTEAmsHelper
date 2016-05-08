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
            this.skryťToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zobraziťToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zobraziťToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(1, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(691, 212);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zvyrazniVDiagrameToolStripMenuItem,
            this.opravChybuToolStripMenuItem,
            this.skryťToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
            // 
            // zvyrazniVDiagrameToolStripMenuItem
            // 
            this.zvyrazniVDiagrameToolStripMenuItem.Name = "zvyrazniVDiagrameToolStripMenuItem";
            this.zvyrazniVDiagrameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.zvyrazniVDiagrameToolStripMenuItem.Text = "Zvýrazní v diagrame";
            this.zvyrazniVDiagrameToolStripMenuItem.Click += new System.EventHandler(this.zvyrazniVDiagrameToolStripMenuItem_Click);
            // 
            // opravChybuToolStripMenuItem
            // 
            this.opravChybuToolStripMenuItem.Name = "opravChybuToolStripMenuItem";
            this.opravChybuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.opravChybuToolStripMenuItem.Text = "Oprav chybu";
            this.opravChybuToolStripMenuItem.Click += new System.EventHandler(this.opravChybuToolStripMenuItem_Click);
            // 
            // skryťToolStripMenuItem
            // 
            this.skryťToolStripMenuItem.Name = "skryťToolStripMenuItem";
            this.skryťToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.skryťToolStripMenuItem.Text = "Skryť";
            this.skryťToolStripMenuItem.Click += new System.EventHandler(this.skryChybuToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(700, 240);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(692, 214);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Chyby";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(692, 214);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Skryté chyby";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(1, 0);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(691, 212);
            this.listBox2.TabIndex = 0;
            this.listBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox2_MouseDown);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zobraziťToolStripMenuItem,
            this.zobraziťToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 70);
            // 
            // zobraziťToolStripMenuItem
            // 
            this.zobraziťToolStripMenuItem.Name = "zobraziťToolStripMenuItem";
            this.zobraziťToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zobraziťToolStripMenuItem.Text = "Oprav chybu";
            this.zobraziťToolStripMenuItem.Click += new System.EventHandler(this.opravChybuToolStripMenuItem_Click);
            // 
            // zobraziťToolStripMenuItem1
            // 
            this.zobraziťToolStripMenuItem1.Name = "zobraziťToolStripMenuItem1";
            this.zobraziťToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.zobraziťToolStripMenuItem1.Text = "Zobraziť";
            this.zobraziťToolStripMenuItem1.Click += new System.EventHandler(this.zobrazChybuToolStripMenuItem_Click);
            // 
            // DefectsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "DefectsWindow";
            this.Size = new System.Drawing.Size(706, 246);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem zvyrazniVDiagrameToolStripMenuItem;
        private ToolStripMenuItem opravChybuToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ListBox listBox2;
        private ToolStripMenuItem skryťToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem zobraziťToolStripMenuItem;
        private ToolStripMenuItem zobraziťToolStripMenuItem1;
    }
}
