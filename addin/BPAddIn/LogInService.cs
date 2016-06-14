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
        /// <summary>
        /// method checks internet connection before uploading log in data
        /// </summary>
        /// <param name="name">name of user</param>
        /// <param name="password">password of user</param>
        /// <returns></returns>
        public string checkConnection(string name, string password)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {                   
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        return this.uploadLogInData(name, password);
                    }
                }
            }
            catch (Exception ex)
            {
                return "noconnection";
            }           
        }

        /// <summary>
        /// method uploads data about user gained in log in process
        /// </summary>
        /// <param name="name">name of user</param>
        /// <param name="password">password of user</param>
        /// <returns></returns>
        public string uploadLogInData(string name, string password)
        {
            LogIn logIn = new LogIn(name, password);

            using (WebClient webClient = new WebClient())
            {
                string result = "";
                string data = "";

                try
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    data = EncodeNonAsciiCharacters(logIn.serialize());
                    result = webClient.UploadString(Utils.serviceAddress + "/auth", data);
                    saveUserToLocalDatabase(logIn.name, result);
                    return result;
                }
                catch (WebException ex)
                {
                    var response = ex.Response as HttpWebResponse;
                    int code = (int)response.StatusCode;
                    if (code == 401)
                    {
                        return "false";
                    }
                    else
                    {
                        return "error";
                    }
                }
                catch (Exception ex2)
                {
                    return "error";
                }
            }
        }

        /// <summary>
        /// method adds new user to local database
        /// </summary>
        /// <param name="name">name of new user</param>
        /// <param name="token">token of new user</param>
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

        /// <summary>
        /// method encodes non ascii characters of param
        /// </summary>
        /// <param name="value">string that should be encoded</param>
        /// <returns>string with encoded non ascii characters</returns>
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
