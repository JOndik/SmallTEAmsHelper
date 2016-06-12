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
        public void isConnectedInternet()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();

                        User user = getLoggedUser();
                        if (user == null)
                        {
                            MessageBox.Show("First, you must log in and create team.");
                            return;
                        }

                        string result = "";
                        string data = "";                      
                        
                        try
                        {
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                            data = user.token;
                            result = webClient.UploadString(Utils.serviceAddress + "/auth/checkJoining", data);
                            DialogResult dialogResult = MessageBox.Show("Do you want to delete your current team?", "Deletion of your team", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                result = webClient.UploadString(Utils.serviceAddress + "/delete", data);
                                MessageBox.Show("Deletion of your team has been successful.");
                            }
                        }
                        catch (WebException ex)
                        {
                            var response = ex.Response as HttpWebResponse;
                            int code = (int)response.StatusCode;
                            if (code == 401)
                            {
                                MessageBox.Show("Please, log in once again.");
                            }
                            else if (code == 404)
                            {
                                MessageBox.Show("Currently, you are not member of any team.");
                            }
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("Unexpected error has occured.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is unavailable. Check your internet connection.");
            }
        }

        public User getLoggedUser()
        {
            List<User> users;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    users = context.user.ToList();
                    User user = null;

                    if (users.Count > 0)
                    {
                        user = users.First();
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        public void isConnected(TeamPairDTO teamPair, JoinWindow joinWindow)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        this.uploadTeamMemberUsername(teamPair, joinWindow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is unavailable. Check your internet connection.");
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
                    result = webClient.UploadString(Utils.serviceAddress + "/auth/pair", data);
                    joinWindow.closeWindow();
                    MessageBox.Show("Confirmation email has been sent to email address of your colleague.");                   
                }
                catch(WebException ex)
                {
                    var response = ex.Response as HttpWebResponse;
                    int code = (int)response.StatusCode;
                    if (code == 401) 
                    {
                        joinWindow.closeWindow();
                        MessageBox.Show("Please, log in once again.");
                    }
                    else if (code == 403)
                    {
                        MessageBox.Show("Self-addition to team is forbidden.");
                    }
                    else if (code == 405)
                    {
                        joinWindow.closeWindow();
                        MessageBox.Show("Your colleague has already added you to team. You must click on link in the email in your email address.");
                    }
                    else if (code == 400)
                    {
                        MessageBox.Show("Your colleague must log in or you have filled incorrect username.");
                    }
                    else if (code == 409)
                    {
                        MessageBox.Show("Filled colleague is already in your team.");
                    }
                    else if (code == 406)
                    {
                        joinWindow.closeWindow();
                        MessageBox.Show("Team can have maximally 2 members.");
                    }
                } 
                catch (Exception ex2)
                {
                    MessageBox.Show("Unexpected error has occured.");
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
