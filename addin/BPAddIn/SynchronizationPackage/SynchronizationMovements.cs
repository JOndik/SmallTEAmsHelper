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

        /// <summary>
        /// method moves element or package into target package
        /// </summary>
        /// <param name="repository">EA repository</param>
        /// <param name="itemGUID">GUID of item that should be moved</param>
        /// <param name="targetPackageGUID">GUID of target package</param>
        /// <param name="elementType">type of element</param>
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
                    BPAddIn.synchronizationWindow.addToList("Movement of package '" + package.Name + "' to package '" +
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

                    BPAddIn.synchronizationWindow.addToList("Movement of element '" + element.Name + "' to package '" +
                    targetPackage.Name + "'");   
                }        
            }           
        }

        /// <summary>
        /// method moves element into target element
        /// </summary>
        /// <param name="repository">EA repository</param>
        /// <param name="itemGUID">GUID of item that should be moved</param>
        /// <param name="targetElementGUID">GUID of target element</param>
        public void moveElementToElement(EA.Repository repository, string itemGUID, string targetElementGUID)
        {
            EA.Element targetElement = (EA.Element)repository.GetElementByGuid(targetElementGUID);
            EA.Element element = (EA.Element)repository.GetElementByGuid(itemGUID);

            if (element.ParentID != targetElement.ElementID)
            {
                element.ParentID = targetElement.ElementID;
                element.Update();
                targetElement.Elements.Refresh();

                BPAddIn.synchronizationWindow.addToList("Movement of element '" + element.Name + "' to element '" +
                   targetElement.Name + "'");
            }
        }

        /// <summary>
        /// method moves diagram into target package
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of diagram that should be moved</param>
        /// <param name="packageGUID">GUID of target package</param>
        public void moveDiagramToPackage(EA.Repository Repository, string diagramGUID, string packageGUID)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.PackageID = package.PackageID;
            diagram.Update();
            package.Diagrams.Refresh();

            BPAddIn.synchronizationWindow.addToList("Movement of diagram '" + diagram.Name + "' to package '" +
               package.Name + "'");
        }

        /// <summary>
        /// method moves diagram into target element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of diagram that should be moved</param>
        /// <param name="elementGUID">GUID of target element</param>
        public void moveDiagramToElement(EA.Repository Repository, string diagramGUID, string elementGUID)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.ParentID = element.ElementID;
            diagram.Update();
            element.Diagrams.Refresh();

            BPAddIn.synchronizationWindow.addToList("Movement of diagram '" + diagram.Name + "' to element '" +
                    element.Name + "'");
        }

        /// <summary>
        /// method moves diagram object of element in diagram
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of element which diagram object that should be moved belongs to</param>
        /// <param name="diagramGUID">GUID of diagram</param>
        /// <param name="coordinates">new coordinates of diagram object that should be moved</param>
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
                BPAddIn.synchronizationWindow.addToList("Change of coordinates of element '" + element.Name + "' in diagram '" +
                    diagram.Name + "' (Location of diagram: package '" + package.Name + "')");
            }
            else {
                EA.Element parent = (EA.Element)Repository.GetElementByID(parentID);
                BPAddIn.synchronizationWindow.addToList("Change of coordinates of element '" + element.Name + "' in diagram '" +
                    diagram.Name + "' (Location of diagram: element '" + parent.Name + "' in package '" + package.Name + "')");
            }
            
            diagram.DiagramObjects.Refresh();
        }
    }
}
