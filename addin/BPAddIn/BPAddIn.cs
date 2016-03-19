using System;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TSF.UmlToolingFramework.Wrappers.EA;
using EA;
using BPAddInTry;

namespace BPAddIn
{
    public class BPAddIn : EAAddinFramework.EAAddinBase
    {
        // define menu constants
        const string menuName = "-&SmallTEAmsHelper";
        const string menuLoginWindow = "Prihlásenie";
        const string menuDbTest = "&Zobraz okno s detekciou chýb";
        const string menuClassNamesValidation = "&Validácia tried";
        const string menuJoining = "Spojenie";
        const string menuUpdate = "Aktualizácia";
        const string menuOpenProperties = "&Open Properties";
        //const string menuSynchronization = "Synchronizácia";
       
        /*const string menuPridajDiagram = "&Pridaj diagram";
        const string menuPresunDiagram = "&Presun diagram";
        const string menuZmenDiagram = "&Zmen diagram";
        const string menuZmazDiagram = "&Zmaz diagram";
        //const string menuNajdiMeno = "&Najdi meno";
        const string menuPridajElement = "&Pridaj element";
        const string menuPridajSpojenie = "&Pridaj spojenie";
        const string menuPridajBalik = "&Pridaj balik";
        const string menuZmenBalik = "&Zmen balik";
        const string menuPresunBalik = "&Presun balik";
        const string menuZmazBalik = "&Zmaz balik";*/
        //const string menuRefresh = "&Refresh";       
        /*const string menuPridajTriedu = "&Pridaj triedu";
        const string menuZmazDiagram = "&Zmaz diagram";*/
        
        Dictionary dict;

        // the control to add to the add-in window
        //private MyEAControl eaControl;

        ContextWrapper contextWrapper;
        Synchronization synchronization;
        UpdateService updateService;
        SynchronizationService synchronizationService;
        public static DefectsWindow defectsWindow = null;

