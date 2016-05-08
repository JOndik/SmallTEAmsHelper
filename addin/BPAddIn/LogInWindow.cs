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
                    MessageBox.Show("Server je nedostupný. Skontrolujte si internetového pripojenie.");
                }
                else if (("false").Equals(result))
                {
                    pfHeslo.Text = "";
                    MessageBox.Show("Zadali ste nesprávne údaje.");
                }
                else if (("error").Equals(result))
                {
                    this.Close();
                    MessageBox.Show("Nastala chyba na serveri.");                   
                }
                else
                {
                    this.Close();
                    MessageBox.Show("Boli ste úspešne prihlásený. Pri ďalšom zapnutí EA sa už nemusíte prihlasovať.");
                }
            }
            else
            {
                pfHeslo.Text = "";
                MessageBox.Show("Vyplňte oba údaje.");
            }
        }
    }
}
