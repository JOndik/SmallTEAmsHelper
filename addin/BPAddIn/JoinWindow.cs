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
    public partial class JoinWindow : Form
    {
        private JoinService joinService = new JoinService();

        public JoinWindow()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = btnConfirmNames;
        }

        private void clickOnBtnConfirm(object sender, EventArgs e)
        {
            string token = "";
            try
            {
                token = joinService.findToken();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba v databáze.");
            }
            
            if (("false").Equals(token))
            {
                MessageBox.Show("Musíte sa najprv prihlásiť.");
                this.Close();
            }
            else
            {
                if (("").Equals(tfSecondMember.Text))
                {
                    MessageBox.Show("Vyplňte prihlasovacie meno kolegu.");                   
                }
                else
                {
                    TeamPairDTO teamPair = new TeamPairDTO();
                    teamPair.teamMemberName = tfSecondMember.Text;
                    teamPair.token = token;
                    joinService.isConnected(teamPair, this);
                }
            }            
        }

        public void closeWindow()
        {
            this.Close();
        }
    }
}
