using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class SynchronizationService
    {
        private User user;
        private const string serviceAddress = "http://147.175.180.200:8080";
        //private const string serviceAddress = "http://192.168.1.138:8080";

        private SynchronizationAdditions synchronizationAdditions;
        private Synchronization synchronization;
        private ModelTraverse modelTraverse;
        public bool changesAllowed { get; set; }

        public SynchronizationService(EA.Repository repository)
        {
            this.modelTraverse = new ModelTraverse(repository);
            this.synchronization = new Synchronization(repository);
            this.changesAllowed = true;
        }

        public void checkConnectionForSynchronization(EA.Repository repository)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(serviceAddress))
                    {
                        stream.Close();
                        this.checkAuthentification(repository);
                    }
                }          
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
               // MessageBox.Show("Pre synchronizáciu je potrebné internetové pripojenie.");
            }
        }

        public void checkAuthentification(EA.Repository repository)
        {
            User user = getLoggedUser();
            if (user == null)
            {
                MessageBox.Show("treba sa prihlasit a spojit sa");
            }
            else
            {
                using (WebClient webClient = new WebClient())
                {
                    this.user = user;
                    string result = "", result2 = "", result3 = "";
                    ModelInformation synchronizationData = new ModelInformation();
                    synchronizationData.token = user.token;
                    synchronizationData.modelGUID = repository.GetPackageByID(1).PackageGUID;
                    string data = user.token;

                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    data = EncodeNonAsciiCharacters(synchronizationData.serialize());
                    result = webClient.UploadString(serviceAddress + "/synchronization", data);

                    if (result == "true")               //v time a uz prebieha sync
                    {
                        this.changesAllowed = false;
                        while (true)
                        {
                            data = user.token;
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                            result2 = webClient.UploadString(serviceAddress + "/synchronization/changes", data);

                            ModelChange modelChange = JsonConvert.DeserializeObject<ModelChange>(result2, new JsonItemConverter());
                            if (modelChange == null)
                            {
                                MessageBox.Show("null");
                                data = user.token;
                                /*webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                result3 = webClient.UploadString(serviceAddress + "/synchronization/wrongSynchronization", data);*/
                                continue;
                            }
                            if (modelChange is PropertyChange)
                            {
                                PropertyChange propertyChange = (PropertyChange)modelChange;
                                if (propertyChange.timestamp == "-1")
                                {
                                    MessageBox.Show("koniec");
                                    this.changesAllowed = true;
                                    repository.RefreshModelView(1);
                                    break;
                                }
                            }

                            MessageBox.Show(modelChange.userName + " " + modelChange.itemGUID + " " + modelChange.elementType + " " + modelChange.classType);

                            if (modelChange is ItemCreation)
                            {
                                ItemCreation itemCreation = (ItemCreation)modelChange;                                  //vytvorenie
                                MessageBox.Show(itemCreation.name + " " + itemCreation.packageGUID);

                                NewCorrespondenceNode newCorrNode = new NewCorrespondenceNode();
                                newCorrNode.firstUsername = user.username;
                                newCorrNode.firstItemGUID = synchronization.handleSynchronizationAdditions(itemCreation, repository);
                                newCorrNode.secondUsername = itemCreation.userName;
                                newCorrNode.secondItemGUID = itemCreation.itemGUID;
                                
                                data = EncodeNonAsciiCharacters(newCorrNode.serialize());
                                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                result3 = webClient.UploadString(serviceAddress + "/synchronization/createNode", data);
                            }
                            else if (modelChange is PropertyChange)
                            {
                                PropertyChange propertyChange = (PropertyChange)modelChange;
                                if (propertyChange.elementDeleted == 0)
                                {
                                        /*if (propertyChange.propertyType == 303 || propertyChange.propertyType == 304)        //presun
                                        {
                                            synchronization.handleSynchronizationMovements(propertyChange, repository);
                                        }
                                        else
                                        {*/
                                            synchronization.handleSynchronizationChanges(propertyChange, repository);       //zmena
                                       // }
                                }
                                else
                                {
                                    synchronization.handleSynchronizationDeletions(propertyChange, repository);             //odstranenie
                                }
                            }
                            else if (modelChange is ScenarioChange)                     //scenare
                            {
                                ScenarioChange scenarioChange = (ScenarioChange)modelChange;
                                MessageBox.Show("scenar " + scenarioChange.name + " " + scenarioChange.itemGUID);

                                if (scenarioChange.status == 1)                            //pridanie
                                {
                                    NewCorrespondenceNode newCorrNode = new NewCorrespondenceNode();
                                    newCorrNode.firstUsername = user.username;
                                    newCorrNode.firstItemGUID = synchronization.handleScenarioAddition(scenarioChange, repository);
                                    newCorrNode.secondUsername = scenarioChange.userName;
                                    newCorrNode.secondItemGUID = scenarioChange.scenarioGUID;
                                    data = EncodeNonAsciiCharacters(newCorrNode.serialize());
                                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                    result3 = webClient.UploadString(serviceAddress + "/synchronization/createNode", data);
                                }
                                else if (scenarioChange.status == 2 || scenarioChange.status == 0)         //zmena alebo odstranenie
                                {
                                    synchronization.handleScenarioChange(scenarioChange, repository);
                                }
                            }
                            else if (modelChange is StepChange)
                            {
                                StepChange stepChange = (StepChange)modelChange;
                                MessageBox.Show("krok v scenari " + stepChange.name + " " + stepChange.itemGUID);

                                if (stepChange.status == 1)
                                {
                                    NewCorrespondenceNode newCorrNode = new NewCorrespondenceNode();
                                    newCorrNode.firstUsername = user.username;
                                    newCorrNode.firstItemGUID = synchronization.handleScenarioStepAddition(stepChange, repository);
                                    newCorrNode.secondUsername = stepChange.userName;
                                    newCorrNode.secondItemGUID = stepChange.stepGUID;
                                    data = EncodeNonAsciiCharacters(newCorrNode.serialize());
                                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                                    result3 = webClient.UploadString(serviceAddress + "/synchronization/createNode", data);
                                }
                                else if (stepChange.status == 2 || stepChange.status == 0)
                                {
                                    
                                }
                            }
                        }                                                                  
                    }
                    else if (result == "false")         //v time ale neprebieha sync
                    {
                        MessageBox.Show(result);        //budete sync prave tento model?
                        changeModelGUID(repository);
                        modelTraverse.sendDataAboutModel(repository);

                        PropertyChange propertyChange = new PropertyChange();
                        propertyChange.modelGUID = repository.GetPackageByID(1).PackageGUID;
                        propertyChange.elementType = 777;
                        modelTraverse.saveCreate(propertyChange, repository);
                    }
                    else if (result == "noSync")            //v time ale nie vsetci poslali data o modeli
                    {
                        MessageBox.Show("poslal si uz, treba pockat na ostatnych, kym kliknu na Sync");
                    }
                    else if (result == "wrong")
                    {
                        MessageBox.Show("synchronizacia z tohto modelu nie je mozna");
                    }
                    else
                    {
                        MessageBox.Show("treba sa spojit");     //nie je v time
                    }
                }
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
