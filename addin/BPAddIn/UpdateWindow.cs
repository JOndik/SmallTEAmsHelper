using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddInTry
{
    public partial class UpdateWindow : Form
    {

        public static WebClient webClient = new WebClient();

        public UpdateWindow()
        {
            InitializeComponent();
        }

        public void startDownload()
        {
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

            string addInPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string zipPath = @"" + Path.GetTempPath() + "sthAddIn-update.zip";
            string extractPath = @"" + Path.GetTempPath() + "sthAddIn";

            webClient.DownloadFileAsync(new Uri("https://ichiban.fiit.stuba.sk:8443/update/currentVersion"), zipPath);

            if (!(Directory.Exists(extractPath)))
            {
                Directory.CreateDirectory(extractPath);
            }
            else
            {
                Directory.Delete(extractPath, true);
                Directory.CreateDirectory(extractPath);
            }

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                }
            }

            string arguments = string.Format("\"{0}\"", addInPath);
            //Process proc = new Process();
            /*proc.StartInfo.WorkingDirectory = string.Format(extractPath);
            proc.StartInfo.FileName = "update.bat";
            proc.StartInfo.CreateNoWindow = false;
            //proc.StartInfo.Arguments = string.Format("\"{0}\"\\", addInPath);
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";*/
            Process.Start(string.Format(extractPath) + "\\update.bat", arguments);

            Process.GetProcessesByName("EA")[0].CloseMainWindow();
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Close();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            label1.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (webClient != null)
            {
                webClient.CancelAsync();
            }
        }

        private void UpdateWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.button1_Click(sender, e);
        }
    }
}