        /// <summary>
        /// constructor where we set the menuheader and menuOptions
        /// </summary>
        public BPAddIn() : base()
        {                                                   
            this.menuHeader = menuName;
            this.menuOptions = new string[] { /*menuRefresh, menuSynchronization,*/ menuLoginWindow, menuJoining, menuDbTest, /*menuClassNamesValidation,*/ menuUpdate, /*menuOpenProperties*/ };
                //menuPridajElement, menuPridajSpojenie,
                //menuPridajBalik, menuZmenBalik, menuPresunBalik, menuZmazBalik, 
                //menuPridajDiagram, menuZmenDiagram, menuPresunDiagram, menuZmazDiagram, menuRefresh, 

            this.dict = new Dictionary();
            //this.defectsWindow = new DefectsWindow();
        }
        /// <summary>
        /// EA_Connect events enable Add-Ins to identify their type and to respond to Enterprise Architect start up.
        /// This event occurs when Enterprise Architect first loads your Add-In. Enterprise Architect itself is loading at this time so that while a Repository object is supplied, there is limited information that you can extract from it.
        /// The chief uses for EA_Connect are in initializing global Add-In data and for identifying the Add-In as an MDG Add-In.
        /// Also look at EA_Disconnect.
        /// </summary>
        /// <param name="Repository">An EA.Repository object representing the currently open Enterprise Architect model.
        /// Poll its members to retrieve model data and user interface status information.</param>
        /// <returns>String identifying a specialized type of Add-In: 
        /// - "MDG" : MDG Add-Ins receive MDG Events and extra menu options.
        /// - "" : None-specialized Add-In.</returns>
        public override string EA_Connect(EA.Repository Repository)
        {
            //Database.openConnection();

            this.synchronization = new Synchronization();
            this.updateService = new UpdateService();
            updateService.compareVersions();

            this.contextWrapper = new ContextWrapper(Repository);
            this.synchronizationService = new SynchronizationService();

    

            return base.EA_Connect(Repository);
        }
        /// <summary>
        /// Called once Menu has been opened to see what menu items should active.
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Location">the location of the menu</param>
        /// <param name="MenuName">the name of the menu</param>
        /// <param name="ItemName">the name of the menu item</param>
        /// <param name="IsEnabled">boolean indicating whethe the menu item is enabled</param>
        /// <param name="IsChecked">boolean indicating whether the menu is checked</param>       
        public override void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                switch (ItemName)
                {
                    // define the state of the hello menu option                                  
                    /*case menuHello:
                        IsEnabled = shouldWeSayHello;
                        break;
                    // define the state of the goodbye menu option
                    case menuGoodbye:
                        IsEnabled = !shouldWeSayHello;
                        break;*/
                    /*case menuOpenProperties:
                        IsEnabled = true;
                        break;*/
                    case menuDbTest:
                        IsEnabled = true;
                        break;
                    /*case menuClassNamesValidation:
                        IsEnabled = true;
                        break;*/
                    case menuLoginWindow:
                        IsEnabled = true;
                        break;
                    case menuUpdate:
                        IsEnabled = true;
                        break;
                    case menuJoining:
                        IsEnabled = true;
                        break; 
                    /*case menuPridajBalik:
                        IsEnabled = true;
                        break;
                    case menuPridajDiagram:
                        IsEnabled = true;
                        break;
                    case menuZmenDiagram:
                        IsEnabled = true;
                        break;
                    case menuZmazDiagram:
                        IsEnabled = true;
                        break;
                    case menuPresunDiagram:
                        IsEnabled = true;
                        break;
                    case menuZmenBalik:
                        IsEnabled = true;
                        break;
                    case menuPresunBalik:
                        IsEnabled = true;
                        break;
                    case menuZmazBalik:
                        IsEnabled = true;
                        break;*/
                    /*case menuRefresh:
                        IsEnabled = true;
                        break;*/
                    /* case menuPridajElement:
                        IsEnabled = true;
                        break;
                    case menuPridajSpojenie:
                        IsEnabled = true;
                        break;*/                                    
                    /*case menuPridajTriedu:
                        IsEnabled = true;
                        break;
                    case menuZmazDiagram:
                        IsEnabled = true;
                        break;*/
                   /* case menuSynchronization:
                        IsEnabled = true;
                        break;*/
                    // there shouldn't be any other, but just in case disable it.
                    default:
                        IsEnabled = false;
                        break;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

        /// <summary>
        /// Called when user makes a selection in the menu.
        /// This is your main exit point to the rest of your Add-in
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Location">the location of the menu</param>
        /// <param name="MenuName">the name of the menu</param>
        /// <param name="ItemName">the name of the selected menu item</param>
        public override void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                // user has clicked the menuHello menu option
                
                case menuJoining:
                    this.showJoinWindow();
                    break;         
                /*case menuOpenProperties:
                    this.testPropertiesDialog(Repository);
                    break;*/
                case menuDbTest:
                    //MessageBox.Show(dict.testSelect());
                    if (defectsWindow == null)
                    {
                        defectsWindow = Repository.AddWindow("Detekované chyby", "BPAddIn.DefectsWindow") as DefectsWindow;
                    }
                    Repository.ShowAddinWindow("Detekované chyby");
                    break;
                /*case menuClassNamesValidation:
                    //MessageBox.Show(Location);
                    traverseModel(Repository);
                    break;*/
                case menuLoginWindow:
                    showLoginWindow();
                    break;                               
                case menuUpdate:
                    updateService.isConnected();
                    break;

                /*case menuPridajBalik:
                    synchronization.pridajBalik(Repository, 1);
                    break;
                case menuPresunBalik:
                    synchronization.presunBalik(Repository, 15, 2);
                    break;
                case menuZmazBalik:
                    synchronization.zmazBalik(Repository, 14);
                    break;
                case menuZmenBalik:
                    synchronization.zmenBalik(Repository, 3);
                    break;
                case menuPridajDiagram:
                    synchronization.pridajDiagram(Repository, 23);
                    break;
                case menuPresunDiagram:
                    synchronization.presunDiagram(Repository, 3, 7);
                    break;
                case menuZmazDiagram:
                    synchronization.zmazDiagram(Repository, 7);
                    break;
                case menuZmenDiagram:
                    synchronization.zmenDiagram(Repository, 4);
                    break; */               
                /*case menuRefresh:
                    synchronization.refresh(Repository);                   
                    break;*/
                /*case menuNajdiMeno:
                    this.najdiMenoPodlaID(Repository);
                    break;
                case menuPridajElement:
                    synchronization.pridajElement(Repository, 1);
                    break;   
                case menuPridajSpojenie:
                    synchronization.pridajSpojenie(Repository, 113, 117);
                    break;
                case menuZmen:
                    this.zmen(Repository, 8);
                    break;
                case menuPridajTriedu:
                    this.pridajTriedu(Repository, 6);
                    break;
                case menuZmazDiagram:
                    this.zmazDiagram(Repository, 22);
                    break;*/
                /*case menuSynchronization:
                    synchronizationService.checkConnectionForSynchronization(Repository);
                    break;*/
            }
        }

        public void showLoginWindow()
        {
            LogInWindow login = new LogInWindow();
            login.ShowDialog();
        }

