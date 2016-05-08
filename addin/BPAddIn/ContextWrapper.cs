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
using BPAddIn.ElementWrappers;
using Newtonsoft.Json;

namespace BPAddIn
{
    class ContextWrapper
    {
        public Wrapper.Model model;
        public EA.Element currentItem { get; set; }
        public EA.Diagram currentDiagram { get; set; }
        public EA.Connector currentConnector { get; set; }
        public EA.Package currentPackage { get; set; }
        public EA.Scenario currentScenario { get; set; }
        public string currentParent { get; set; }
        List<ConstraintWrapper> currentConstraintsList { get; set; }
        List<ScenarioWrapper> currentScenarioList { get; set; }
        Dictionary<string, List<EA.ScenarioStep>> currentScenarioStepList { get; set; }
        Dictionary<string, string> currentExtensionPoints { get; set; }
        Dictionary<string, EA.Attribute> currentAttributes { get; set; }
        Dictionary<int, string> currentDiagramObjectPositions { get; set; }
        //Dictionary<int, int> currentDiagramObjectSequences { get; set; }
        public string currentAuthor { get; set; }
        private ChangeService changeService;
        private RuleService ruleService;
        private bool changed = false;
        private Thread changesDispatcherThread;
        private ItemTypes itemTypes;

        public ContextWrapper(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
            this.changeService = new ChangeService();
            this.ruleService = new RuleService();
            this.itemTypes = new ItemTypes(repository);
            changesDispatcherThread = new Thread(new ThreadStart(this.changeService.startActivityDispatcher));
            changesDispatcherThread.Start();
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
                        //this.currentConstraints = currentItem.Constraints;

                        if (currentItem.Type == "Class")
                        {
                            currentAttributes = new Dictionary<string, EA.Attribute>();
                            foreach (EA.Attribute attr in currentItem.Attributes)
                            {
                                currentAttributes.Add(attr.AttributeGUID, attr);
                            }
                        }

                        if (currentItem.Type == "UseCase")
                        {
                            // CONSTRAINTS
                            currentConstraintsList = new List<ConstraintWrapper>();
                            foreach (EA.Constraint constraint in currentItem.Constraints)
                            {
                                currentConstraintsList.Add(new ConstraintWrapper(constraint));
                            }

                            // SCENARIOS
                            currentScenarioList = new List<ScenarioWrapper>();
                            currentScenarioStepList = new Dictionary<string, List<EA.ScenarioStep>>();
                            foreach (EA.Scenario scenario in currentItem.Scenarios)
                            {
                                currentScenarioList.Add(new ScenarioWrapper(scenario));
                                if (!currentScenarioStepList.ContainsKey(scenario.ScenarioGUID))
                                {
                                    currentScenarioStepList.Add(scenario.ScenarioGUID, new List<ScenarioStep>());
                                }

                                foreach (ScenarioStep step in scenario.Steps)
                                {
                                    currentScenarioStepList[scenario.ScenarioGUID].Add(step);
                                }
                            }
                        }

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
                        this.currentDiagram = (EA.Diagram)repository.GetDiagramByGuid(GUID);
                        this.currentConnector = null;
                        this.currentItem = null;
                        changed = false;
                        currentAuthor = ((EA.Diagram)repository.GetDiagramByGuid(GUID)).Author;
                        currentParent = currentDiagram.ParentID.ToString();
                        /*if (currentDiagram.Type == "Use Case")
                        {*/
                            currentExtensionPoints = new Dictionary<string, string>();
                            currentDiagramObjectPositions = new Dictionary<int, string>();
                            foreach (EA.DiagramObject diagramObject in currentDiagram.DiagramObjects)
                            {
                                try
                                {
                                    EA.Collection col = repository.GetElementSet("" + diagramObject.ElementID, 1);
                                    //EA.Element element = repository.GetElementByID(diagramObject.ElementID);
                                    EA.Element element = (EA.Element)col.GetAt(0);

                                    if (element.Type == "UseCase")
                                    {
                                        currentExtensionPoints.Add(element.ElementGUID, element.ExtensionPoints);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show(ex.ToString());
                                }

                                string coordinates = "";
                                coordinates += "l=" + diagramObject.left + ";";
                                coordinates += "r=" + diagramObject.right + ";";
                                coordinates += "t=" + diagramObject.top + ";";
                                coordinates += "b=" + diagramObject.bottom + ";";
                                currentDiagramObjectPositions.Add(diagramObject.ElementID, coordinates);

                                //currentDiagramObjectSequences.Add(diagramObject.ElementID, diagramObject.Sequence);
                            }
                        //}

                        
                        /*foreach (EA.DiagramObject diagramObject in currentDiagram.DiagramObjects)
                        {
                            string coordinates = "";
                            coordinates += "l=" + diagramObject.left + ";";
                            coordinates += "r=" + diagramObject.right + ";";
                            coordinates += "t=" + diagramObject.top + ";";
                            coordinates += "b=" + diagramObject.bottom + ";";
                            currentDiagramObjectPositions.Add(diagramObject.ElementID, coordinates);
                        }*/
                        break;
                    default:
                        return;
                }
            }
            catch (NullReferenceException nEx) { }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        public void handleChange(EA.Repository repository, string GUID, EA.ObjectType ot)
        {
            // check extension pointy       
            if (ot == ObjectType.otDiagram)
            {
                try
                {
                    EA.Diagram changedDiagram = (EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID);
                    foreach (EA.DiagramObject diagramObject in changedDiagram.DiagramObjects)
                    {
                        string coordinates = "";
                        coordinates += "l=" + diagramObject.left + ";";
                        coordinates += "r=" + diagramObject.right + ";";
                        coordinates += "t=" + diagramObject.top + ";";
                        coordinates += "b=" + diagramObject.bottom + ";";

                       // MessageBox.Show(diagramObject.ElementID.ToString());
                        if (currentDiagramObjectPositions[diagramObject.ElementID] != coordinates)
                        {
                           // MessageBox.Show("if");
                            EA.Element element = model.getWrappedModel().GetElementByID(diagramObject.ElementID);

                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                            propertyChange.itemGUID = element.ElementGUID;
                            propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                            propertyChange.propertyType = 405;
                            propertyChange.propertyBody = coordinates;
                            propertyChange.oldPropertyBody = GUID;

                            changeService.saveChange(propertyChange);

                            currentDiagramObjectPositions[diagramObject.ElementID] = coordinates;
                        }

                        /*if (currentDiagramObjectSequences[diagramObject.ElementID] != diagramObject.Sequence)
                        {
                            // MessageBox.Show("if");
                            EA.Element element = model.getWrappedModel().GetElementByID(diagramObject.ElementID);

                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                            propertyChange.itemGUID = element.ElementGUID;
                            propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                            propertyChange.propertyType = 406;
                            propertyChange.propertyBody = diagramObject.Sequence.ToString();
                            propertyChange.oldPropertyBody = GUID;

                            changeService.saveChange(propertyChange);

                            currentDiagramObjectSequences[diagramObject.ElementID] = diagramObject.Sequence;
                        }*/
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }

                try
                {
                    EA.Diagram changedDiagram = (EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID);
                    if (changedDiagram.Type == "Use Case")
                    {
                        Dictionary<string, string> updatedExtensionPoints = new Dictionary<string, string>();

                        foreach (EA.DiagramObject diagramObjects in changedDiagram.DiagramObjects)
                        {
                            EA.Element element = (EA.Element)repository.GetElementByID(diagramObjects.ElementID);
                            if (element.Type == "UseCase")
                            {
                                if (!currentExtensionPoints.ContainsKey(element.ElementGUID))
                                {
                                    PropertyChange propertyChange = new PropertyChange();
                                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                                    propertyChange.itemGUID = element.ElementGUID;
                                    propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                                    propertyChange.propertyType = 13;
                                    propertyChange.propertyBody = element.ExtensionPoints;
                                    propertyChange.oldPropertyBody = "";

                                    changeService.saveChange(propertyChange);
                                    updatedExtensionPoints.Add(element.ElementGUID, element.ExtensionPoints);
                                    currentExtensionPoints.Add(element.ElementGUID, element.ExtensionPoints);
                                }
                                else if (currentExtensionPoints[element.ElementGUID] != element.ExtensionPoints)
                                {
                                    PropertyChange propertyChange = new PropertyChange();
                                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                                    propertyChange.itemGUID = element.ElementGUID;
                                    propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                                    propertyChange.propertyType = 13;
                                    propertyChange.propertyBody = element.ExtensionPoints;
                                    propertyChange.oldPropertyBody = currentExtensionPoints[element.ElementGUID];

                                    changeService.saveChange(propertyChange);
                                    updatedExtensionPoints.Add(element.ElementGUID, element.ExtensionPoints);
                                    currentExtensionPoints[element.ElementGUID] = element.ExtensionPoints;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }

                
            }

            if (GUID != null && changed == false && currentItem != null)
            {
                try
                {
                    handleAttributeChange(GUID);
                    handleElementChange(GUID);
                    //currentItem = null;
                    //this.changed = true;
                }
                catch (NullReferenceException ex2) { }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
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
                    //MessageBox.Show(ex.ToString());
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
                    //MessageBox.Show(ex.ToString());
                }
            }
        }

        public void handleElementChange(string GUID)
        {
            EA.Element changedElement = model.getWrappedModel().GetElementByGuid(GUID);
            EA.Element changedPositionElement = model.getWrappedModel().GetElementByGuid(currentItem.ElementGUID);
            //MessageBox.Show(changedElement.Name);
            //check move
            if ((currentItem.PackageID != changedPositionElement.PackageID) || (currentItem.ParentID != changedPositionElement.ParentID))
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.elementType = 0;

                if (changedPositionElement.ParentID == 0)
                {
                    //MessageBox.Show("tu");
                    EA.Package targetPackage = model.getWrappedModel().GetPackageByID(changedPositionElement.PackageID);
                    propertyChange.itemGUID = changedPositionElement.ElementGUID;
                    propertyChange.elementType = itemTypes.getElementType(changedPositionElement.ElementGUID);
                    propertyChange.propertyType = 401;
                    propertyChange.propertyBody = targetPackage.PackageGUID;
                }
                else
                { 
                    /*MessageBox.Show(changedPositionElement.Name + " " + currentItem.Name);
                    MessageBox.Show("tu2");*/
                    EA.Element targetElement = model.getWrappedModel().GetElementByID(changedPositionElement.ParentID);
                    propertyChange.itemGUID = changedPositionElement.ElementGUID;
                    propertyChange.propertyType = 402;
                    propertyChange.propertyBody = targetElement.ElementGUID;
                }
                //MessageBox.Show("save");
                changeService.saveChange(propertyChange);
                return;
            }

            // check name
            if (currentItem.Name != changedElement.Name)
            {
                //MessageBox.Show("element change");
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getElementType(GUID);
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
                propertyChange.elementType = itemTypes.getElementType(GUID);
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
                propertyChange.elementType = itemTypes.getElementType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedElemStereotype;
                propertyChange.oldPropertyBody = currentElemStereotype;

                changeService.saveChange(propertyChange);
            }

            // check notes
            if (currentItem.Notes != changedElement.Notes)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getElementType(GUID);
                propertyChange.propertyType = 2;
                propertyChange.propertyBody = changedElement.Notes;
                propertyChange.oldPropertyBody = currentItem.Notes;

                changeService.saveChange(propertyChange);
            }

            // check subtype
            if (currentItem.Subtype != changedElement.Subtype)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getElementType(GUID);
                propertyChange.propertyType = 3;
                propertyChange.propertyBody = changedElement.Subtype.ToString();
                propertyChange.oldPropertyBody = currentItem.Subtype.ToString();

                changeService.saveChange(propertyChange);
            }

            try
            {
                handleUseCaseChanges(GUID, changedElement);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

            currentItem = changedElement;
        }

        public void handleAttributeChange(string GUID)
        {
            EA.Element changedElement = model.getWrappedModel().GetElementByGuid(GUID);

            foreach (EA.Attribute attr in changedElement.Attributes)
            {
                EA.Attribute currentAttribute = currentAttributes[attr.AttributeGUID];
                // check name
                if (currentAttribute.Name != attr.Name)
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    propertyChange.itemGUID = attr.AttributeGUID;
                    propertyChange.elementType = 90;
                    propertyChange.propertyType = 0;
                    propertyChange.propertyBody = attr.Name;
                    propertyChange.oldPropertyBody = currentAttribute.Name;

                    changeService.saveChange(propertyChange);
                    currentAttributes[attr.AttributeGUID].Name = attr.Name;
                }

                // check scope
                if (currentAttribute.Visibility != attr.Visibility)
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    propertyChange.itemGUID = attr.AttributeGUID;
                    propertyChange.elementType = 90;
                    propertyChange.propertyType = 300;
                    propertyChange.propertyBody = attr.Visibility;
                    propertyChange.oldPropertyBody = currentAttribute.Visibility;

                    changeService.saveChange(propertyChange);
                    currentAttributes[attr.AttributeGUID].Visibility = attr.Visibility;
                }

                // check notes
                if (currentAttribute.Notes != attr.Notes)
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    propertyChange.itemGUID = attr.AttributeGUID;
                    propertyChange.elementType = 90;
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = attr.Notes;
                    propertyChange.oldPropertyBody = currentAttribute.Notes;

                    changeService.saveChange(propertyChange);
                    currentAttributes[attr.AttributeGUID].Visibility = attr.Visibility;
                }
            }

            

