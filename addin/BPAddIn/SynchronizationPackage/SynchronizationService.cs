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
                MessageBox.Show("Server is unavailable. Check your internet connection.");
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
                    BPAddIn.synchronizationWindow = repository.AddWindow("Synchronization", "BPAddIn.SynchronizationPackage.SynchronizationWindow") as SynchronizationWindow;
                }
                repository.ShowAddinWindow("Synchronization");
                BPAddIn.synchronizationWindow.removeList();

                DateTime localDate = DateTime.Now;
                var culture = new CultureInfo("de-DE");
                BPAddIn.synchronizationWindow.addToList(localDate.ToString(culture));


                if (number == 0)
                {
                    BPAddIn.synchronizationWindow.addToList("No modifications.");
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
                                repository.ShowAddinWindow("Synchronization");
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
                MessageBox.Show("Server is unavailable. Check your internet connection.");
            }
        }

        public void showCorrectWindow(EA.Repository repository)
        {
            User user = getLoggedUser();
            if (user == null)
            {
                MessageBox.Show("First you must log in and add your colleague to team.");
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
                        MessageBox.Show("Please, log in once again.");
                    }
                    else if (code == 400)
                    {
                        MessageBox.Show("Currently, you are not a member of any team. First, add your colleague to team.");
                    }
                    else if (code == 405)
                    {
                        MessageBox.Show("Synchronization of this model in your team is not allowed.");
                    }
                    else if (code == 403)
                    {
                        MessageBox.Show("Your colleague in team has not sent data about model yet.");
                    }
                    else if (code == 404)
                    {
                        DialogResult resultWindow = MessageBox.Show("Do you want to synchronise currently opened model? Change of model will not be possible.", "Choice of model for synchronization", MessageBoxButtons.YesNo);
                        if (resultWindow == DialogResult.Yes)
                        {
                            SendingDataWindow sendingDataWindow = new SendingDataWindow(repository);
                            sendingDataWindow.ShowDialog();
                        }
                    }
                    else if (code == 406)
                    {
                        MessageBox.Show("Your colleague has not been added to your team yet.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error has occured.");
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
                MessageBox.Show("Server is unavailable. Check your internet connection.");
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
                                MessageBox.Show("Please, log in once again.");
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

        public void startNewProject()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(Utils.serviceAddress))
                    {
                        stream.Close();

                        User user = getLoggedUser();
                        if (user == null)
                        {
                            MessageBox.Show("First, you must log in and create team.");
                            return;
                        }

                        string result = "";
                        string data = "";

                        try
                        {
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                            data = user.token;
                            result = webClient.UploadString(Utils.serviceAddress + "/auth/checkProject", data);
                            DialogResult dialogResult = MessageBox.Show("Do you want to start new project?", "Start new project", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                result = webClient.UploadString(Utils.serviceAddress + "/delete/start", data);
                                MessageBox.Show("Now you can start new project.");
                            }
                        }
                        catch (WebException ex)
                        {
                            var response = ex.Response as HttpWebResponse;
                            int code = (int)response.StatusCode;
                            if (code == 401)
                            {
                                MessageBox.Show("Please, log in once again.");
                            }
                            else if (code == 404)
                            {
                                MessageBox.Show("Currently, you are not member of any team.");
                            }
                            else if (code == 403)
                            {
                                MessageBox.Show("Currently, you do not participate in any project of your team.");
                            }
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("Unexpected error has occured.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is unavailable. Check your internet connection.");
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
