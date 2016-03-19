using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BPAddIn.DataContract;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class SynchronizationService
    {
        private User user;
        private const string serviceAddress = "http://147.175.180.200:8080";
        private ChangeService changeService;

        public SynchronizationService()
        {
            this.changeService = new ChangeService();
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

                //MessageBox.Show(e.ToString());
                MessageBox.Show("Pre synchronizáciu je potrebné internetové pripojenie.");
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
                    string result = "";
                    string data = user.token;   

                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    result = webClient.UploadString(serviceAddress + "/synchronization", data);

                    if (result == "true")               //v time a uz prebieha sync
                    {
                        MessageBox.Show("tu pojde kod pre vykonanie sync");
                    }
                    else if (result == "false")         //v time ale neprebieha sync
                    {
                        MessageBox.Show(result);        //budete sync prave tento model?
                        changeModelGUID(repository);
                        sendDataAboutModel(repository);
                        ItemCreation itemCreation = new ItemCreation();
                        itemCreation.modelGUID = repository.GetPackageByID(1).PackageGUID;
                        itemCreation.elementType = 777;
                        saveCreate(itemCreation);
                    }
                    else if (result == "noSync")            //v time ale nie vsetci poslali data o modeli
                    {
                        MessageBox.Show("poslal si uz, treba pockat na ostatnych, kym kliknu na Sync");
                    }         
                    else
                    {
                        MessageBox.Show("treba sa spojit");     //nie je v time
                    }
                }
            }
        }

        public void sendDataAboutModel(EA.Repository Repository)
        {
            for (short i = 0; i < Repository.Models.Count; i++)
            {
                EA.Package model = (EA.Package)Repository.Models.GetAt(i);

                if (model.Packages.Count > 0)
                {
                    traversePackages(Repository, model.Packages);
                }
            }
        }

        public void traversePackages(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = repository.GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = package.PackageGUID;
                itemCreation.elementType = 800;
                itemCreation.author = package.Element.Author;
                itemCreation.name = package.Name;
                itemCreation.parentGUID = "0";
                EA.Package parentPackage = repository.GetPackageByID(package.ParentID);
                if (parentPackage != null)
                {
                    itemCreation.packageGUID = parentPackage.PackageGUID;
                }
                saveCreate(itemCreation);
                if (package.Packages.Count > 0)
                {
                    traversePackages(repository, package.Packages);
                }
            }
        }


        public void saveCreate(ModelChange change)
        {
            change.timestamp = "0";
            changeService.saveChange(change);
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
