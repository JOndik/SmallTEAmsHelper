
namespace BPAddIn.SynchronizationPackage
{
    public partial class SynchronizationProgressWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.lbSync = new System.Windows.Forms.Label();
            this.btnBegin = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(176, 95);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbSync
            // 
            this.lbSync.AutoSize = true;
            this.lbSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbSync.Location = new System.Drawing.Point(41, 42);
            this.lbSync.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSync.Name = "lbSync";
            this.lbSync.Size = new System.Drawing.Size(266, 24);
            this.lbSync.TabIndex = 2;
            this.lbSync.Text = "Synchronization is in progress.";
            this.lbSync.Visible = false;
            // 
            // btnBegin
            // 
            this.btnBegin.Location = new System.Drawing.Point(45, 41);
            this.btnBegin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(196, 28);
            this.btnBegin.TabIndex = 3;
            this.btnBegin.Text = "Start synchronization";
            this.btnBegin.UseVisualStyleBackColor = true;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(293, 41);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 28);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "Cancel";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // SynchronizationProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 150);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.lbSync);
            this.Controls.Add(this.btnOK);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SynchronizationProgressWindow";
            this.Text = "Synchronization";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbSync;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.Button btnBack;
    }
}