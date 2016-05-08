using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{
    public class SynchronizationChanges
    {
        public Wrapper.Model model;

        public SynchronizationChanges()
        {

        }

        public SynchronizationChanges(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
        }

        public void changePackageAuthor(EA.Repository Repository, string packageGUID, string author)
        {
            MessageBox.Show("zmena autora balika");

            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Element packageElement = (EA.Element)package.Element;
            packageElement.Author = author;
            package.Update();
        }

        public void changeDiagramName(EA.Repository Repository, string diagramGUID, string name)
        {
            MessageBox.Show("zmena nazvu diagramu");
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Name = name;
            diagram.Update();
        }

        public void changeDiagramAuthor(EA.Repository Repository, string diagramGUID, string author)
        {
            MessageBox.Show("zmena autora diagramu");
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Author = author;
            diagram.Update();
        }

        public void changeDiagramNotes(EA.Repository Repository, string diagramGUID, string notes)
        {
            MessageBox.Show("zmena notes diagramu");
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Notes = notes;
            diagram.Update();
        }

        public void changeDiagramStereotype(EA.Repository Repository, string diagramGUID, string stereotype)
        {
            MessageBox.Show("zmena stereotypu diagramu");
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Stereotype = stereotype;
            diagram.Update();
        }

        public void changeElementName(EA.Repository Repository, string elementGUID, string name)
        {
            MessageBox.Show("zmena nazvu elementu");

            EA.Element element = model.getWrappedModel().GetElementByGuid(elementGUID);
            element.Name = name;
            element.Update();
        }

        public void changeElementAuthor(EA.Repository Repository, string elementGUID, string author)
        {
            MessageBox.Show("zmena autora elementu");
            EA.Element element = model.getWrappedModel().GetElementByGuid(elementGUID);
            element.Author = author;
            element.Update();
        }

        public void changeElementNotes(EA.Repository Repository, string elementGUID, string notes)
        {
            MessageBox.Show("zmena notes elementu");
            EA.Element element = model.getWrappedModel().GetElementByGuid(elementGUID);
            element.Notes = notes;
            element.Update();
        }

        public void changeElementStereotype(EA.Repository Repository, string elementGUID, string stereotype)
        {
            MessageBox.Show("zmena stereotypu elementu");
            EA.Element element = model.getWrappedModel().GetElementByGuid(elementGUID);
            element.Stereotype = stereotype;
            element.Update();
        }

        public void changeConnectorName(EA.Repository Repository, string connectorGUID, string name)
        {
            MessageBox.Show("zmena mena spojenia");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Name = name;
            connector.Update();
        }

        public void changeConnectorNotes(EA.Repository Repository, string connectorGUID, string notes)
        {
            MessageBox.Show("zmena notes spojenia");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Notes = notes;
            connector.Update();
        }

        public void changeConnectorStereotype(EA.Repository Repository, string connectorGUID, string stereotype)
        {
            MessageBox.Show("zmena stereotypu spojenia");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Stereotype = stereotype;
            connector.Update();
        }

        public void changeConnectorSource(EA.Repository Repository, string connectorGUID, string sourceGUID)
        {
            MessageBox.Show("zmena source spojenia");
            EA.Element sourceElement = (EA.Element)Repository.GetElementByGuid(sourceGUID);
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientID = sourceElement.ElementID;
            connector.Update();
        }

        public void changeConnectorTarget(EA.Repository Repository, string connectorGUID, string targetGUID)
        {
            MessageBox.Show("zmena target spojenia");
            EA.Element targetElement = (EA.Element)Repository.GetElementByGuid(targetGUID);
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierID = targetElement.ElementID;
            connector.Update();
        }

        public void changeConnectorSourceCardinality(EA.Repository Repository, string connectorGUID, string cardinality)
        {
            MessageBox.Show("zmena kardinality source");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientEnd.Cardinality = cardinality;
            connector.Update();
        }

        public void changeConnectorTargetCardinality(EA.Repository Repository, string connectorGUID, string cardinality)
        {
            MessageBox.Show("zmena kardinality target");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierEnd.Cardinality = cardinality;
            connector.Update();
        }

        public void changeConnectorGuard(EA.Repository Repository, string connectorGUID, string guard)
        {
            MessageBox.Show("zmena guard spojenia");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.TransitionGuard = guard;
            connector.Update();
        }

        public void changeConnectorDirection(EA.Repository Repository, string connectorGUID, string direction)
        {
            MessageBox.Show("zmena smeru spojenia");
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Direction = direction;
            connector.Update();
        }

        public void setExtensionPoints(EA.Repository Repository, string elementGUID, string extensionPoints)
        {
            MessageBox.Show("nastavenie bodov rozsirenia");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            element.ExtensionPoints = extensionPoints;
            element.Update();
        }

        public void changeAttributeName(EA.Repository Repository, string attributeGUID, string name)
        {
            MessageBox.Show("zmena mena atributu");
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Name = name;
            attribute.Update();
        }

        public void changeAttributeVisibility(EA.Repository Repository, string attributeGUID, string scope)
        {
            MessageBox.Show("zmena scope atributu");
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Visibility = scope;
            attribute.Update();
        }

        public void changeScenario(EA.Repository repository, string scenarioGUID, string elementGUID, string name, string type)
        {
            MessageBox.Show("zmena scenara");
            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);
            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario actualScenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (actualScenario.ScenarioGUID == scenarioGUID)
                {
                    actualScenario.Name = name;
                    actualScenario.Type = type;
                    actualScenario.Update();
                    break;
                }
            }
            element.Scenarios.Refresh();
        }

        public void changeScenarioStep(EA.Repository Repository, string elementGUID, string scenarioGUID, string stepGUID, int position, 
            string stepType, string name, string uses, string results, string state)
        {
            MessageBox.Show("zmena kroku scenara");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario scenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (scenario.ScenarioGUID == scenarioGUID)
                {
                    for (short j = 0; j < scenario.Steps.Count; j++)
                    {
                        EA.ScenarioStep scenarioStep = (EA.ScenarioStep)scenario.Steps.GetAt(i);
                        if (scenarioStep.StepGUID == stepGUID)
                        {
                            scenarioStep.Pos = position;
                            //scenarioStep.StepType = (EA.ScenarioStepType)stepType;
                            scenarioStep.Name = name;
                            scenarioStep.Uses = uses;
                            scenarioStep.Results = results;
                            scenarioStep.State = state;
                            scenarioStep.Update();
                            scenario.Steps.Refresh();
                            break;
                        }
                    }
                }
            }
            element.Scenarios.Refresh();
        }

        public void addConstraint(EA.Repository repository, string elementGUID, string name, string type)
        {
            MessageBox.Show("pridanie obmedzenia");

            int index = name.IndexOf("notes:=") + 7;

            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);
            EA.Constraint constraint = (EA.Constraint)element.Constraints.AddNew(name.Substring(0, index-8), type);
            constraint.Notes = name.Substring(index, name.Length - index);
            constraint.Update();
            element.Constraints.Refresh();
        }

        public void refresh(EA.Repository Repository)
        {
            //createForm(Repository);
            //getNotes();
            //addUserInterface(Repository);
            //verticalPartition(Repository);
            //guids(Repository, "{A8D0FAF8-0B9A-4b6e-A897-1BFCA4B0CDE5}");
            guids(Repository, "{5A3EB405-B7A0-428d-981A-9042F33F9D3D}");
            //guids2(Repository, "{B8A0AF88-2321-42d0-A31F-0A24FD80A3AD}");
            //Repository.RefreshModelView(1);
            //najdiChybu(Repository);
            //najdiChybu2(Repository);
            //strukturnaZmena(Repository);
            //zistiGUID(Repository);
            //zistiInfoSpojenie(Repository);
            //zistiInfoElement(Repository);
            //MessageBox.Show(Repository.GetPackageByID(1).PackageGUID);
            //MessageBox.Show(changeModelGUID(Repository));
            //this.pridajBalik(Repository, changeModelGUID(Repository));
        }

        public void guids2(EA.Repository repository, string GUID)
        {
            EA.Element element = (EA.Element)repository.GetElementByGuid(GUID);

            /*for (short i = 0; i < element.Elements.Count; i++)
            {
                EA.Element el = (EA.Element)element.Elements.GetAt(i);
                MessageBox.Show("Element " + el.ElementID + " " + el.Name + " " + el.ElementGUID);
                //MessageBox.Show(element.Type + " " + element.Subtype);
            }*/
            for (short i = 0; i < element.Diagrams.Count; i++)
            {
                EA.Diagram diagram = (EA.Diagram)element.Diagrams.GetAt(i);
                MessageBox.Show("Diagram " + diagram.Name + " " + diagram.DiagramGUID);
                for (short j = 0; j < diagram.DiagramObjects.Count; j++)
                {
                    EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.DiagramObjects.GetAt(j);
                    EA.Element curElement = (EA.Element)repository.GetElementByID(diagramObject.ElementID);

                    MessageBox.Show("Diagram object " + curElement.ElementID + " " + curElement.Name + " " + curElement.ElementGUID + " " +
                        diagramObject.Sequence.ToString());

                    /*for (short k = 0; k < element.Connectors.Count; k++)
                    {
                        EA.Connector conn = (EA.Connector)element.Connectors.GetAt(k);
                        MessageBox.Show("Connector " + conn.ConnectorGUID + " " + conn.Name);
                        element.Connectors.DeleteAt(k, false);
                    }*/

                }
                /*for (short j = 0; j < diagram.DiagramLinks.Count; j++)
                {
                    EA.DiagramLink diagramLink = (EA.DiagramLink)diagram.DiagramLinks.GetAt(j);
                    EA.Diagram diag = (EA.Diagram)repository.GetDiagramByID(diagramLink.DiagramID);
                    EA.Connector conn = (EA.Connector)repository.GetConnectorByID(diagramLink.ConnectorID);
                    MessageBox.Show("Spojenie " + conn.ConnectorGUID + " " + conn.ConnectorID + " Diagram " + diag.DiagramGUID);
                    diagram.DiagramLinks.DeleteAt(j, false);
                }*/
            }
        }

        public void guids(EA.Repository repository, string GUID)
        {
            EA.Package package = (EA.Package)repository.GetPackageByGuid(GUID);

            /*for (short i = 0; i < package.Elements.Count; i++)
            {
                EA.Element element = (EA.Element)package.Elements.GetAt(i);
                MessageBox.Show("Element " + element.ElementID + " " + element.Name + " " + element.ElementGUID);
                //MessageBox.Show(element.Type + " " + element.Subtype);
            }*/
            for (short i = 0; i < package.Diagrams.Count; i++)
            {
                EA.Diagram diagram = (EA.Diagram)package.Diagrams.GetAt(i);
                
                /*if (diagram.Name == "formular")
                */
                    MessageBox.Show("Diagram " + diagram.Name + " " + diagram.DiagramGUID);
                    for (short j = 0; j < diagram.DiagramObjects.Count; j++)
                    {
                        EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.DiagramObjects.GetAt(j);
                        EA.Element element = (EA.Element)repository.GetElementByID(diagramObject.ElementID);
                        
                        MessageBox.Show("Diagram object " + element.ElementID + " " + element.Name + " " + element.ElementGUID + " " +
                            diagramObject.Sequence.ToString() + " " + diagramObject.left + " " + diagramObject.right + " " + diagramObject.top +
                            " " + diagramObject.bottom);

                        /*for (short k = 0; k < element.Connectors.Count; k++)
                        {
                            EA.Connector conn = (EA.Connector)element.Connectors.GetAt(k);
                            MessageBox.Show("Connector " + conn.ConnectorGUID + " " + conn.Name);
                            element.Connectors.DeleteAt(k, false);
                        }*/

                    }
                    /*for (short j = 0; j < diagram.DiagramLinks.Count; j++)
                    {
                        EA.DiagramLink diagramLink = (EA.DiagramLink)diagram.DiagramLinks.GetAt(j);
                        EA.Diagram diag = (EA.Diagram)repository.GetDiagramByID(diagramLink.DiagramID);
                        EA.Connector conn = (EA.Connector)repository.GetConnectorByID(diagramLink.ConnectorID);
                        MessageBox.Show("Spojenie " + conn.ConnectorGUID + " " + conn.ConnectorID + " Diagram " + diag.DiagramGUID);
                        diagram.DiagramLinks.DeleteAt(j, false);
                    }*/
                //}
                
            }

        }

        public void createForm(EA.Repository repository)
        {
            /*EA.Package package = (EA.Package)repository.GetPackageByGuid("{1AA2AD71-65C5-4d4d-B570-AFAD6A9F23A4}");
            EA.Diagram newDiagram = (EA.Diagram)package.Diagrams.AddNew("Form", "User Interface");
            
            newDiagram.Update();
            package.Diagrams.Refresh();*/

            EA.Element element = model.getWrappedModel().GetElementByGuid("{1AA2AD71-65C5-4d4d-B570-AFAD6A9F23A4}");
            string text = repository.GetFormatFromField("TXT", repository.GetFieldFromFormat("RTF", element.GetLinkedDocument()));
            MessageBox.Show(element.Name + " " + text);

            //repository.GetFormatFromField("TXT", repository.GetFieldFromFormat("RTF", element.GetLinkedDocument())) = "Jano";
            element.Update();
            text = repository.GetFormatFromField("TXT", repository.GetFieldFromFormat("RTF", element.GetLinkedDocument()));
            MessageBox.Show(element.Name + " " + text);
        }

        public void getNotes()
        {
            /*string s = "sec2,notes:=poznamky";
            MessageBox.Show(s.IndexOf("notes").ToString());
            int index = s.IndexOf("notes") + 7;
            MessageBox.Show(index.ToString() + s.Length.ToString());
            MessageBox.Show(s.Substring(index, s.Length-index));*/

            string[] coordinate;
            string str = "l=539;r=629;t=-320;b=-390";
            string[] parts = str.Split(';');

            str = parts[0];
            coordinate = str.Split('=');
            MessageBox.Show(coordinate[1]);

            str = parts[1];
            coordinate = str.Split('=');
            MessageBox.Show(coordinate[1]);

            str = parts[2];
            coordinate = str.Split('=');
            MessageBox.Show(coordinate[1]);

            str = parts[3];
            coordinate = str.Split('=');
            MessageBox.Show(coordinate[1]);
        }

        public void addUserInterface(EA.Repository repository)
        {
            //EA.Package pack = model.getWrappedModel().GetPackageByGuid("{999C0D4B-8C80-4882-A2CD-56E490489A4B}");
            
            //Wrapper.Diagram diagram; diagram.stereotypes = "User Interface";
            
            EA.Package package = (EA.Package)repository.GetPackageByGuid("{999C0D4B-8C80-4882-A2CD-56E490489A4B}");

            EA.Element element = (EA.Element)package.Elements.AddNew("1", "GUIElement");
            element.Stereotype = "button";
            element.Update();

            for (short i = 0; i < package.Diagrams.Count; i++)
            {
                EA.Diagram act = (EA.Diagram)package.Diagrams.GetAt(i);
                if (act.Name == "form")
                {
                    MessageBox.Show(act.Type + " " + act.MetaType);
                    EA.DiagramObject obj = (EA.DiagramObject)act.DiagramObjects.AddNew("l=200;r=400;t=200;b=400;", "");
                    obj.ElementID = element.ElementID;
                    obj.Update();
                }
            }

            /*EA.Diagram diagram = (EA.Diagram)package.Diagrams.AddNew("form", "Extended::User Interface");
            //diagram.Stereotype = "User Interface";
            diagram.Update();
            package.Diagrams.Refresh();*/
        }

        public void verticalPartition(EA.Repository repository)
        {
            EA.Package package = (EA.Package)repository.GetPackageByGuid("{DA36F6B9-19B4-41d3-B772-D05911B5D780}");
            for (short i = 0; i < package.Elements.Count; i++)
            {
                EA.Element element = (EA.Element)package.Elements.GetAt(i);
                if (element.Type == "ActivityPartition")
                {
                    for (short j = 0; j < element.CustomProperties.Count; j++)
                    {
                        EA.CustomProperty cp = (EA.CustomProperty)element.CustomProperties.GetAt(j);
                        MessageBox.Show(cp.Name + " " + cp.Value.ToString());
                        /*cp.Value = "true";
                        element.Update();*/
                    }

                        /*EA.Properties p = (EA.Properties)element.Properties;
                        for (short j = 0; j < element.Properties.Count; j++)
                        {
                            EA.Property pr = (EA.Property)element
                            MessageBox.Show(p.Name + " " + p.Value.ToString());
                        }*/
                    //}
                }
            }
        }
    }
}
