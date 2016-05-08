using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net;
using System.Text;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Threading.Tasks;
using System.Windows.Forms;
using BPAddIn;

namespace BPAddIn.SynchronizationPackage
{
    public class SynchronizationService
    {
        private User user;
        private static System.Timers.Timer timer;
        private ModelInformation modelInformation;
        private SendingDataWindow sendingDataWindow;

        private Synchronization synchronization;
        private ModelTraverse modelTraverse;

        public SynchronizationService(EA.Repository repository)
        {
            this.modelTraverse = new ModelTraverse(repository);
            this.synchronization = new Synchronization(repository);
        }

        public void checkConnectionForSynchronization(EA.Repository repository)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        this.executeSynchronization(repository);
                    }
                }
            }
            catch (Exception e)
            {
                BPAddIn.changesAllowed = true;
                MessageBox.Show(e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
                MessageBox.Show("Server je nedostupný. Skontrolujte si internetové pripojenie.");
            }
        }

        public void executeSynchronization(EA.Repository repository)
        {
            User user = getLoggedUser();
            using (WebClient webClient = new WebClient())
            {
                this.user = user;
                string result = "", result2 = "", result3 = "";
                ModelInformation synchronizationData = new ModelInformation();
                synchronizationData.token = user.token;
                EA.Package package = (EA.Package)repository.GetPackageByID(1);
                synchronizationData.modelGUID = package.PackageGUID;
                string data = user.token;

                BPAddIn.changesAllowed = false;
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                result = webClient.UploadString(Utils.serviceAddress + "/synchronization/getNumber", user.token);
                int number = Convert.ToInt32(result);
                if (BPAddIn.synchronizationWindow == null)
                {
                    BPAddIn.synchronizationWindow = repository.AddWindow("Synchronizácia", "BPAddIn.SynchronizationPackage.SynchronizationWindow") as SynchronizationWindow;
                }
                repository.ShowAddinWindow("Synchronizácia");
                BPAddIn.synchronizationWindow.removeList();

                DateTime localDate = DateTime.Now;
                var culture = new CultureInfo("de-DE");
                BPAddIn.synchronizationWindow.addToList(localDate.ToString(culture));


                if (number == 0)
                {
                    BPAddIn.synchronizationWindow.addToList("Žiadne zmeny.");
                    BPAddIn.changesAllowed = true;
                    return;
                }

                while (true)
                {
                    try
                    {
                        data = user.token;
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                        result2 = webClient.UploadString(Utils.serviceAddress + "/synchronization/changes", data);

                        ModelChange modelChange = JsonConvert.DeserializeObject<ModelChange>(result2, new JsonItemConverter());
                        if (modelChange == null)
                        {
                            data = user.token;
                            continue;
                        }
                        if (modelChange is PropertyChange)
                        {
                            PropertyChange propertyChange = (PropertyChange)modelChange;
                            if (propertyChange.timestamp == "-1")
                            {
                                repository.ShowAddinWindow("Synchronizácia");
                                repository.RefreshModelView(1);
                                break;
                            }
                        }

                        if (modelChange is ItemCreation)
                        {
                            ItemCreation itemCreation = (ItemCreation)modelChange;                                  //vytvorenie

                            NewCorrespondenceNode newCorrNode = new NewCorrespondenceNode();
                            newCorrNode.firstUsername = user.username;
                            newCorrNode.firstItemGUID = synchronization.handleSynchronizationAdditions(itemCreation, repository);
                            newCorrNode.secondUsername = itemCreation.userName;
                            newCorrNode.secondItemGUID = itemCreation.itemGUID;

                            data = EncodeNonAsciiCharacters(newCorrNode.serialize());
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                            result3 = webClient.UploadString(Utils.serviceAddress + "/corrModel/createNode", data);
                        }
                        else if (modelChange is PropertyChange)
                        {
                            PropertyChange propertyChange = (PropertyChange)modelChange;
                            if (propertyChange.elementDeleted == 0)
                            {
                                synchronization.handleSynchronizationChanges(propertyChange, repository);       //zmena
                            }
                            else
                            {
                                synchronization.handleSynchronizationDeletions(propertyChange, repository);             //odstranenie
                            }
                        }
                        else if (modelChange is StepChange)                    //scenare
                        {
                            StepChange scenarioChange = (StepChange)modelChange;

                            if (scenarioChange.status == 1)                            //pridanie
                            {
                                NewCorrespondenceNode newCorrNode = new NewCorrespondenceNode();
                                newCorrNode.firstUsername = user.username;
                                newCorrNode.firstItemGUID = synchronization.handleScenarioAddition(scenarioChange, repository);
                                newCorrNode.secondUsername = scenarioChange.userName;
                                newCorrNode.secondItemGUID = scenarioChange.scenarioGUID;
                                data = EncodeNonAsciiCharacters(newCorrNode.serialize());
                                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                result3 = webClient.UploadString(Utils.serviceAddress + "/corrModel/createNode", data);
                            }
                            else if (scenarioChange.status == 2 || scenarioChange.status == 0)         //zmena alebo odstranenie
                            {
                                synchronization.handleScenarioChange(scenarioChange, repository);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }               
            }
        }

        public void checkInternetConnection(EA.Repository repository)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        this.showCorrectWindow(repository);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Server je nedostupný. Skontrolujte si internetové pripojenie.");
            }
        }

        public void showCorrectWindow(EA.Repository repository)
        {
            User user = getLoggedUser();
            if (user == null)
            {
                MessageBox.Show("Najprv sa musíte prihlásiť a spojiť s kolegom v tíme.");
                return;
            }
            using (WebClient webClient = new WebClient())
            {
                this.user = user;
                string result = "", result2 = "", result3 = "";
                ModelInformation synchronizationData = new ModelInformation();
                synchronizationData.token = user.token;
                EA.Package package = (EA.Package)repository.GetPackageByID(1);
                synchronizationData.modelGUID = package.PackageGUID;
                string data = user.token;

                try
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    data = EncodeNonAsciiCharacters(synchronizationData.serialize());
                    result = webClient.UploadString(Utils.serviceAddress + "/synchronization", data);
                    BPAddIn.syncProgressWindow = new SynchronizationProgressWindow(repository);
                    BPAddIn.syncProgressWindow.ShowDialog();
                }
                catch (WebException e)
                {
                    var response = e.Response as HttpWebResponse;
                    int code = (int)response.StatusCode;
                    if (code == 401)
                    {
                        MessageBox.Show("Prihláste sa ešte raz.");
                    }
                    else if (code == 400)
                    {
                        MessageBox.Show("Aktuálne nie ste v žiadnom tíme. Vyberte možnosť Pridanie kolegu do tímu.");
                    }
                    else if (code == 405)
                    {
                        MessageBox.Show("Synchronizácia z tohto modelu nie je možná.");
                    }
                    else if (code == 403)
                    {
                        MessageBox.Show("Pre synchronizáciu je potrebné, aby váš kolega poslal dáta o modeli.");
                    }
                    else if (code == 404)
                    {
                        DialogResult resultWindow = MessageBox.Show("Chcete synchronizovať práve otvorený model? Zmena modelu nebude možná.", "Výber modelu pre synchronizáciu", MessageBoxButtons.YesNo);
                        if (resultWindow == DialogResult.Yes)
                        {
                            SendingDataWindow sendingDataWindow = new SendingDataWindow(repository);
                            sendingDataWindow.ShowDialog();
                        }
                    }
                    else if (code == 406)
                    {
                        MessageBox.Show("Pre synchronizáciu je potrebné, aby sa váš kolega pridal do tímu.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nastala chyba.");
                }
            }
        }

        public void sendDataAboutModel(SendingDataWindow sendingDataWindow, EA.Repository repository)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();
                        this.sendingDataWindow = sendingDataWindow;
                        ChangeService.newEvent.Set();
                        changeModelGUID(repository);
                        modelTraverse.sendDataAboutModel(repository);

                        ScenarioChange propertyChange = new ScenarioChange();
                        propertyChange.modelGUID = repository.GetPackageByID(1).PackageGUID;
                        propertyChange.elementType = 777;
                        modelTraverse.saveCreate(propertyChange, repository);

                        user = getLoggedUser();
                        modelInformation = new ModelInformation();
                        modelInformation.modelGUID = repository.GetPackageByID(1).PackageGUID;
                        modelInformation.token = user.token;

                        timer = new System.Timers.Timer();
                        timer.Elapsed += new System.Timers.ElapsedEventHandler(checkLastCreate);
                        timer.Interval = 5000;
                        timer.Start();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Server je nedostupný. Skontrolujte si internetové pripojenie.");
            }           
        }

        private void checkLastCreate(Object source, System.Timers.ElapsedEventArgs el)
        {
            bool confirmed = false;
            string data, result;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();

                        try
                        {
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                            data = EncodeNonAsciiCharacters(modelInformation.serialize());
                            result = webClient.UploadString(Utils.serviceAddress + "/changes/lastCreate", data);
                            confirmed = true;
                        }
                        catch (WebException e)
                        {
                            var response = e.Response as HttpWebResponse;
                            int code = (int)response.StatusCode;
                            if (code == 401)
                            {
                                MessageBox.Show("Prihláste sa ešte raz.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                timer.Close();
                sendingDataWindow.showMessage();
            }

            if (confirmed)
            {
                timer.Close();
                sendingDataWindow.setVisible(true);
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

        public void changeModelGUID(EA.Repository Repository)
        {
            String guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            for (short i = 0; i < Repository.Models.Count; i++)
            {
                EA.Package pack = (EA.Package)Repository.Models.GetAt(i);
                pack.PackageGUID = guid;
                pack.Update();
                Repository.Models.Refresh();
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
