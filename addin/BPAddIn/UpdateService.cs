using BPAddInTry;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    class UpdateService
    {
        private const string serviceAddress = "http://147.175.180.200:8080";
        //private const string serviceAddress = "http://147.175.180.200:8080";
        //private const string serviceAddress = "https://ichiban.fiit.stuba.sk:8443";

        public void isConnected()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(serviceAddress))
                    {
                        this.checkUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vyskytol sa problém. Skontrolujte internetové pripojenie.");
            }
        }

        public void checkUpdate()
        {
            using (WebClient webClient = new WebClient())
            {
                string result = "";

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                result = webClient.DownloadString(serviceAddress + "/update");

                double actualVersion;
                try 
                {
                    actualVersion = double.Parse(result, System.Globalization.CultureInfo.InvariantCulture);
                 
                    if (actualVersion > findVersion())
                    {
                        if (isAdmin() == true)
                        {
                                //MessageBox.Show("admin - chcete aktualizovat? " + Path.GetTempPath() + " " + AppDomain.CurrentDomain.BaseDirectory);
                                DialogResult dialogResult = MessageBox.Show("K dispozícii je update. Chcete ho nainštalovať?", 
                                "Update rozšírenia", MessageBoxButtons.YesNo);
                                if (dialogResult == DialogResult.Yes)
                                {
                                    string addInPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                    string zipPath = @"" + Path.GetTempPath() + "sthAddIn-update.zip";
                                    string extractPath = @"" + Path.GetTempPath() + "sthAddIn";

                                    webClient.DownloadFile("https://ichiban.fiit.stuba.sk:8443/update/currentVersion", zipPath);

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

                                    /*UpdateWindow updateWindow = new UpdateWindow();
                                    updateWindow.Show();
                                    updateWindow.startDownload();*/
                                }
                                else
	                            {
	                            }                               
                        }
                        else
                        {
                            MessageBox.Show("K dispozícii je update. Pre jeho inštaláciu vypnite Enterprise Architect a spustite ho s administrátorskými právami.");
                        }                        
                    }
                    else 
                    {
                        MessageBox.Show("Používate aktuálnu verziu rozšírenia.");
                    }           
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Vyskytol sa problém. Skontrolujte internetové pripojenie.");
                    //MessageBox.Show(ex.ToString());
                }                             
            }
        }

        public void update()
        {
           
        }

        public double findVersion()
        {
            List<Version> versions;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    versions = context.version.ToList();
                    Version version = new Version();

                    if (versions.Count > 0)
                    {
                        version = versions.First();
                        return version.number;
                    }
                    else
                    {
                        version.id = 1;
                        version.number = 1.0;
                        context.version.Add(version);
                        context.SaveChanges();
                        return version.number;
                    }
                }
            }      
        }

        public static bool isAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void compareVersions()
        {
            string addInPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string number;

            try
            {
                number = File.ReadAllLines(addInPath + "\\version.txt").First();

                double currentVersion = findVersion();
                double newVersion = double.Parse(number, System.Globalization.CultureInfo.InvariantCulture);

                if (newVersion > currentVersion)
                {
                    if (currentVersion <= 1.0)
                    {
                        // update db schema
                        lock (LocalDBContext.Lock)
                        {
                            using (LocalDBContext context = new LocalDBContext())
                            {
                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `item_creations`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `item_creations` (`timestamp`	varchar(255), `itemGUID` varchar(255), `modelGUID` varchar(255), `id` INTEGER PRIMARY KEY AUTOINCREMENT,"
                                    + "`elementType` int, `parentGUID` varchar(255), `author` varchar(255),	`name` varchar(255), `packageGUID`	varchar(255), `srcGUID`	varchar(255),"
                                    + "`targetGUID`	varchar(255), `diagramGUID`	varchar(255), `coordinates`	varchar(255), `elementDeleted` int);");

                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `model_changes`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `model_changes` (`timestamp` varchar(255),	`itemGUID` varchar(255), `modelGUID` varchar(255), `id`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                                    + "`elementType` int, `elementDeleted` int);");

                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `property_changes`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `property_changes` (`timestamp` varchar(255), `itemGUID` varchar(255),	`propertyBody`	varchar(255), `propertyType` int,"
                                    + "`elementType` int, `modelGUID` varchar(255),	`oldPropertyBody` varchar(255),	`id` INTEGER PRIMARY KEY AUTOINCREMENT,	`elementDeleted` int);");

                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `scenario_changes`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `scenario_changes` (`timestamp` varchar(255), `itemGUID` varchar(255),	`modelGUID`	varchar(255),`id` INTEGER PRIMARY KEY AUTOINCREMENT,"
	                                + "`elementType` int, `name` varchar(255), `type` varchar(255),	`status` int, `scenarioGUID` varchar(255), `elementDeleted` int);");

                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `step_changes`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `step_changes` (`timestamp` varchar(255), `itemGUID` varchar(255),	`modelGUID`	varchar(255), `id` INTEGER PRIMARY KEY AUTOINCREMENT,"
	                                + "`elementType` int, `status`	int, `scenarioGUID`	varchar(255), `position` int, `stepType` varchar(255), `name` varchar(255),	`uses`	varchar(255), `results` varchar(255),"
	                                + "`state` varchar(255), `extensionGUID` varchar(255), `joiningStepGUID` varchar(255),	`joiningStepPosition` varchar(255),	`stepGUID`	varchar(255), `elementDeleted`	int);");
                            }
                        }
                    }

                    this.editVersion(newVersion);
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void editVersion(double actualVersion)
        {
            List<Version> versions;

            lock (LocalDBContext.Lock)
            {
                using (LocalDBContext context = new LocalDBContext())
                {
                    versions = context.version.ToList();
                    Version version = versions.First();
                    version.number = actualVersion;
                    context.SaveChanges();
                }
            }
        }
    }
}
