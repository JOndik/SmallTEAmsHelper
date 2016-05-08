using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class LogInService
    {
        public string checkConnection(string name, string password)
        {
            try
            {
                //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (WebClient webClient = new WebClient())
                {
                    /*System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };*/
                    
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        return this.uploadLogInData(name, password);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "noconnection";
            }           
        }


        public string uploadLogInData(string name, string password)
        {
            LogIn logIn = new LogIn(name, password);



            using (WebClient webClient = new WebClient())
            {
                string result = "";
                string data = "";            
               

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                //data = Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(dtoWrapper.serialize()));
                data = EncodeNonAsciiCharacters(logIn.serialize());
                result = webClient.UploadString(Utils.serviceAddress + "/auth", data);

                if (("false").Equals(result) || ("error").Equals(result))
                {                   
                    return result;
                }
                else
                {
                    saveUserToLocalDatabase(logIn.name, result);
                    return result;
                }
            }
        }

        public void saveUserToLocalDatabase(String name, String token)
        {
            List<User> users;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    users = context.user.ToList();
                    User user = new User();
                    
                    if (users.Count > 0)
                    {
                        user = users.First();
                        user.username = name;
                        user.token = token;
                        context.SaveChanges();
                    }
                    else
                    {                    
                        user.id = 1;
                        user.username = name;
                        user.token = token;
                        context.user.Add(user);
                        context.SaveChanges();
                    }

                    ChangeService.userToken = user.token;
                }
            }           
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
