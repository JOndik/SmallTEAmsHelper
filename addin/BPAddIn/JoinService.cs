using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class JoinService
    {
        //private const string serviceAddress = "http://192.168.137.89:8080";
        //private const string serviceAddress = "http://147.175.180.200:8080";
        private const string serviceAddress = "https://ichiban.fiit.stuba.sk:8443";

        public void isConnected(TeamPairDTO teamPair, JoinWindow joinWindow)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(serviceAddress))
                    {
                        stream.Close();
                        this.uploadTeamMemberUsername(teamPair, joinWindow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba internetového pripojenia.");
            }
        }

        public void uploadTeamMemberUsername(TeamPairDTO teamPair, JoinWindow joinWindow)
        {
            using (WebClient webClient = new WebClient())
            {
                string result = "";
                string data = "";

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                data = ChangeService.EncodeNonAsciiCharacters(teamPair.serialize());
                try
                {
                    result = webClient.UploadString(serviceAddress + "/auth/pair", data);
                    MessageBox.Show("result");
                    joinWindow.Close();
                }
                catch(WebException ex)
                {
                    //MessageBox.Show("result2");
                    var response = ex.Response as HttpWebResponse;
                    int code = (int)response.StatusCode;
                    if (code == 401) {
                        MessageBox.Show("Prihláste sa ešte raz.");
                    }
                    else if (code == 400)
                    {
                        MessageBox.Show("Váš kolega sa musí prihlásiť do EA.");
                    }
                } 
                catch (Exception ex2)
                {

                }
            }
        }

        public string findToken()
        {
            List<User> users;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    users = context.user.ToList();
                    User user;

                    if (users.Count > 0)
                    {
                        user = users.First();
                        return user.token;
                    }
                    else
                    {
                        return "false";
                    }
                }
            }
        }
    }
}
