using BPAddIn.DataContract;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class ChangeService
    {
        //private const string serviceAddress = "http://localhost:8080";
        private const string serviceAddress = "http://147.175.180.200:8080";
        //private const string serviceAddress = "https://ichiban.fiit.stuba.sk:8443";
        public static string userToken = "";

        public void saveChange(ModelChange change)
        {
            try
            {
                lock (LocalDBContext.Lock)
                {
                    using (LocalDBContext context = new LocalDBContext())
                    {
                        context.modelChanges.Add(change);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void startActivityDispatcher()
        {

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    List<User> users = context.user.ToList();
                    if (users.Count > 0)
                    {
                        User user = users.First();
                        if (user != null)
                        {
                            userToken = user.token;
                        }
                        else
                        {
                            userToken = "123456";
                        }
                    }
                }
            }

            int sleepTime = 5 * 60 * 1000; // 5 min

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(serviceAddress))
                    {
                        stream.Close();
                        this.uploadChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Thread.Sleep(sleepTime);
            }
        }

        public void uploadChanges()
        {
            List<ModelChange> modelChanges;

            try {
                lock (LocalDBContext.Lock)
                {
                    using (LocalDBContext context = new LocalDBContext())
                    {
                        modelChanges = context.modelChanges.ToList();
                        modelChanges.AddRange(context.Set<PropertyChange>().ToList());
                        modelChanges.AddRange(context.Set<ItemCreation>().ToList());
                        modelChanges.AddRange(context.Set<ScenarioChange>().ToList());
                        modelChanges.AddRange(context.Set<StepChange>().ToList());
                    }
                }

                if (modelChanges.Count == 0)
                {
                    Thread.Sleep(15 * 1000);
                    uploadChanges();
                }

                using (WebClient webClient = new WebClient())
                {
                    string result = "";
                    string data = "";
                    DTOWrapper dtoWrapper = new DTOWrapper();
                    dtoWrapper.userToken = userToken;

                    foreach (ModelChange change in modelChanges)
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                        dtoWrapper.modelChange = change;
                        //data = Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(dtoWrapper.serialize()));
                        data = EncodeNonAsciiCharacters(dtoWrapper.serialize());
                        result = webClient.UploadString(serviceAddress + "/changes", data);

                        if (result == "")
                        {
                            lock (LocalDBContext.Lock)
                            {
                                using (LocalDBContext context = new LocalDBContext())
                                {
                                    context.Remove(change);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }

                uploadChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string EncodeNonAsciiCharacters(string value)
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
