using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    class Synchronization
    {
        public void refresh(EA.Repository Repository)
        {
            //Repository.RefreshModelView(1);
            //najdiChybu(Repository);
            //najdiChybu2(Repository);
            //strukturnaZmena(Repository);
            //zistiGUID(Repository);
            zistiInfoSpojenie(Repository);
            //zistiInfoElement(Repository);
        }

        public void zistiInfoSpojenie(EA.Repository Repository)
        {
            /*EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(1);
            EA.Collection col = (EA.Collection)diagram.DiagramLinks;

            for (short i = 0; i < col.Count; i++)
            {
                EA.DiagramLink dl = (EA.DiagramLink)col.GetAt(i);
                EA.Connector con = (EA.Connector)Repository.GetConnectorByID(dl.ConnectorID);
                if (con.Type.Equals("Sequence"))
                {
                    //MessageBox.Show(con.Subtype + " " + con.Stereotype + " " + con.RouteStyle.ToString());
                    MessageBox.Show(con.TransitionAction + " E " + con.TransitionEvent + " G " + con.TransitionGuard);
                }
            }*/
            MessageBox.Show(Repository.GetPackageByID(1).PackageGUID);
            //MessageBox.Show(Repository.ProjectGUID);
        }

        public void zistiInfoElement(EA.Repository Repository)
        {
            /*EA.Package pack = (EA.Package)Repository.GetPackageByID(2);
            EA.Collection col = (EA.Collection)pack.Elements;

            for (short i = 0; i < col.Count; i++)
            {
                EA.Element e = (EA.Element)col.GetAt(i);
                if (e.Name.Equals("Typ"))
                {
                    MessageBox.Show(e.Type + " " + e.Subtype);
                }
            }*/

            EA.Package pack = (EA.Package)Repository.GetPackageByID(23);
            EA.Collection col = (EA.Collection)pack.Diagrams;

            for (short i = 0; i < col.Count; i++)
            {
                EA.Diagram e = (EA.Diagram)col.GetAt(i);
                if (e.Name.Equals("Balik 23"))
                {
                    MessageBox.Show(e.Type + " " + e.MetaType);
                }
            }
        }


        public void najdiChybu2(EA.Repository Repository)       //+ doplnit podmienky pre final point
        {
            int count = 0;
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(11);
            EA.Collection elements = (EA.Collection)diagram.DiagramObjects;
            MessageBox.Show(elements.Count.ToString());

            for (short i = 0; i < elements.Count; i++)          //pre kazdy element v diagrame
            {
                count = 0;
                EA.DiagramObject actualElement = (EA.DiagramObject)elements.GetAt(i);
                EA.Element el = (EA.Element)Repository.GetElementByID(actualElement.ElementID);

                if (!el.Type.Equals("Decision") && !el.Type.Equals("MergeNode"))                //okrem rozhodovacich blokov
                {
                    EA.Collection coll = (EA.Collection)el.Connectors;
                    //MessageBox.Show(el.Name + " " + coll.Count.ToString());

                    if (coll.Count > 0)                                     //ak vychadza alebo vchadza z/do elementu spojenie
                    {
                        for (short j = 0; j < coll.Count; j++)
                        {
                            EA.Connector conn = (EA.Connector)coll.GetAt(j);
                            if (conn.SupplierID == actualElement.ElementID)
                            {
                                count++;
                                if (count > 1)
                                {
                                    break;
                                }
                            }
                        }
                        if (count > 1)                                      //ak vchadzaju aspon 2 spojenia do elementu
                        {
                            MessageBox.Show(el.Name + " " + el.Type);

                            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
                            EA.Element element = (EA.Element)package.Elements.AddNew("", "MergeNode");
                            element.Update();

                            EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew("l=200;r=400;t=200;b=400;", "");
                            displayElement.ElementID = element.ElementID;
                            displayElement.Update();

                            for (short k = 0; k < coll.Count; k++)
                            {
                                EA.Connector conn = (EA.Connector)coll.GetAt(k);
                                if (conn.SupplierID == actualElement.ElementID)
                                {
                                    conn.SupplierID = element.ElementID;
                                    conn.Update();
                                }
                            }
                            coll.Refresh();

                            EA.Connector con = (EA.Connector)element.Connectors.AddNew("", "ControlFlow");
                            con.SupplierID = actualElement.ElementID;
                            con.Update();
                            element.Connectors.Refresh();
                            el.Connectors.Refresh();

                            MessageBox.Show(element.Connectors.Count.ToString() + " " + el.Connectors.Count.ToString());
                        }
                    }
                }

                //MessageBox.Show(el.Name);

                //EA.Element el = (EA.Element)actualElement;
                //MessageBox.Show(actualElement);

                //EA.ObjectType actual = (EA.ObjectType)elements.GetAt(i);
            }

            //EA.Connector connectors = (EA.Connector)Repository.GetConnectorByID(12);
            //MessageBox.Show(connectors.ClientID.ToString());
        }

        public void najdiChybu(EA.Repository Repository)
        {
            int count = 0;
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(11);
            EA.Collection elements = (EA.Collection)diagram.DiagramObjects;
            MessageBox.Show(elements.Count.ToString());

            for (short i = 0; i < elements.Count; i++)          //pre kazdy element v diagrame
            {
                count = 0;
                EA.DiagramObject actualElement = (EA.DiagramObject)elements.GetAt(i);
                EA.Element el = (EA.Element)Repository.GetElementByID(actualElement.ElementID);

                if (!el.Type.Equals("Decision") && !el.Type.Equals("MergeNode"))                //okrem rozhodovacich blokov
                {
                    EA.Collection coll = (EA.Collection)el.Connectors;
                    //MessageBox.Show(el.Name + " " + coll.Count.ToString());

                    if (coll.Count > 0)                                     //ak vychadza alebo vchadza z/do elementu spojenie
                    {
                        for (short j = 0; j < coll.Count; j++)
                        {
                            EA.Connector conn = (EA.Connector)coll.GetAt(j);
                            if (conn.ClientID == actualElement.ElementID)
                            {
                                count++;
                                if (count > 1)
                                {
                                    break;
                                }
                            }
                        }
                        if (count > 1)                                      //ak vychadzaju aspon 2 spojenia z elementu
                        {
                            MessageBox.Show(el.Name + " " + el.Type);

                            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
                            EA.Element element = (EA.Element)package.Elements.AddNew("", "Decision");
                            element.Update();

                            EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew("l=200;r=400;t=200;b=400;", "");
                            displayElement.ElementID = element.ElementID;
                            displayElement.Update();

                            for (short k = 0; k < coll.Count; k++)
                            {
                                EA.Connector conn = (EA.Connector)coll.GetAt(k);
                                if (conn.ClientID == actualElement.ElementID)
                                {
                                    conn.ClientID = element.ElementID;
                                    conn.Update();
                                }
                            }
                            coll.Refresh();

                            EA.Connector con = (EA.Connector)el.Connectors.AddNew("", "ControlFlow");
                            con.SupplierID = element.ElementID;
                            con.Update();
                            el.Connectors.Refresh();
                            element.Connectors.Refresh();

                            //MessageBox.Show(element.Connectors.Count.ToString() + " " + el.Connectors.Count.ToString());
                        }
                    }
                }

                //MessageBox.Show(el.Name);

                //EA.Element el = (EA.Element)actualElement;
                //MessageBox.Show(actualElement);

                //EA.ObjectType actual = (EA.ObjectType)elements.GetAt(i);
            }

            //EA.Connector connectors = (EA.Connector)Repository.GetConnectorByID(12);
            //MessageBox.Show(connectors.ClientID.ToString());
        }

        public void strukturnaZmena(EA.Repository Repository)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid("{CB922C4E-0C5D-43d5-AAAC-4BE89A548260}");

            String guidObject = "{418758FF-42AE-4a45-BA96-4639D8100C33}";

            EA.Element element = (EA.Element)Repository.GetElementByGuid(guidObject);
            int elementID = element.ElementID;

            //EA.DiagramObject obj = (EA.DiagramObject)diagram.GetDiagramObjectByID(94, "Stav 94");
            //MessageBox.Show(obj.ObjectType.ToString());
            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject act = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                //MessageBox.Show(act.ElementID.ToString());

                if (act.ElementID == elementID)
                {
                    MessageBox.Show(act.left.ToString() + " " + act.right.ToString() + " " + act.top.ToString() + " " + act.bottom.ToString());
                    act.left += 20;
                    act.right += 20;
                    act.top -= 20;
                    act.bottom -= 20;
                    act.Update();
                }
            }
        }

        public void zistiGUID(EA.Repository Repository)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(14);
            MessageBox.Show(diagram.DiagramGUID);
        }

        public void pridajElement(EA.Repository Repository, int diagramID)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(diagramID);
            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);

            EA.Element element = (EA.Element)package.Elements.AddNew("Form", "Sequence");
            //element.Subtype = 4;

            //EA.Element element = (EA.Element)package.Elements.AddNew("Stav", "State");

            /*EA.Element partition = (EA.Element)Repository.GetElementByID(34);
            
            EA.Element element = (EA.Element)partition.Elements.AddNew("Overenie", "Activity");
            element.Stereotype = "loop";
                      
            //EA.Element element = (EA.Element)partition.Elements.AddNew("Initial", "StateNode");  zle
            //element.MiscData(100);*/

            element.Author = "Janko";
            element.Notes = "notes of element";
            element.Update();

            EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew("l=200;r=400;t=200;b=400;", "");
            displayElement.ElementID = element.ElementID;
            displayElement.Update();

            MessageBox.Show(diagram.Name + " " + package.Name + " " + element.ElementID.ToString() + " " + displayElement.ElementID.ToString());
        }

        public void pridajSpojenie(EA.Repository Repository, int element1ID, int element2ID)
        {
            EA.Element source = (EA.Element)Repository.GetElementByID(element1ID);
            MessageBox.Show(source.Name + " " + source.ElementID.ToString());
            EA.Element target = (EA.Element)Repository.GetElementByID(element2ID);
            MessageBox.Show(target.Name + " " + target.ElementID.ToString());

            EA.Connector con = (EA.Connector)source.Connectors.AddNew("", "Sequence");
            con.TransitionAction = "Call";   // Control Flow Type – Kind
            con.TransitionEvent = "Asynchronous";   //Control Flow Type – Synch
            con.Name = "message";
            con.TransitionGuard = "retval=void;paramsDlg=int cislo"; 	//Signature - Parameters

            con.SupplierID = element2ID;
            con.Update();
            source.Connectors.Refresh();
            MessageBox.Show(con.ConnectorID.ToString());

            //zmena target pri spojeni                                                              OK
            /*EA.Connector con = (EA.Connector)Repository.GetConnectorByID(3);
            con.SupplierID = 17;
            con.Update();
            MessageBox.Show(con.ConnectorID.ToString());*/

            //pridanie extension point
            //EA.Element target = (EA.Element)Repository.GetElementByID(element2ID);

            //target.Update();
        }

        public void presunElement(EA.Repository Repository, int elementID, int packageID)
        {
            EA.Element element = (EA.Element)Repository.GetElementByID(elementID);
            element.PackageID = packageID;
            element.Update();
        }

        public void pridajBalik(EA.Repository Repository, int packageID)
        {
            EA.Collection packages = (EA.Collection)Repository.GetPackageByID(1).Packages;
            EA.Package newPackage = (EA.Package)packages.AddNew("Balik", "Nothing");
            newPackage.Notes = "Notes of package";
            newPackage.Update();

            EA.Element packageMetaElement = (EA.Element)newPackage.Element;
            packageMetaElement.Author = "Janko";
            //packageMetaElement.Notes = "Notes are written here";
            newPackage.Update();

            packages.Refresh();
            MessageBox.Show(newPackage.PackageID.ToString() + "   " + newPackage.PackageGUID);
        }

        public void pridajDiagram(EA.Repository Repository, int packageID)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByID(packageID);
            //EA.Diagram newDiagram = (EA.Diagram)package.Diagrams.AddNew("Sequence Diagram", "Sequence");
            EA.Diagram newDiagram = (EA.Diagram)package.Diagrams.AddNew("Formular", "Custom");
            newDiagram.Author = "Janko";
            newDiagram.Notes = "Notes of diagram";
            newDiagram.Update();

            MessageBox.Show(newDiagram.DiagramID.ToString() + " " + newDiagram.PackageID.ToString());
        }

        public void presunBalik(EA.Repository Repository, int packageID, int destination)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByID(packageID);
            //EA.Package parentPackage = (EA.Package)Repository.GetPackageByID(package.ParentID);

            package.ParentID = destination;
            package.Update();

            /*EA.Package destPackage = (EA.Package)Repository.GetPackageByID(destination);

            EA.Collection packages = (EA.Collection)destPackage.Packages;
            packages.Refresh();

            packages = (EA.Collection)parentPackage.Packages;
            packages.Refresh();*/
        }

        public void presunDiagram(EA.Repository Repository, int diagramID, int destination)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(diagramID);
            /*EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
            EA.Package destPackage = (EA.Package)Repository.GetPackageByID(destination);
            EA.Collection diagrams = (EA.Collection)package.Diagrams;
            EA.Collection destDiagrams = (EA.Collection)destPackage.Diagrams;*/

            //int id = diagram.PackageID;
            diagram.PackageID = destination;
            diagram.Update();

            //Repository.RefreshModelView(destination);
            //Repository.RefreshModelView(id);
            //destDiagrams.Refresh();
            //diagrams.Refresh();
        }

        public void zmazBalik(EA.Repository Repository, int packageID)
        {
            EA.Package packageToDelete = (EA.Package)Repository.GetPackageByID(packageID);
            EA.Package parentPackage = (EA.Package)Repository.GetPackageByID(packageToDelete.ParentID);
            //MessageBox.Show(packageToDelete.PackageID.ToString() + " " + parentPackage.PackageID.ToString());
            //MessageBox.Show(packageToDelete.Name + " " + parentPackage.Name);

            for (short i = 0; i < parentPackage.Packages.Count; i++)
            {
                EA.Package actualPackage = (EA.Package)parentPackage.Packages.GetAt(i);
                if (actualPackage.PackageID == packageID)
                {
                    //MessageBox.Show(actualPackage.PackageID.ToString() + " " + packageID);
                    parentPackage.Packages.DeleteAt(i, false);
                    break;
                }
            }
            //parentPackage.Packages.Refresh();  
        }

        public void zmazDiagram(EA.Repository Repository, int diagramID)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(diagramID);
            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
            //MessageBox.Show(diagramID.ToString() + " " + diagram.Name + " " + package.Name);
            for (short i = 0; i < package.Diagrams.Count; i++)
            {
                EA.Diagram actualDiagram = (EA.Diagram)package.Diagrams.GetAt(i);
                if (actualDiagram.DiagramID == diagramID)
                {
                    package.Diagrams.DeleteAt(i, false);
                    break;
                }
            }
            //package.Diagrams.Refresh();
        }

        public void zmenBalik(EA.Repository Repository, int packageID)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByID(10);
            package.Name = "Zmeneny nazov";
            package.Update();
        }

        public void zmenDiagram(EA.Repository Repository, int diagramID)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(diagramID);
            diagram.Name = "Zmeneny nazov";
            //diagram.Author = "Martin";
            diagram.Update();
        }

        public void pridajTriedu(EA.Repository Repository, int diagramID)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByID(diagramID);
            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
            EA.Element element = (EA.Element)package.Elements.AddNew("ReferenceType", "Class");
            element.Update();

            EA.IDualDiagramObject displayElement = (EA.IDualDiagramObject)diagram.DiagramObjects.AddNew("l=200;r=400;t=200;b=400;", "");
            displayElement.ElementID = element.ElementID;
            displayElement.Update();

            MessageBox.Show(element.ElementID.ToString());
        }

        public void najdiMenoPodlaID(EA.Repository Repository)
        {
            EA.Package pack = (EA.Package)Repository.GetPackageByID(1);
            MessageBox.Show(pack.Name);
        }
    }
}
