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
        private const string serviceAddress = "http://localhost:8080";

        public void saveChange(ModelChange change)
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

        public void startActivityDispatcher()
        {
            int sleepTime = 5 * 60 * 1000; // 5 min

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(serviceAddress))
                    {
                        this.uploadChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("error");
                Thread.Sleep(sleepTime);
            }
        }

        public void uploadChanges()
        {
            List<ModelChange> modelChanges;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    modelChanges = context.modelChanges.ToList();
                    modelChanges.AddRange(context.Set<PropertyChange>().ToList());
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
