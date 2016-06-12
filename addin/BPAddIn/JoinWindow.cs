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
                MessageBox.Show("Unexpected error in database has occured");
            }
            
            if (("false").Equals(token))
            {
                MessageBox.Show("First you must log in.");
                this.Close();
            }
            else
            {
                if (("").Equals(tfSecondMember.Text))
                {
                    MessageBox.Show("Fill in username of your team colleague.");                   
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
