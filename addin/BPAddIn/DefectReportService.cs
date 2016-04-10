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
    public class DefectReportService
    {
        public static string userToken = "";

        public void saveReport(DefectReport report)
        {
            try
            {
                lock (LocalDBContext.Lock)
                {
                    using (LocalDBContext context = new LocalDBContext())
                    {
                        var oldReport = from r in context.defectReports
                                     where r.ruleGUID == report.ruleGUID
                                     select r;

                        DefectReport old = oldReport.FirstOrDefault();

                        if (old != null)
                        {
                            old.actionsBeforeCorrection = report.actionsBeforeCorrection;
                            context.SaveChanges();
                        }
                        else
                        {
                            context.defectReports.Add(report);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
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
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        this.uploadReports();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Thread.Sleep(sleepTime);
            }
        }

        public void uploadReports()
        {
            List<DefectReport> defectReports;

            try
            {
                lock (LocalDBContext.Lock)
                {
                    using (LocalDBContext context = new LocalDBContext())
                    {
                        defectReports = context.defectReports.ToList();
                    }
                }

                if (defectReports.Count == 0)
                {
                    Thread.Sleep(15 * 1000);
                    uploadReports();
                }

                using (WebClient webClient = new WebClient())
                {
                    string result = "";
                    string data = "";

                    foreach (DefectReport defectReport in defectReports)
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                        defectReport.userToken = userToken;
                        //data = Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(dtoWrapper.serialize()));
                        data = EncodeNonAsciiCharacters(defectReport.serialize());
                        result = webClient.UploadString(Utils.serviceAddress + "/defectReports", data);

                        if (result == "")
                        {
                            lock (LocalDBContext.Lock)
                            {
                                using (LocalDBContext context = new LocalDBContext())
                                {
                                    context.Remove(defectReport);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }

                uploadReports();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
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
