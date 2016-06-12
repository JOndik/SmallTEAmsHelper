namespace BPAddIn.SynchronizationPackage
{
    partial class SendingDataWindow
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
            this.lbWait = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lbSend = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbWait
            // 
            this.lbWait.AutoSize = true;
            this.lbWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbWait.Location = new System.Drawing.Point(35, 63);
            this.lbWait.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(299, 18);
            this.lbWait.TabIndex = 0;
            this.lbWait.Text = "This process may take maximally 5 minutes.";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(98, 111);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(208, 28);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start sending data";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(392, 111);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 28);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Cancel";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(246, 111);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(100, 28);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "OK";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Visible = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lbSend
            // 
            this.lbSend.AutoSize = true;
            this.lbSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbSend.Location = new System.Drawing.Point(35, 28);
            this.lbSend.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSend.Name = "lbSend";
            this.lbSend.Size = new System.Drawing.Size(519, 18);
            this.lbSend.TabIndex = 4;
            this.lbSend.Text = "For start of synchronization, it is necessary to send data about chosen model.";
            // 
            // SendingDataWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 169);
            this.Controls.Add(this.lbSend);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbWait);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SendingDataWindow";
            this.Text = "Send data about model";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbWait;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Label lbSend;
    }
}