            //check visibility
            /*if (currentAttribute.Visibility != changedAttribute.Visibility)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = 90;
                propertyChange.propertyType = 300;
                propertyChange.propertyBody = changedAttribute.Visibility;
                propertyChange.oldPropertyBody = currentAttribute.Visibility;

                changeService.saveChange(propertyChange);
            }*/

            //currentAttribute = changedAttribute;
        }

        public void handleDiagramObjectCreation(Repository repository, int elementID, int diagramID, string DUID)
        {
            try
            {
                changed = false;
                /*string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                            inner join t_object o on do.Object_ID = o.Object_ID
                                            where o.Object_ID=" + elementID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    EA.Diagram diag = diagram.wrappedDiagram;
                    EA.DiagramObject diagObj = diag.GetDiagramObjectByID(elementID, "");
                    EA.Element element = repository.GetElementByID(diagObj.ElementID);

                    ItemCreation itemCreation = new ItemCreation();
                    itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    itemCreation.itemGUID = element.ElementGUID;
                    itemCreation.elementType = itemTypes.getElementType(element.ElementGUID);
                    itemCreation.author = element.Author;
                    itemCreation.name = element.Name;
                    itemCreation.parentGUID = "0";

                    changeService.saveChange(itemCreation);
                }*/

                EA.Element el = repository.GetElementByID(elementID);
                EA.Diagram diag = repository.GetDiagramByID(diagramID);

                Wrapper.Diagram diagram = new Wrapper.Diagram(model, diag);
                Wrapper.ElementWrapper elWrapper = new Wrapper.ElementWrapper(model, el);

                DiagramObject cur = diagram.getdiagramObjectForElement(elWrapper);
                string coordinates = "";
                coordinates += "l=" + cur.left + ";";
                coordinates += "r=" + cur.right + ";";
                coordinates += "t=" + cur.top + ";";
                coordinates += "b=" + cur.bottom + ";";
                currentDiagramObjectPositions.Add(cur.ElementID, coordinates);

                //EA.DiagramObject cur = diagram.GetDiagramObjectByID(elementID, "");

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = el.ElementGUID;
                itemCreation.diagramGUID = diag.DiagramGUID;
                itemCreation.elementType = 700;
                itemCreation.coordinates = "";

                for (short i = 0; i < diag.DiagramObjects.Count; i++)
                {
                    EA.DiagramObject diagramObject = (EA.DiagramObject)diag.DiagramObjects.GetAt(i);
                    if (diagramObject.ElementID == el.ElementID)
                    {
                        coordinates = "";
                        coordinates += "l=" + diagramObject.left + ";";
                        coordinates += "r=" + diagramObject.right + ";";
                        coordinates += "t=" + diagramObject.top + ";";
                        coordinates += "b=" + diagramObject.bottom + ";";
                        itemCreation.coordinates = coordinates;
                        break;
                    }
                }

                /*itemCreation.elementType = itemTypes.getElementType(element.ElementGUID);
                itemCreation.author = element.Author;
                itemCreation.name = element.Name;
                itemCreation.parentGUID = "0";*/

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleElementCreation(Repository repository, int elementID)
        {
            try {
                //MessageBox.Show("element creation");
                changed = false;
                EA.Element el = repository.GetElementByID(elementID);
                currentItem = el;
                
                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = el.ElementGUID;
                itemCreation.elementType = itemTypes.getElementType(el.ElementGUID);
                itemCreation.author = el.Author;
                itemCreation.name = el.Name;
                itemCreation.parentGUID = "0";

                if (itemTypes.getElementType(el.ElementGUID) != 6 && 
                    (itemTypes.getElementType(el.ElementGUID) < 30 || itemTypes.getElementType(el.ElementGUID) > 44))
                {
                    if (el.ParentID != 0)
                    {
                        EA.Element parent = repository.GetElementByID(el.ParentID);
                        if (parent != null)
                        {
                            itemCreation.parentGUID = parent.ElementGUID;
                        }
                    }
                }

                EA.Package package = repository.GetPackageByID(el.PackageID);
                if (package != null)
                {
                    itemCreation.packageGUID = package.PackageGUID;
                }

                /*string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID="+el.ElementID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    //MessageBox.Show(diagram.name);

                    itemCreation.diagramGUID = diagram.diagramGUID;
                    Wrapper.ElementWrapper elWrapper = new Wrapper.ElementWrapper(model, el);
                    DiagramObject cur = diagram.getdiagramObjectForElement(elWrapper);
                    string coordinates = "";
                    coordinates += "l=" + cur.left + ";";
                    coordinates += "r=" + cur.right + ";";
                    coordinates += "t=" + cur.top + ";";
                    coordinates += "b=" + cur.bottom + ";";
                    itemCreation.coordinates = coordinates;
                }*/

                changeService.saveChange(itemCreation);

                if (el.Type == "UseCase")
                {
                    currentExtensionPoints.Add(el.ElementGUID, el.ExtensionPoints);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleDiagramChange(string GUID)
        {
            EA.Diagram changedDiagram = (EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID);
            EA.Diagram changedPositionDiagram = (EA.Diagram)model.getWrappedModel().GetDiagramByGuid(currentDiagram.DiagramGUID);
            //MessageBox.Show(changedElement.Name);
            //check move
            /*MessageBox.Show(currentDiagram.PackageID.ToString() + " " + changedDiagram.PackageID.ToString() + " " 
                + currentDiagram.ParentID.ToString() + " " + changedDiagram.ParentID.ToString()
                + " " + changedPositionDiagram.ParentID.ToString() + " " + changedPositionDiagram.PackageID.ToString());*/

            if ((currentDiagram.PackageID != changedDiagram.PackageID) || (currentDiagram.ParentID != changedDiagram.ParentID) ||
                (currentParent != changedDiagram.ParentID.ToString()))
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.elementType = 0;

                /*if (changedPositionDiagram.ParentID == 0)
                {*/
                    //MessageBox.Show("tu");
                //MessageBox.Show(changedPositionDiagram.ParentID.ToString());
                EA.Package targetPackage = model.getWrappedModel().GetPackageByID(changedDiagram.PackageID);

                if (changedDiagram.ParentID != 0)
                {
                    EA.Element targetElement = model.getWrappedModel().GetElementByID(changedDiagram.ParentID);
                    propertyChange.itemGUID = changedDiagram.DiagramGUID;
                    propertyChange.propertyType = 403;
                    propertyChange.propertyBody = targetElement.ElementGUID;
                }
                else 
                {
                    propertyChange.itemGUID = changedDiagram.DiagramGUID;
                    propertyChange.propertyType = 404;
                    propertyChange.propertyBody = targetPackage.PackageGUID;
                }

            
                /*}
                else
                {
                    /*MessageBox.Show(changedPositionElement.Name + " " + currentItem.Name);
                    MessageBox.Show("tu2");
                    EA.Element targetElement = model.getWrappedModel().GetElementByID(changedPositionDiagram.ParentID);
                    propertyChange.itemGUID = changedPositionDiagram.DiagramGUID;
                    propertyChange.propertyType = 404;
                    propertyChange.propertyBody = targetElement.ElementGUID;*/
                //}
                //MessageBox.Show("save");
                changeService.saveChange(propertyChange);
            }

            // check name
            if (currentDiagram.Name != changedDiagram.Name)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getDiagramType(GUID);
                propertyChange.propertyType = 0;
                propertyChange.propertyBody = changedDiagram.Name;
                propertyChange.oldPropertyBody = currentDiagram.Name;

                changeService.saveChange(propertyChange);
            }

            // check author       
            if (currentAuthor != changedDiagram.Author)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getDiagramType(GUID);
                propertyChange.propertyType = 1;
                propertyChange.propertyBody = changedDiagram.Author;
                propertyChange.oldPropertyBody = currentAuthor;

                changeService.saveChange(propertyChange);
            }

            // check stereotype
            string changedDiagStereotype = changedDiagram.Stereotype;
            string currentDiagStereotype = currentDiagram.Stereotype;

            if (changedDiagStereotype != currentDiagStereotype)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getDiagramType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedDiagStereotype;
                propertyChange.oldPropertyBody = currentDiagStereotype;

                changeService.saveChange(propertyChange);
            }

            // check notes
            if (changedDiagram.Notes != currentDiagram.Notes)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getDiagramType(GUID);
                propertyChange.propertyType = 2;
                propertyChange.propertyBody = changedDiagram.Notes;
                propertyChange.oldPropertyBody = currentDiagram.Notes;

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

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = diagram.DiagramGUID;
                itemCreation.elementType = itemTypes.getDiagramType(diagram.DiagramGUID);
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
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleAttributeCreation(Repository repository, int attributeID)
        {
            try
            {
                changed = false;
                EA.Attribute attribute = repository.GetAttributeByID(attributeID);
                EA.Element element = repository.GetElementByID(attribute.ParentID);

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                itemCreation.itemGUID = attribute.AttributeGUID;
                itemCreation.parentGUID = element.ElementGUID;
                itemCreation.elementType = 90;
                itemCreation.name = attribute.Name;
                itemCreation.coordinates = attribute.Visibility;

                changeService.saveChange(itemCreation);

                currentAttributes.Add(attribute.AttributeGUID, attribute);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handlePackageDeletion(Repository repository, int packageID)
        {
            try
            {
                changed = false;
                EA.Package package = repository.GetPackageByID(packageID);

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = package.PackageGUID;
                modelChange.elementType = 3;
                modelChange.elementDeleted = 1;

                changeService.saveChange(modelChange);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleAttributeDeletion(Repository repository, int attributeID)
        {
            try
            {
                changed = false;
                EA.Attribute attribute = repository.GetAttributeByID(attributeID);

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = attribute.AttributeGUID;
                modelChange.elementType = 90;
                modelChange.elementDeleted = 1;

                changeService.saveChange(modelChange);

                currentAttributes.Remove(attribute.AttributeGUID);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleDiagramDeletion(Repository repository, int diagramID)
        {
            try
            {
                changed = false;
                EA.Diagram diagram = repository.GetDiagramByID(diagramID);

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = diagram.DiagramGUID;
                modelChange.elementType = itemTypes.getDiagramType(diagram.DiagramGUID); ;
                modelChange.elementDeleted = 1;

                changeService.saveChange(modelChange);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleDiagramObjectDeletion(Repository repository, int elementID)
        {
            try
            {
                changed = false;

                /*string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID=" + diagramObjectID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    //MessageBox.Show(diagram.name);
                    for (short i = 0; i < diagram.diagramElements.Count; i++)
                    {
                        EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.diagramElements
                    }
                    //itemCreation.diagramGUID = diagram.diagramGUID;
                    //Wrapper.ElementWrapper elWrapper = new Wrapper.ElementWrapper(model, el);
                    DiagramObject cur = diagram.getdiagramObjectForElement(elWrapper);
                    string coordinates = "";
                    coordinates += "l=" + cur.left + ";";
                    coordinates += "r=" + cur.right + ";";
                    coordinates += "t=" + cur.top + ";";
                    coordinates += "b=" + cur.bottom + ";";
                    itemCreation.coordinates = coordinates;
                }*/

                EA.Element element = repository.GetElementByID(elementID);
                EA.Diagram diagram = repository.GetCurrentDiagram();

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = element.ElementGUID;
                modelChange.propertyBody = diagram.DiagramGUID;
                modelChange.elementType = 700;
                modelChange.elementDeleted = 1;

                changeService.saveChange(modelChange);

                currentDiagramObjectPositions.Remove(element.ElementID);
            }
            catch (Exception ex)
            {
               //MessageBox.Show(ex.ToString());
            }
        }

        public void handleElementDeletion(Repository repository, int elementID)
        {
            try
            {
                changed = false;
                EA.Element element = repository.GetElementByID(elementID);

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = element.ElementGUID;
                modelChange.elementType = itemTypes.getElementType(element.ElementGUID); ;
                modelChange.elementDeleted = 1;

                changeService.saveChange(modelChange);

                if (element.Type == "UseCase")
                {
                    currentExtensionPoints.Remove(element.ElementGUID);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleConnectorDeletion(Repository repository, int connectorID)
        {
            try
            {
                changed = false;
                EA.Connector connector = repository.GetConnectorByID(connectorID);
                EA.Element sourceElement = (EA.Element)repository.GetElementByID(connector.ClientID);

                /*string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID="+sourceElement.ElementID;

                Wrapper.Model model = new Wrapper.Model(repository);
                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);*/

                PropertyChange modelChange = new PropertyChange();
                modelChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                modelChange.itemGUID = connector.ConnectorGUID;
                modelChange.elementType = itemTypes.getConnectorType(connector.ConnectorGUID);
                modelChange.elementDeleted = 1;

                /*if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    modelChange.propertyBody = diagram.diagramGUID;
                }*/

                changeService.saveChange(modelChange);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
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
                itemCreation.elementType = 3;
                itemCreation.author = package.Element.Author;
                itemCreation.name = package.Name;
                itemCreation.parentGUID = "0";

                /*if (package.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(package.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }*/

                EA.Package parentPackage = repository.GetPackageByID(package.ParentID);
                if (parentPackage != null)
                {
                    itemCreation.packageGUID = parentPackage.PackageGUID;
                }

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
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
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
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
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 200;
                propertyChange.propertyBody = changedConnector.Stereotype;
                propertyChange.oldPropertyBody = currentConnector.Stereotype;

                changeService.saveChange(propertyChange);
            }

            // check notes
            if (changedConnector.Notes != currentConnector.Notes)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 2;
                propertyChange.propertyBody = changedConnector.Notes;
                propertyChange.oldPropertyBody = currentConnector.Notes;

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
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
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
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 302;
                propertyChange.propertyBody = changedClientGUID;
                propertyChange.oldPropertyBody = currentClientGUID;

                changeService.saveChange(propertyChange);
            }

            // check source cardinality
            if (changedConnector.ClientEnd.Cardinality != currentConnector.ClientEnd.Cardinality)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 303;
                propertyChange.propertyBody = changedConnector.ClientEnd.Cardinality;
                propertyChange.oldPropertyBody = currentConnector.ClientEnd.Cardinality;

                changeService.saveChange(propertyChange);
            }

            // check target cardinality
            if (changedConnector.SupplierEnd.Cardinality != currentConnector.SupplierEnd.Cardinality)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 304;
                propertyChange.propertyBody = changedConnector.SupplierEnd.Cardinality;
                propertyChange.oldPropertyBody = currentConnector.SupplierEnd.Cardinality;

                changeService.saveChange(propertyChange);
            }

            // check guard
            if (changedConnector.TransitionGuard != currentConnector.TransitionGuard)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 305;
                propertyChange.propertyBody = changedConnector.TransitionGuard;
                propertyChange.oldPropertyBody = currentConnector.TransitionGuard;

                changeService.saveChange(propertyChange);
            }

            // check action
            if (changedConnector.TransitionAction != currentConnector.TransitionAction)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 306;
                propertyChange.propertyBody = changedConnector.TransitionAction;
                propertyChange.oldPropertyBody = currentConnector.TransitionAction;

                changeService.saveChange(propertyChange);
            }

            // check direction
            if (changedConnector.Direction != currentConnector.Direction)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getConnectorType(GUID);
                propertyChange.propertyType = 307;
                propertyChange.propertyBody = changedConnector.Direction;
                propertyChange.oldPropertyBody = currentConnector.Direction;

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
                itemCreation.elementType = itemTypes.getConnectorType(connector.ConnectorGUID);
                itemCreation.name = connector.Name;
                itemCreation.srcGUID = repository.GetElementByID(connector.ClientID).ElementGUID;
                itemCreation.targetGUID = repository.GetElementByID(connector.SupplierID).ElementGUID;

                /*string sqlGetDiagram = @"select do.Diagram_ID from t_diagramlinks do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID="+connector.ConnectorID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    //MessageBox.Show(diagram.name);

                    itemCreation.diagramGUID = diagram.diagramGUID;
                }*/

                //itemCreation.diagramGUID = repository.GetDiagramByID(connector.DiagramID).DiagramGUID;

                changeService.saveChange(itemCreation);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void handleUseCaseChanges(string GUID, EA.Element changedElement)
        {
            if (changedElement.Type != "UseCase")
            {
                return;
            }

            // CONSTRAINTS
            EA.Collection changedConstraints = changedElement.Constraints;
            List<ConstraintWrapper> changedConstraintsList = new List<ConstraintWrapper>();

            foreach (EA.Constraint constraint in changedConstraints)
            {
                changedConstraintsList.Add(new ConstraintWrapper(constraint));
            }

            if (!((changedConstraintsList.Count == currentConstraintsList.Count)
                && !changedConstraintsList.Except(currentConstraintsList).Any()))
            {
                List<ConstraintWrapper> createdConstraints = changedConstraintsList.Except(currentConstraintsList).ToList();
                List<ConstraintWrapper> removedConstraints = currentConstraintsList.Except(changedConstraintsList).ToList();

                foreach (ConstraintWrapper constraintWrapper in removedConstraints)
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    propertyChange.itemGUID = GUID;
                    propertyChange.elementType = itemTypes.getElementType(GUID);
                    propertyChange.propertyType = 11;
                    propertyChange.propertyBody = constraintWrapper.constraint.Name;
                    propertyChange.oldPropertyBody = constraintWrapper.constraint.Type;
                    propertyChange.elementDeleted = 1;

                    changeService.saveChange(propertyChange);
                }

                foreach (ConstraintWrapper constraintWrapper in createdConstraints)
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                    propertyChange.itemGUID = GUID;
                    propertyChange.elementType = itemTypes.getElementType(GUID);
                    propertyChange.propertyType = 10;
                    propertyChange.propertyBody = constraintWrapper.constraint.Name + ",notes:=" + constraintWrapper.constraint.Notes;
                    propertyChange.oldPropertyBody = constraintWrapper.constraint.Type;

                    changeService.saveChange(propertyChange);
                }
            }

            

            // SCENARIO
            EA.Collection changedScenarios = changedElement.Scenarios;
            List<ScenarioWrapper> changedScenariosList = new List<ScenarioWrapper>();

            foreach (EA.Scenario scenario in changedScenarios)
            {
                changedScenariosList.Add(new ScenarioWrapper(scenario));
            }

            Dictionary<string, ScenarioWrapper> changedDict = new Dictionary<string, ScenarioWrapper>();
            foreach (ScenarioWrapper wrapper in changedScenariosList)
            {
                if (!changedDict.ContainsKey(wrapper.getGUID()))
                {
                    changedDict.Add(wrapper.getGUID(), wrapper);
                }
            }

            Dictionary<string, ScenarioWrapper> currentDict = new Dictionary<string, ScenarioWrapper>();
            foreach (ScenarioWrapper wrapper in currentScenarioList)
            {
                if (!currentDict.ContainsKey(wrapper.getGUID()))
                {
                    currentDict.Add(wrapper.getGUID(), wrapper);
                }
            }

            if (!((changedScenariosList.Count == currentScenarioList.Count)
                && !changedScenariosList.Except(currentScenarioList).Any()))
            {              
                // scenario delete
                foreach (KeyValuePair<string, ScenarioWrapper> scenario in currentDict)
                {
                    if (!changedDict.ContainsKey(scenario.Key))
                    {
                        EA.Scenario eaScenario = scenario.Value.scenario;
                        StepChange scenarioChange = new StepChange();
                        scenarioChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                        scenarioChange.itemGUID = GUID;
                        scenarioChange.elementType = itemTypes.getElementType(GUID);
                        scenarioChange.status = 0;
                        scenarioChange.scenarioGUID = eaScenario.ScenarioGUID;

                        changeService.saveChange(scenarioChange);
                    }
                }

                // scenario add
                foreach (KeyValuePair<string, ScenarioWrapper> scenario in changedDict)
                {
                    if (!currentDict.ContainsKey(scenario.Key))
                    {
                        // pridaj scenar
                        EA.Scenario eaScenario = scenario.Value.scenario;
                        StepChange scenarioChange = new StepChange();
                        scenarioChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                        scenarioChange.itemGUID = GUID;
                        scenarioChange.elementType = itemTypes.getElementType(GUID);
                        scenarioChange.name = eaScenario.Name;
                        scenarioChange.stepType = eaScenario.Type;
                        scenarioChange.status = 1;
                        scenarioChange.scenarioGUID = eaScenario.ScenarioGUID;
                        scenarioChange.state = eaScenario.XMLContent;

                        changeService.saveChange(scenarioChange);

                        // pridaj jeho kroky
                        /*foreach (EA.ScenarioStep step in eaScenario.Steps)
                        {
                            StepChange stepChange = new StepChange();
                            stepChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                            stepChange.itemGUID = GUID;
                            stepChange.elementType = itemTypes.getElementType(GUID);
                            stepChange.position = step.Pos;
                            stepChange.stepType = step.StepType.ToString();
                            stepChange.status = 1;
                            stepChange.name = step.Name;
                            stepChange.scenarioGUID = eaScenario.ScenarioGUID;
                            stepChange.stepGUID = step.StepGUID;
                            stepChange.uses = step.Uses;
                            stepChange.results = step.Results;
                            stepChange.state = step.State;
                            stepChange.extensionGUID = "";
                            stepChange.joiningStepGUID = "";
                            stepChange.joiningStepPosition = "";

                            foreach (EA.ScenarioExtension ext in step.Extensions)
                            {
                                stepChange.extensionGUID += ext.ExtensionGUID + ",";
                                stepChange.joiningStepGUID += ext.Join + ",";
                                stepChange.joiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                            }

                            changeService.saveChange(stepChange);
                        }*/
                    }
                    else
                    {
                        try
                        {
                            // zisti zmeny v scenari
                            EA.Scenario eaChangedScenario = scenario.Value.scenario;
                            EA.Scenario eaCurrentScenario = currentDict[scenario.Key].scenario;
                            bool wasChanged = false;

                            StepChange changedScenario = new StepChange();
                            changedScenario.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                            changedScenario.itemGUID = GUID;
                            changedScenario.elementType = itemTypes.getElementType(GUID);
                            changedScenario.status = 2;
                            changedScenario.scenarioGUID = eaChangedScenario.ScenarioGUID;
                            changedScenario.name = eaChangedScenario.Name;
                            changedScenario.stepType = eaChangedScenario.Type;
                            changedScenario.state = eaChangedScenario.XMLContent;

                            /*ScenarioChange currentScenario = new ScenarioChange();
                            currentScenario.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                            currentScenario.itemGUID = GUID;
                            currentScenario.elementType = getElementType(GUID);
                            currentScenario.name = eaCurrentScenario.Name;
                            currentScenario.type = eaCurrentScenario.Type;
                            currentScenario.status = 4;
                            currentScenario.scenarioGUID = eaCurrentScenario.ScenarioGUID;*/

                            if (eaChangedScenario.Name != eaCurrentScenario.Name)
                            {
                                wasChanged = true;
                                changedScenario.name = eaChangedScenario.Name;
                            }

                            if (eaChangedScenario.Type != eaCurrentScenario.Type)
                            {
                                wasChanged = true;
                                changedScenario.stepType = eaChangedScenario.Type;
                            }

                            if (eaChangedScenario.XMLContent != eaCurrentScenario.XMLContent)
                            {
                                wasChanged = true;
                                changedScenario.state = eaChangedScenario.XMLContent;
                            }

                            if (wasChanged)
                            {
                                //changeService.saveChange(currentScenario);
                                changeService.saveChange(changedScenario);
                            }

                            /*Dictionary<string, EA.ScenarioStep> changedSteps = new Dictionary<string, ScenarioStep>();
                            foreach (EA.ScenarioStep step in eaChangedScenario.Steps)
                            {
                                if (!changedSteps.ContainsKey(step.StepGUID))
                                {
                                    changedSteps.Add(step.StepGUID, step);
                                }
                            }

                            Dictionary<string, EA.ScenarioStep> currentSteps = new Dictionary<string, EA.ScenarioStep>();
                            foreach (EA.ScenarioStep step in currentScenarioStepList[eaCurrentScenario.ScenarioGUID])
                            {
                                if (!currentSteps.ContainsKey(step.StepGUID))
                                {
                                    currentSteps.Add(step.StepGUID, step);
                                }
                            }

                            // step delete
                            foreach (KeyValuePair<string, EA.ScenarioStep> step in currentSteps)
                            {
                                if (!changedSteps.ContainsKey(step.Key))
                                {
                                    StepChange stepChange = new StepChange();
                                    stepChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                                    stepChange.itemGUID = GUID;
                                    stepChange.elementType = itemTypes.getElementType(GUID);
                                    stepChange.status = 0;
                                    stepChange.scenarioGUID = scenario.Value.scenario.ScenarioGUID;
                                    stepChange.stepGUID = step.Value.StepGUID;

                                    changeService.saveChange(stepChange);
                                }
                            }

                            // step add
                            foreach (KeyValuePair<string, EA.ScenarioStep> stepD in changedSteps)
                            {
                                if (!currentSteps.ContainsKey(stepD.Key))
                                {
                                    EA.ScenarioStep step = stepD.Value;
                                    StepChange stepChange = new StepChange();
                                    stepChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                                    stepChange.itemGUID = GUID;
                                    stepChange.elementType = itemTypes.getElementType(GUID);
                                    stepChange.status = 1;
                                    stepChange.stepGUID = step.StepGUID;
                                    stepChange.scenarioGUID = scenario.Value.scenario.ScenarioGUID;
                                    stepChange.name = step.Name;
                                    stepChange.position = step.Pos;
                                    stepChange.stepType = step.StepType.ToString();
                                    stepChange.uses = step.Uses;
                                    stepChange.results = step.Results;
                                    stepChange.state = step.State;
                                    stepChange.extensionGUID = "";
                                    stepChange.joiningStepGUID = "";
                                    stepChange.joiningStepPosition = "";

                                    foreach (EA.ScenarioExtension ext in step.Extensions)
                                    {
                                        stepChange.extensionGUID += ext.ExtensionGUID + ",";
                                        stepChange.joiningStepGUID += ext.Join + ",";
                                        stepChange.joiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                                    }

                                    changeService.saveChange(stepChange);
                                }
                            }
                            
                            // step modify
                            foreach (KeyValuePair<string, EA.ScenarioStep> stepD in changedSteps)
                            {
                                if (currentSteps.ContainsKey(stepD.Key))
                                {
                                    wasChanged = false;
                                    EA.ScenarioStep changedStep = stepD.Value;
                                    if (!currentSteps.ContainsKey(stepD.Key))
                                    {
                                        break;
                                    }
                                    EA.ScenarioStep currentStep = currentSteps[stepD.Key]; 
                                    StepChange stepChange = new StepChange();
                                    stepChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                                    stepChange.itemGUID = GUID;
                                    stepChange.elementType = itemTypes.getElementType(GUID);
                                    stepChange.status = 2;
                                    stepChange.stepGUID = changedStep.StepGUID;
                                    stepChange.scenarioGUID = scenario.Value.scenario.ScenarioGUID;
                                    stepChange.name = currentStep.Name;
                                    stepChange.stepType = currentStep.StepType.ToString();
                                    stepChange.position = currentStep.Pos;
                                    stepChange.uses = currentStep.Uses;
                                    stepChange.results = currentStep.Results;
                                    stepChange.state = currentStep.State;

                                    if (changedStep.Name != currentStep.Name)
                                    {
                                        wasChanged = true;
                                        stepChange.name = changedStep.Name;
                                    }

                                    if (changedStep.Pos != currentStep.Pos)
                                    {
                                        wasChanged = true;
                                        stepChange.position = changedStep.Pos;
                                    }

                                    if (changedStep.StepType != currentStep.StepType)
                                    {
                                        wasChanged = true;
                                        stepChange.stepType = changedStep.StepType.ToString();
                                    }

                                    if (changedStep.Uses != currentStep.Uses)
                                    {
                                        wasChanged = true;
                                        stepChange.uses = changedStep.Uses;
                                    }

                                    if (changedStep.Results != currentStep.Results)
                                    {
                                        wasChanged = true;
                                        stepChange.results = changedStep.Results;
                                    }

                                    if (changedStep.State != currentStep.State)
                                    {
                                        wasChanged = true;
                                        stepChange.state = changedStep.State;
                                    }

                                    string changedExtensionGUID = "";
                                    string changedJoiningStepGUID = "";
                                    string changedJoiningStepPosition = "";

                                    foreach (EA.ScenarioExtension ext in changedStep.Extensions)
                                    {
                                        changedExtensionGUID += ext.ExtensionGUID + ",";
                                        changedJoiningStepGUID += ext.Join + ",";
                                        changedJoiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                                    }


                                    string otherExtensionGUID = "";
                                    string otherJoiningStepGUID = "";
                                    string otherJoiningStepPosition = "";

                                    foreach (EA.ScenarioExtension ext in currentStep.Extensions)
                                    {
                                        otherExtensionGUID += ext.ExtensionGUID + ",";
                                        otherJoiningStepGUID += ext.Join + ",";
                                        otherJoiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                                    }

                                    if (changedExtensionGUID != otherExtensionGUID)
                                    {
                                        wasChanged = true;
                                        stepChange.extensionGUID = changedExtensionGUID;
                                    }

                                    if (changedJoiningStepGUID != otherJoiningStepGUID)
                                    {
                                        wasChanged = true;
                                        stepChange.joiningStepGUID = changedJoiningStepGUID;
                                    }

                                    if (changedJoiningStepPosition != otherJoiningStepPosition)
                                    {
                                        wasChanged = true;
                                        stepChange.joiningStepPosition = changedJoiningStepPosition;
                                    }

                                    if (wasChanged)
                                    {
                                        changeService.saveChange(stepChange);
                                    }
                                }
                            }*/
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.ToString());
                        }
                    }                    
                }
            }

            // EXTENSION POINTS
            if (!currentItem.ExtensionPoints.Equals(changedElement.ExtensionPoints))
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.modelGUID = model.getWrappedModel().GetPackageByID(1).PackageGUID;
                propertyChange.itemGUID = GUID;
                propertyChange.elementType = itemTypes.getElementType(GUID);
                propertyChange.propertyType = 13;
                propertyChange.propertyBody = changedElement.ExtensionPoints;
                propertyChange.oldPropertyBody = currentItem.ExtensionPoints;

                changeService.saveChange(propertyChange);
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
    }
}
