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
        /// <summary>
        /// method checks internet connection before checking for update
        /// </summary>
        public void isConnected()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        this.checkUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is unavailable. Check your internet connection.");
            }
        }

        /// <summary>
        /// method checks for update and executes update of EA extension if update is available
        /// </summary>
        public void checkUpdate()
        {
            using (WebClient webClient = new WebClient())
            {
                string result = "";

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                result = webClient.DownloadString(Utils.serviceAddress + "/update");

                double actualVersion;
                try 
                {
                    actualVersion = double.Parse(result, System.Globalization.CultureInfo.InvariantCulture);
                 
                    if (actualVersion > findVersion())
                    {
                        if (isAdmin() == true)
                        {
                                DialogResult dialogResult = MessageBox.Show("The update of extension is available. Do you want to install it?",
                                    "Update of SmallTEAmsHelper extension", MessageBoxButtons.YesNo);

                                if (dialogResult == DialogResult.Yes)
                                {
                                    string addInPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                    string zipPath = @"" + Path.GetTempPath() + "sthAddIn-update.zip";
                                    string extractPath = @"" + Path.GetTempPath() + "sthAddIn";

                                    webClient.DownloadFile(Utils.serviceAddress + "/update/currentVersion", zipPath);

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
                                    Process.Start(string.Format(extractPath) + "\\update.bat", arguments);
                                    Process.GetProcessesByName("EA")[0].CloseMainWindow();
                                }
                                else
	                            {
	                            }                               
                        }
                        else
                        {
                            MessageBox.Show("The update of extension is available. Please, close Enterprise Architect and open it with administrator rights.");
                        }                        
                    }
                    else 
                    {
                        MessageBox.Show("Currently, you use up-to-date version of extension.");
                    }           
                }
                catch (Exception ex) { }                             
            }
        }

        public void update()
        {
           
        }

        /// <summary>
        /// method finds current version of EA extension in local database
        /// </summary>
        /// <returns>double value of current version</returns>
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

        /// <summary>
        /// method checks if EA was opened in administrator rights
        /// </summary>
        /// <returns>boolean value of result</returns>
        public static bool isAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// method updates structure of local database
        /// </summary>
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

                    if (currentVersion <= 1.1)
                    {
                        // update db schema
                        lock (LocalDBContext.Lock)
                        {
                            using (LocalDBContext context = new LocalDBContext())
                            {
                                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS `defect_reports`");
                                context.Database.ExecuteSqlCommand("CREATE TABLE `defect_reports` (`timestamp`	varchar(255), `ruleName` varchar(255), `modelGUID` varchar(255), `id` INTEGER PRIMARY KEY AUTOINCREMENT,"
                                    + "`ruleGUID` varchar(255), `actionsBeforeCorrection` int, `isHidden` int);");
                            }
                        }
                    }

                    this.editVersion(newVersion);
                }
            }
            catch(Exception ex) { }
        }

        /// <summary>
        /// method changes current version of EA extension in local database
        /// </summary>
        /// <param name="actualVersion">current version</param>
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
