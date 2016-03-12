using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Threading;
using BPAddInTry;

namespace BPAddIn
{
    class ContextWrapper
    {
        public Wrapper.Model model;
        public EA.Element currentItem { get; set; }
        public Wrapper.Diagram currentDiagram { get; set; }
        public EA.Connector currentConnector { get; set; }
        public EA.Package currentPackage { get; set; }
        public EA.Scenario currentScenario { get; set; }
        public string currentAuthor { get; set; }
        private ChangeService changeService;
        private RuleService ruleService;
        private bool changed = false;
        private Thread dispatcherThread;

        public ContextWrapper(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
            this.changeService = new ChangeService();
            this.ruleService = new RuleService();
            dispatcherThread = new Thread(new ThreadStart(this.changeService.startActivityDispatcher));
            dispatcherThread.Start();
        }

        public void handleContextItemChange(EA.Repository repository, string GUID, ObjectType ot)
        {
            try
            {
                switch (ot)
                {
                    case ObjectType.otElement:
                        //this.currentItem = (Wrapper.Element)model.selectedElement;
                        this.currentItem = repository.GetElementByGuid(GUID);
                        this.currentConnector = null;
                        this.currentDiagram = null;
                        changed = false;
                        currentAuthor = repository.GetElementByGuid(GUID).Author;
                        break;
                    case ObjectType.otPackage:
                        //this.currentItem = (Wrapper.Element)model.selectedElement;
                        this.currentItem = repository.GetElementByGuid(GUID);
                        this.currentConnector = null;
                        this.currentDiagram = null;
                        changed = false;
                        currentAuthor = repository.GetElementByGuid(GUID).Author;
                        break;
                    case ObjectType.otConnector:
                        this.currentConnector = repository.GetConnectorByGuid(GUID);
                        this.currentItem = null;
                        this.currentDiagram = null;
                        changed = false;
                        break;
                    case ObjectType.otDiagram:
                        this.currentDiagram = (Wrapper.Diagram)model.selectedDiagram;
                        this.currentConnector = null;
                        this.currentItem = null;
                        changed = false;
                        currentAuthor = ((EA.Diagram)repository.GetDiagramByGuid(GUID)).Author;
                        break;
                    case ObjectType.otScenario:
                        MessageBox.Show(repository.GetElementByGuid(GUID).Name);
                        break;
                    default:
                        return;
                }
            }
            catch (NullReferenceException nEx) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void handleChange(string GUID, EA.ObjectType ot)
        {
            if (GUID != null && changed == false && currentItem != null)
            {
                try
                {
                    handleElementChange(GUID);
                    //currentItem = null;
                    //this.changed = true;
                }
                catch (NullReferenceException ex2) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            if (GUID != null && changed == false && currentDiagram != null)
            {
                try
                {
                    handleDiagramChange(GUID);
                    //currentDiagram = null;
                    //this.changed = true;
                }
                catch (NullReferenceException ex2) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            if (GUID != null && changed == false && currentConnector != null)
            {
                try
                {
                    handleConnectorChange(GUID);
                    //currentConnector = null;
                    //this.changed = true;
                }
                catch (NullReferenceException ex2) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void handleElementChange(string GUID)
        {
            EA.Element changedElement = model.getWrappedModel().GetElementByGuid(GUID);

            // check name
            if (currentItem.Name != changedElement.Name)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getElementType(GUID);
                propertyChange.propertyType = 0;
                propertyChange.propertyBody = changedElement.Name;
                propertyChange.oldPropertyBody = currentItem.Name;

                changeService.saveChange(propertyChange); 
            }

            // check author
            //EA.Element chngElement = model.getWrappedModel().GetElementByGuid(GUID);
            
            if (currentItem.Author != changedElement.Author)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getElementType(GUID);
                propertyChange.propertyType = 1;
                propertyChange.propertyBody = changedElement.Author;
                propertyChange.oldPropertyBody = currentItem.Author;

                changeService.saveChange(propertyChange);
            }

            // check stereotype
            //string changedElemStereotype = changedElement.stereotypes.Count > 0 ? changedElement.stereotypes.ElementAt(0).name : "";
            //string currentElemStereotype = currentItem.stereotypes.Count > 0 ? currentItem.stereotypes.ElementAt(0).name : "";
            string changedElemStereotype = changedElement.Stereotype;
            string currentElemStereotype = currentItem.Stereotype;

            if (changedElemStereotype != currentElemStereotype)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getElementType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedElemStereotype;
                propertyChange.oldPropertyBody = currentElemStereotype;

                changeService.saveChange(propertyChange);
            }

            handleUseCaseChanges(GUID, changedElement);

            currentItem = changedElement;
        }

        public void handleElementCreation(Repository repository, int elementID)
        {
            try {
                changed = false;
                EA.Element el = repository.GetElementByID(elementID);
                currentItem = el;
                
                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = el.ElementGUID;
                itemCreation.elementType = getElementType(el.ElementGUID);
                itemCreation.author = el.Author;
                itemCreation.name = el.Name;

                if (el.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(el.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }

                EA.Package package = repository.GetPackageByID(el.PackageID);
                if (package != null)
                {
                    itemCreation.packageGUID = package.PackageGUID;
                }

                string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID="+el.ElementID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    MessageBox.Show(diagram.name);

                    itemCreation.diagramGUID = diagram.diagramGUID;
                    Wrapper.ElementWrapper elWrapper = new Wrapper.ElementWrapper(model, el);
                    DiagramObject cur = diagram.getdiagramObjectForElement(elWrapper);
                    string coordinates = "";
                    coordinates += "l=" + cur.left + ";";
                    coordinates += "r=" + cur.right + ";";
                    coordinates += "t=" + cur.top + ";";
                    coordinates += "b=" + cur.bottom + ";";
                    itemCreation.coordinates = coordinates;
                }

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void handleDiagramChange(string GUID)
        {
            Wrapper.Diagram changedDiagram = (Wrapper.Diagram)model.getDiagramByGUID(GUID);

            // check name
            if (currentDiagram.name != changedDiagram.name)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getDiagramType(GUID);
                propertyChange.propertyType = 0;
                propertyChange.propertyBody = changedDiagram.name;
                propertyChange.oldPropertyBody = currentDiagram.name;

                changeService.saveChange(propertyChange);
            }

            // check author
            EA.Diagram chngElement = (EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID);

            if (currentAuthor != chngElement.Author)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getDiagramType(GUID);
                propertyChange.propertyType = 1;
                propertyChange.propertyBody = chngElement.Author;
                propertyChange.oldPropertyBody = currentAuthor;

                changeService.saveChange(propertyChange);
            }

            // check stereotype
            string changedDiagStereotype = changedDiagram.stereotypes.Count > 0 ? changedDiagram.stereotypes.ElementAt(0).name : "";
            string currentDiagStereotype = currentDiagram.stereotypes.Count > 0 ? currentDiagram.stereotypes.ElementAt(0).name : "";

            if (changedDiagStereotype != currentDiagStereotype)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getDiagramType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedDiagStereotype;
                propertyChange.oldPropertyBody = currentDiagStereotype;

                changeService.saveChange(propertyChange);
            }

            currentDiagram = changedDiagram;
        }

        public void handleDiagramCreation(Repository repository, int diagramID)
        {
            try
            {
                changed = false;
                EA.Diagram diagram = repository.GetDiagramByID(diagramID);
                currentDiagram = (Wrapper.Diagram)model.getDiagramByGUID(diagram.DiagramGUID);

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = diagram.DiagramGUID;
                itemCreation.elementType = getDiagramType(diagram.DiagramGUID);
                itemCreation.author = diagram.Author;
                itemCreation.name = diagram.Name;
                itemCreation.parentGUID = "0";

                if (diagram.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(diagram.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }

                EA.Package package = repository.GetPackageByID(diagram.PackageID);
                if (package != null)
                {
                    itemCreation.packageGUID = package.PackageGUID;
                }

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void handlePackageCreation(Repository repository, int packageID)
        {
            try
            {
                changed = false;
                EA.Package package = repository.GetPackageByID(packageID);
                currentPackage = package;

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = package.PackageGUID;
                itemCreation.elementType = 800;
                itemCreation.author = package.Element.Author;
                itemCreation.name = package.Name;
                itemCreation.parentGUID = "0";

                if (package.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(package.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }

                EA.Package parentPackage = repository.GetPackageByID(package.PackageID);
                if (parentPackage != null)
                {
                    itemCreation.packageGUID = parentPackage.PackageGUID;
                }

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void handleConnectorChange(string GUID)
        {
            EA.Connector changedConnector = model.getWrappedModel().GetConnectorByGuid(GUID);

            // check name
            if (currentConnector.Name != changedConnector.Name)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getConnectorType(GUID);
                propertyChange.propertyType = 0;
                propertyChange.propertyBody = changedConnector.Name;
                propertyChange.oldPropertyBody = currentConnector.Name;

                changeService.saveChange(propertyChange);
            }

            // check stereotype
            if (changedConnector.Stereotype != currentConnector.Stereotype)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getConnectorType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedConnector.Stereotype;
                propertyChange.oldPropertyBody = currentConnector.Stereotype;

                changeService.saveChange(propertyChange);
            }

            // check target
            if (changedConnector.SupplierID != currentConnector.SupplierID)
            {
                string changedSupplierGUID = model.getWrappedModel().GetElementByID(changedConnector.SupplierID).ElementGUID;
                string currentSupplierGUID = model.getWrappedModel().GetElementByID(currentConnector.SupplierID).ElementGUID;

                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getConnectorType(GUID);
                propertyChange.propertyType = 301;
                propertyChange.propertyBody = changedSupplierGUID;
                propertyChange.oldPropertyBody = currentSupplierGUID;

                changeService.saveChange(propertyChange);
            }

            // check source
            if (changedConnector.ClientID != currentConnector.ClientID)
            {
                string changedClientGUID = model.getWrappedModel().GetElementByID(changedConnector.ClientID).ElementGUID;
                string currentClientGUID = model.getWrappedModel().GetElementByID(currentConnector.ClientID).ElementGUID;

                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getConnectorType(GUID);
                propertyChange.propertyType = 302;
                propertyChange.propertyBody = changedClientGUID;
                propertyChange.oldPropertyBody = currentClientGUID;

                changeService.saveChange(propertyChange);
            }

            currentConnector = changedConnector;
        }

        public void handleConnectorCreation(Repository repository, int connectorID)
        {
            try
            {
                currentItem = null;
                currentDiagram = null;
                changed = false;
                EA.Connector connector = repository.GetConnectorByID(connectorID);
                currentConnector = connector;

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = connector.ConnectorGUID;
                itemCreation.elementType = getConnectorType(connector.ConnectorGUID);
                itemCreation.name = connector.Name;
                itemCreation.srcGUID = repository.GetElementByID(connector.ClientID).ElementGUID;
                itemCreation.targetGUID = repository.GetElementByID(connector.SupplierID).ElementGUID;

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void handleUseCaseChanges(string GUID, EA.Element changedElement)
        {
            if (changedElement.Type != "UseCase")
            {
                return;
            }

            EA.Collection changedConstraints = changedElement.Constraints;

            foreach (Constraint constraint in changedConstraints)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = getElementType(GUID);
                propertyChange.propertyType = 10;
                propertyChange.propertyBody = constraint.Name;
                propertyChange.oldPropertyBody = constraint.Type;

                changeService.saveChange(propertyChange);
            }
        }

        public int getElementType(string GUID)
        {
            string type = model.getWrappedModel().GetElementByGuid(GUID).Type;

            switch (type)
            {
                case "Class":
                    return 0;
                case "Activity":
                    return 1;
                case "UseCase":
                    return 2;
                case "Package":
                    return 3;
                case "Actor":
                    return 30;
                default:
                    return -1;
            }
        }

        public int getDiagramType(string GUID)
        {
            string type = ((EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID)).Type;

            switch (type)
            {
                case "Logical":
                    return 50;
                case "Activity":
                    return 51;
                case "Use Case":
                    return 52;
                case "Sequence":
                    return 53;
                case "Statechart":
                    return 54;
                case "Analysis":
                    return 55;
                case "Component":
                    return 56;
                case "Deployment":
                    return 57;
                case "Custom":
                    return 58;
                default:
                    return -1;
            }
        }

        public int getConnectorType(string GUID)
        {
            EA.Connector connector = (EA.Connector)model.getWrappedModel().GetConnectorByGuid(GUID);
            string type = ((EA.Connector)model.getWrappedModel().GetConnectorByGuid(GUID)).Type;

            switch (type)
            {
                case "Association":
                    return 70;
                case "Generalization":
                    return 71;
                case "Realization":
                    return 72;
                case "UseCase":
                    if (connector.Stereotype == "include")
                    {
                        return 73;
                    }

                    if (connector.Stereotype == "extend")
                    {
                        return 74;
                    }
                    return -1;
                default:
                    return -1;
            }
        }

        public void broadcastEvent(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            ruleService.broadcastEvent(model, Repository, GUID, ot);
        }

        public void broadcastEvent(EA.Repository Repository, long ID)
        {
            ruleService.broadcastEvent(model, Repository, ID);
        }

        public string printCurrentItem()
        {
            return currentItem.ToString();
        }
        public string printCurrentDiagram()
        {
            return currentDiagram.ToString();
        }
        public string printCurrentConnector()
        {
            return currentConnector.ToString();
        }
    }
}
