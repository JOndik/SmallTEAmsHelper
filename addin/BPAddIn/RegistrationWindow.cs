using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public partial class RegistrationWindow : Form
    {
        private RegistrationService registrationService = new RegistrationService();

        public RegistrationWindow()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = btnRegister;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            String result = registrationService.checkConnection(tfUsername.Text, pfPassword.Text);
            if (("noconnection").Equals(result))
            {
                this.Close();
                MessageBox.Show("Server is unavailable. Check your internet connection.");
            }
            else if (("wrongName").Equals(result))
            {
                MessageBox.Show("User with the inserted username already exists.");
            }
            else if (("notFilled").Equals(result))
            {
                MessageBox.Show("Fill in both username, and password.");
            }
            else if (("error").Equals(result))
            {
                this.Close();
                MessageBox.Show("Unexpected error has occured.");
            }
            else
            {
                this.Close();
                MessageBox.Show("You have been signed up successfully.");
            }
        }
    }
}
