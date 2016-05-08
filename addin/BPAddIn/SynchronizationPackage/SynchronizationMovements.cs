using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn.SynchronizationPackage
{
    public class SynchronizationMovements
    {
        
        public Wrapper.Model model;
        private ItemTypes itemTypes;

        public SynchronizationMovements(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
            this.itemTypes = new ItemTypes(repository);
        }

        public void moveElementOrPackageToPackage(EA.Repository repository, string itemGUID, string targetPackageGUID, int elementType)
        {
            EA.Package targetPackage = (EA.Package)repository.GetPackageByGuid(targetPackageGUID);

            if (elementType == 3)
            {
                EA.Package package = (EA.Package)repository.GetPackageByGuid(itemGUID);

                if (package.ParentID != targetPackage.PackageID)
                {
                    package.ParentID = targetPackage.PackageID;
                    package.Update();
                    targetPackage.Packages.Refresh();
                    BPAddIn.synchronizationWindow.addToList("Presun balíka '" + package.Name + "' do balíka '" +
                    targetPackage.Name + "'");
                }
            }
            else
            {
                EA.Element element = (EA.Element)repository.GetElementByGuid(itemGUID);

                if (element.ParentID != targetPackage.PackageID)
                {
                    element.PackageID = targetPackage.PackageID;
                    element.Update();
                    targetPackage.Elements.Refresh();

                    BPAddIn.synchronizationWindow.addToList("Presun elementu '" + element.Name + "' do balíka '" +
                    targetPackage.Name + "'");   
                }        
            }           
        }

        public void moveElementToElement(EA.Repository repository, string itemGUID, string targetElementGUID)
        {
            EA.Element targetElement = (EA.Element)repository.GetElementByGuid(targetElementGUID);
            EA.Element element = (EA.Element)repository.GetElementByGuid(itemGUID);

            if (element.ParentID != targetElement.ElementID)
            {
                element.ParentID = targetElement.ElementID;
                element.Update();
                targetElement.Elements.Refresh();

                BPAddIn.synchronizationWindow.addToList("Presun elementu '" + element.Name + "' do elementu '" +
                   targetElement.Name + "'");
            }
        }

        public void moveDiagramToPackage(EA.Repository Repository, string diagramGUID, string packageGUID)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.PackageID = package.PackageID;
            diagram.Update();
            package.Diagrams.Refresh();

            BPAddIn.synchronizationWindow.addToList("Presun diagramu '" + diagram.Name + "' do balíka '" +
               package.Name + "'");
        }

        public void moveDiagramToElement(EA.Repository Repository, string diagramGUID, string elementGUID)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.ParentID = element.ElementID;
            diagram.Update();
            element.Diagrams.Refresh();

            BPAddIn.synchronizationWindow.addToList("Presun diagramu '" + diagram.Name + "' do elementu '" +
                    element.Name + "'");
        }

        public void moveElementInDiagram(EA.Repository Repository, string elementGUID, string diagramGUID, string coordinates)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);

            int left, right, top, bottom, pocet = 0;

            Wrapper.Diagram diagramWrapper = new Wrapper.Diagram(model, diagram);
            Wrapper.ElementWrapper elWrapper = new Wrapper.ElementWrapper(model, element);
            EA.DiagramObject diagramObject = diagramWrapper.getdiagramObjectForElement(elWrapper);

            string[] coordinate;
            string str;
            string[] parts = coordinates.Split(';');

            str = parts[0];
            coordinate = str.Split('=');
            diagramObject.left = Convert.ToInt32(coordinate[1]);
            left = Convert.ToInt32(coordinate[1]);

            str = parts[1];
            coordinate = str.Split('=');
            diagramObject.right = Convert.ToInt32(coordinate[1]);
            right = Convert.ToInt32(coordinate[1]);

            str = parts[2];
            coordinate = str.Split('=');
            diagramObject.top = Convert.ToInt32(coordinate[1]);
            top = Convert.ToInt32(coordinate[1]);

            str = parts[3];
            coordinate = str.Split('=');
            diagramObject.bottom = Convert.ToInt32(coordinate[1]);
            bottom = Convert.ToInt32(coordinate[1]);

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObj = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                EA.Element el = (EA.Element)Repository.GetElementByID(diagramObj.ElementID);
                if (diagramObj.left >= left && diagramObj.right <= right && diagramObj.top <= top && diagramObj.bottom >= bottom)
                {
                    if (diagramObj.ElementID != diagramObject.ElementID)
                    {
                        pocet++;
                    }
                }
            }

            diagramObject.Sequence = 1 + pocet;
            diagramObject.Update();

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObj = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                EA.Element el = (EA.Element)Repository.GetElementByID(diagramObj.ElementID);
                if (diagramObj.left <= left && diagramObj.right >= right && diagramObj.top >= top && diagramObj.bottom <= bottom)
                {
                    if (diagramObj.ElementID != diagramObject.ElementID)
                    {
                        diagramObj.Sequence += 1;
                        diagramObj.Update();
                    }
                }
            }

            int parentID = diagram.ParentID;
            EA.Package package = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
            if (parentID == 0)
            {
                BPAddIn.synchronizationWindow.addToList("Zmena súradníc elementu '" + element.Name + "' v diagrame '" +
                    diagram.Name + "' (Umiestnenie diagramu: balík '" + package.Name + "')");
            }
            else {
                EA.Element parent = (EA.Element)Repository.GetElementByID(parentID);
                BPAddIn.synchronizationWindow.addToList("Zmena súradníc elementu '" + element.Name + "' v diagrame '" +
                    diagram.Name + "' (Umiestnenie diagramu: element '" + parent.Name + "' v balíku '" + package.Name + "')");
            }
            
            diagram.DiagramObjects.Refresh();
        }
    }
}