        public void showJoinWindow()
        {
            JoinWindow join = new JoinWindow();
            join.ShowDialog();
        }
        
        /// <summary>
        /// Called when EA start model validation. Just shows a message box
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Args">the arguments</param>
		public override void EA_OnStartValidation(EA.Repository Repository, object Args)
        {
            MessageBox.Show("Validation started");
        }
        /// <summary>
        /// Called when EA ends model validation. Just shows a message box
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Args">the arguments</param>
        public override void EA_OnEndValidation(EA.Repository Repository, object Args)
        {
            MessageBox.Show("Validation ended");
        }
        /// <summary>
        /// called when the selected item changes
        /// This operation will show the guid of the selected element in the eaControl
        /// </summary>
        /// <param name="Repository">the EA.Repository</param>
        /// <param name="GUID">the guid of the selected item</param>
        /// <param name="ot">the object type of the selected item</param>
        public override void EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            if (Repository == null || GUID == null || ot.Equals(null))
            {
                return;
            }

            //if (ot == ObjectType.otElement || ot == ObjectType.otDiagram || ot == ObjectType.otPackage || ot == ObjectType.otConnector) {
                contextWrapper.handleContextItemChange(Repository, GUID, ot);
            //}
        }
        public override bool EA_OnContextItemDoubleClicked(Repository Repository, string GUID, ObjectType ot)
        {
            contextWrapper.handleContextItemChange(Repository, GUID, ot);
            return base.EA_OnContextItemDoubleClicked(Repository, GUID, ot);
        }
        public override void EA_OnNotifyContextItemModified(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            try {
                //MessageBox.Show(GUID + " " + Repository.GetElementByGuid(GUID).Type);

                //MessageBox.Show(ot.ToString());
                contextWrapper.handleChange(GUID, ot);
                contextWrapper.broadcastEvent(Repository, GUID, ot);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        public override bool EA_OnPostNewConnector(Repository Repository, EventProperties Info)
        {
            EventProperty connectorID = Info.Get("ConnectorID");
            contextWrapper.handleConnectorCreation(Repository, Convert.ToInt32(connectorID.Value.ToString()));
            return base.EA_OnPostNewConnector(Repository, Info);
        }
        public override bool EA_OnPostNewElement(Repository Repository, EventProperties Info)
        {
            EventProperty elementID = Info.Get("ElementID");
            contextWrapper.handleElementCreation(Repository, Convert.ToInt32(elementID.Value.ToString()));
            return base.EA_OnPostNewElement(Repository, Info);
        }
        public override bool EA_OnPostNewDiagram(Repository Repository, EventProperties Info)
        {
            EventProperty diagramID = Info.Get("DiagramID");
            contextWrapper.handleDiagramCreation(Repository, Convert.ToInt32(diagramID.Value.ToString()));
            return base.EA_OnPostNewDiagram(Repository, Info);
        }
        public override bool EA_OnPostNewPackage(Repository Repository, EventProperties Info)
        {
            EventProperty packageID = Info.Get("PackageID");
            contextWrapper.handlePackageCreation(Repository, Convert.ToInt32(packageID.Value.ToString()));
            return base.EA_OnPostNewPackage(Repository, Info);
        }
        public override bool EA_OnPreDeletePackage(Repository Repository, EventProperties Info)
        {
            EventProperty packageID = Info.Get("PackageID");
            contextWrapper.handlePackageDeletion(Repository, Convert.ToInt32(packageID.Value.ToString()));
            return base.EA_OnPreDeletePackage(Repository, Info);
        }
        public override bool EA_OnPreDeleteDiagram(Repository Repository, EventProperties Info)
        {
            EventProperty diagramID = Info.Get("DiagramID");
            contextWrapper.handleDiagramDeletion(Repository, Convert.ToInt32(diagramID.Value.ToString()));
            return base.EA_OnPreDeleteDiagram(Repository, Info);
        }
        public override bool EA_OnPreDeleteDiagramObject(Repository Repository, EventProperties Info)
        {
            EventProperty diagramObjectID = Info.Get("DiagramObjectID");
            contextWrapper.handleDiagramObjectDeletion(Repository, Convert.ToInt32(diagramObjectID.Value.ToString()));
            return base.EA_OnPreDeleteDiagramObject(Repository, Info);
        }
        public override bool EA_OnPreDeleteElement(Repository Repository, EventProperties Info)
        {
            EventProperty elementID = Info.Get("ElementID");
            contextWrapper.handleElementDeletion(Repository, Convert.ToInt32(elementID.Value.ToString()));
            return base.EA_OnPreDeleteElement(Repository, Info);
        }
        public override bool EA_OnPreDeleteConnector(Repository Repository, EventProperties Info)
        {
            EventProperty connectorID = Info.Get("ConnectorID");
            contextWrapper.handleConnectorDeletion(Repository, Convert.ToInt32(connectorID.Value.ToString()));
            return base.EA_OnPreDeleteConnector(Repository, Info);
        }
        public override void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void testPropertiesDialog(EA.Repository repository)
        {
            int diagramID = repository.GetCurrentDiagram().DiagramID;
            repository.OpenDiagramPropertyDlg(diagramID);
        }

        public bool checkClassName(string name)
        {
            if (dict.isNoun(name.Split(" ".ToCharArray())))
            {
                return true;
            }

            return false;
        }

        public void traverseModel(EA.Repository Repository)
        {
            for (short i = 0; i < Repository.Models.Count; i++)
            {
                EA.Package model = (EA.Package)Repository.Models.GetAt(i);

                if (model.Packages.Count > 0)
                {
                    traversePackages(model.Packages);
                }

                if (model.Elements.Count > 0)
                {
                    traverseElements(model.Elements);
                }
            }
        }

        public void traversePackages(EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package) packages.GetAt(i);

                if (package.Packages.Count > 0)
                {
                    traversePackages(package.Packages);
                }

                if (package.Elements.Count > 0)
                {
                    traverseElements(package.Elements);
                }
            }
        }

        public void traverseElements(EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element elem = (EA.Element)elements.GetAt(i);
                if (elem.Type.Equals("Class"))
                {
                    if (!checkClassName(elem.Name))
                    {
                        MessageBox.Show(elem.Name + " nie je spravny nazov pre triedu");
                    }
                }
            }
        }
    }
    public class InternalHelpers
    {
        static public IWin32Window GetMainWindow()
        {
            List<Process> allProcesses = new List<Process>(Process.GetProcesses());
            Process proc = allProcesses.Find(pr => pr.ProcessName == "EA");
            if (proc.MainWindowTitle == "")  //somtimes a wrong handle is returned, in this case also the title is emty
                return null;                   //return null in this case
            else                             //otherwise return the right handle
                return new WindowWrapper(proc.MainWindowHandle);
        }


        internal class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public IntPtr Handle
            {
                get { return _hwnd; }
            }

            private IntPtr _hwnd;
        }
    }

    public enum EaType
    {
        Package,
        Element,
        Attribute,
        Operation,
        Diagram
    }

    public static class EaRepositoryExtensions
    {
        static public DialogResult ShowDialogAtMainWindow(this Form form)
        {
            IWin32Window win32Window = InternalHelpers.GetMainWindow();
            if (win32Window != null)  // null means that the main window handle could not be evaluated
                return form.ShowDialog(win32Window);
            else
                return form.ShowDialog();  //fallback: use it without owner
        }
        public static void OpenEaPropertyDlg(this EA.Repository rep, int id, EaType type)
        {
            string dlg;
            switch (type)
            {
                case EaType.Package: dlg = "PKG"; break;
                case EaType.Element: dlg = "ELM"; break;
                case EaType.Attribute: dlg = "ATT"; break;
                case EaType.Operation: dlg = "OP"; break;
                case EaType.Diagram: dlg = "DGM"; break;
                default: dlg = String.Empty; break;
            }
            IWin32Window mainWindow = InternalHelpers.GetMainWindow();
            if (mainWindow != null)
            {
                string ret = rep.CustomCommand("CFormCommandHelper", "ProcessCommand", "Dlg=" + dlg + ";id=" + id + ";hwnd=" + mainWindow.Handle);
            }
        }

        public static void OpenPackagePropertyDlg(this EA.Repository rep, int packageId)
        {
            rep.OpenEaPropertyDlg(packageId, EaType.Package);
        }

        public static void OpenElementPropertyDlg(this EA.Repository rep, int elementId)
        {
            rep.OpenEaPropertyDlg(elementId, EaType.Element);
        }

        public static void OpenAttributePropertyDlg(this EA.Repository rep, int attributeId)
        {
            rep.OpenEaPropertyDlg(attributeId, EaType.Attribute);
        }

        public static void OpenOperationPropertyDlg(this EA.Repository rep, int opertaionId)
        {
            rep.OpenEaPropertyDlg(opertaionId, EaType.Operation);
        }

        public static void OpenDiagramPropertyDlg(this EA.Repository rep, int diagramId)
        {
            rep.OpenEaPropertyDlg(diagramId, EaType.Diagram);
        }
    }

}