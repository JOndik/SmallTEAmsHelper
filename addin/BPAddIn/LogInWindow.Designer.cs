namespace BPAddIn
{
    partial class LogInWindow
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
            this.btnLogIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tfPrihlasMeno = new System.Windows.Forms.TextBox();
            this.pfHeslo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(127, 122);
            this.btnLogIn.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(132, 49);
            this.btnLogIn.TabIndex = 0;
            this.btnLogIn.Text = "Prihlásiť sa";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.clickOnBtnLogIn);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Prihlasovacie meno:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 77);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Heslo:";
            // 
            // tfPrihlasMeno
            // 
            this.tfPrihlasMeno.Location = new System.Drawing.Point(179, 38);
            this.tfPrihlasMeno.Margin = new System.Windows.Forms.Padding(4);
            this.tfPrihlasMeno.Name = "tfPrihlasMeno";
            this.tfPrihlasMeno.Size = new System.Drawing.Size(179, 22);
            this.tfPrihlasMeno.TabIndex = 3;
            // 
            // pfHeslo
            // 
            this.pfHeslo.Location = new System.Drawing.Point(179, 77);
            this.pfHeslo.Margin = new System.Windows.Forms.Padding(4);
            this.pfHeslo.Name = "pfHeslo";
            this.pfHeslo.PasswordChar = '*';
            this.pfHeslo.Size = new System.Drawing.Size(179, 22);
            this.pfHeslo.TabIndex = 4;
            // 
            // LogInWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 197);
            this.Controls.Add(this.pfHeslo);
            this.Controls.Add(this.tfPrihlasMeno);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLogIn);
            this.Location = new System.Drawing.Point(450, 150);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LogInWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Prihlásenie do AIS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tfPrihlasMeno;
        private System.Windows.Forms.TextBox pfHeslo;
    }
}