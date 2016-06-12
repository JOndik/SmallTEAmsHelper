using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class RegistrationService
    {
        public string checkConnection(string name, string password)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        return this.uploadRegistrationData(name, password);
                    }
                }
            }
            catch (Exception ex)
            {
                return "noconnection";
            }
        }

        public string uploadRegistrationData(string name, string password)
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
                    result = webClient.UploadString(Utils.serviceAddress + "/auth/register", data);
                    return result;
                }
                catch (WebException ex)
                {
                    var response = ex.Response as HttpWebResponse;
                    int code = (int)response.StatusCode;
                    if (code == 400)
                    {
                        return "wrongName";
                    }
                    else if (code == 401)
                    {
                        return "notFilled";
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
