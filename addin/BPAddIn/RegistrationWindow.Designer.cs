namespace BPAddIn
{
    partial class RegistrationWindow
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
            this.pfPassword = new System.Windows.Forms.TextBox();
            this.tfUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pfPassword
            // 
            this.pfPassword.Location = new System.Drawing.Point(128, 77);
            this.pfPassword.Margin = new System.Windows.Forms.Padding(4);
            this.pfPassword.Name = "pfPassword";
            this.pfPassword.PasswordChar = '*';
            this.pfPassword.Size = new System.Drawing.Size(231, 22);
            this.pfPassword.TabIndex = 9;
            // 
            // tfUsername
            // 
            this.tfUsername.Location = new System.Drawing.Point(128, 38);
            this.tfUsername.Margin = new System.Windows.Forms.Padding(4);
            this.tfUsername.Name = "tfUsername";
            this.tfUsername.Size = new System.Drawing.Size(231, 22);
            this.tfUsername.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 77);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Username*:";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(128, 155);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(4);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(132, 49);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Text = "Sign up";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "* username must be valid email address";
            // 
            // RegistrationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 226);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pfPassword);
            this.Controls.Add(this.tfUsername);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRegister);
            this.Name = "RegistrationWindow";
            this.Text = "Sign up";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pfPassword;
        private System.Windows.Forms.TextBox tfUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label label1;

    }
}