namespace BPAddIn
{
    partial class JoinWindow
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
            this.btnConfirmNames = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tfSecondMember = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnConfirmNames
            // 
            this.btnConfirmNames.Location = new System.Drawing.Point(171, 85);
            this.btnConfirmNames.Name = "btnConfirmNames";
            this.btnConfirmNames.Size = new System.Drawing.Size(99, 39);
            this.btnConfirmNames.TabIndex = 0;
            this.btnConfirmNames.Text = "Potvrdiť";
            this.btnConfirmNames.UseVisualStyleBackColor = true;
            this.btnConfirmNames.Click += new System.EventHandler(this.clickOnBtnConfirm);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Prihlasovacie meno vášho kolegu do AIS:";
            // 
            // tfSecondMember
            // 
            this.tfSecondMember.Location = new System.Drawing.Point(263, 39);
            this.tfSecondMember.Name = "tfSecondMember";
            this.tfSecondMember.Size = new System.Drawing.Size(154, 20);
            this.tfSecondMember.TabIndex = 2;
            // 
            // JoinWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 156);
            this.Controls.Add(this.tfSecondMember);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConfirmNames);
            this.Name = "JoinWindow";
            this.Text = "Spojenie projektov";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConfirmNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tfSecondMember;
    }
}