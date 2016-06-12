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
    public partial class LogInWindow : Form
    {
        private LogInService logInService = new LogInService();

        public LogInWindow()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = btnLogIn;
        }

        private void clickOnBtnLogIn(object sender, EventArgs e)
        {
            string result;

            if (!tfPrihlasMeno.Text.Equals("") && !pfHeslo.Text.Equals(""))
            {
                result = logInService.checkConnection(tfPrihlasMeno.Text, pfHeslo.Text);
                if (("noconnection").Equals(result))
                {
                    this.Close();
                    MessageBox.Show("Server is unavailable. Check your internet connection.");
                }
                else if (("false").Equals(result))
                {
                    pfHeslo.Text = "";
                    MessageBox.Show("Incorrect username or password.");
                }
                else if (("error").Equals(result))
                {
                    this.Close();
                    MessageBox.Show("Unexpected error has occured.");                   
                }
                else
                {
                    this.Close();
                    MessageBox.Show("You have been logged in successfully. You do not need to log in after next opening of Enterprise Architect.");
                }
            }
            else
            {
                pfHeslo.Text = "";
                MessageBox.Show("Fill in username and password.");
            }
        }
    }
}
