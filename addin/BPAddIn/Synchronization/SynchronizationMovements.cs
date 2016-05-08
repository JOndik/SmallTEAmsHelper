using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{
    public class SynchronizationMovements
    {
        
        public Wrapper.Model model;
        public SynchronizationMovements(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
        }

        public void moveElementOrPackageToPackage(EA.Repository repository, string itemGUID, string targetPackageGUID, int elementType)
        {
            MessageBox.Show("presun do balika");
            EA.Package targetPackage = (EA.Package)repository.GetPackageByGuid(targetPackageGUID);

            if (elementType == 3)
            {
                EA.Package package = (EA.Package)repository.GetPackageByGuid(itemGUID);
                //EA.Element package = (EA.Element)model.getWrappedModel().GetElementByGuid(itemGUID);

                if (package.ParentID != targetPackage.PackageID)
                {
                    MessageBox.Show("presun balika " + package.Name + " " + package.PackageGUID + " do balika " + 
                        targetPackage.Name + " " + targetPackage.PackageGUID);
                    package.ParentID = targetPackage.PackageID;
                    package.Update();
                    targetPackage.Packages.Refresh();
                }
            }
            else
            {
                EA.Element element = (EA.Element)repository.GetElementByGuid(itemGUID);
                /*if (element.PackageID != targetPackage.PackageID)
                {*/
                    MessageBox.Show("presun elementu " + element.Name + " " + element.ElementGUID + 
                        " do balika " + targetPackage.Name + " " + targetPackage.PackageGUID);
                    element.PackageID = targetPackage.PackageID;
                    element.Update();
                    targetPackage.Elements.Refresh();
                //}
            }           
        }

        public void moveElementToElement(EA.Repository repository, string itemGUID, string targetElementGUID)
        {
            EA.Element targetElement = (EA.Element)repository.GetElementByGuid(targetElementGUID);
            EA.Element element = (EA.Element)repository.GetElementByGuid(itemGUID);
            MessageBox.Show("presun elementu " + element.Name + " do elementu " + targetElement.Name);
            element.ParentID = targetElement.ElementID;
            element.Update();
            targetElement.Elements.Refresh();
        }

        public void moveDiagramToPackage(EA.Repository Repository, string diagramGUID, string packageGUID)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            MessageBox.Show("presun diagramu " + diagram.Name + " do balika " + package.Name);
            diagram.PackageID = package.PackageID;
            diagram.Update();
            package.Diagrams.Refresh();
        }

        public void moveDiagramToElement(EA.Repository Repository, string diagramGUID, string elementGUID)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            MessageBox.Show("presun diagramu " + diagram.Name + " " + diagram.DiagramGUID + 
                " do elementu " + element.Name + " " + element.ElementGUID);
            diagram.ParentID = element.ElementID;
            diagram.Update();
            element.Diagrams.Refresh();
        }

        public void moveElementInDiagram(EA.Repository Repository, string elementGUID, string diagramGUID, string coordinates)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);

            int left, right, top, bottom, pocet = 0;

            MessageBox.Show("zmena suradnic elementu "  + element.Name + " " + element.ElementGUID + 
               " v diagrame " + diagram.Name + " " + diagram.DiagramGUID);

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
                        MessageBox.Show("zvyseny pocet je: " + pocet);
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
                        MessageBox.Show("naslo: " + el.Name + " " + diagramObj.Sequence + " presuvany" + diagramObject.Sequence);
                        /*if (diagramObj.Sequence > 1)
                        {*/
                        diagramObj.Sequence += 1;
                        //diagramObj.Sequence += diagramObject.Sequence - diagramObj.Sequence;
                        diagramObj.Update();
                        //}
                    }
                }
            }

            diagram.DiagramObjects.Refresh();
        }


        /*public void movePackageToPackage(EA.Repository Repository, string packageGUID, string targetPackageGUID)
        {
            MessageBox.Show("presun balika do balika");
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(targetPackageGUID);
            EA.Package element = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            element.PackageGUID = package.PackageGUID;
            element.Update();
        }*/

        /*public void moveElementToPackage(EA.Repository Repository, string elementGUID, string packageGUID)
        {
            MessageBox.Show("presun elementu do balika");
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            element.PackageID = package.PackageID;
            element.Update();
        }*/

        /*public void moveElementToElement(EA.Repository Repository, string elementGUID, string targetElementGUID)
        {
            MessageBox.Show("presun elementu do elementu " + targetElementGUID);

            EA.Element targetElement = (EA.Element)Repository.GetElementByGuid(targetElementGUID);
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            element.ParentID = targetElement.ElementID;
            element.Update();
        }*/

        public void moveDiagramObject(EA.Repository Repository, string elementGUID, string diagramGUID, string coordinates)
        {

        }
    }
}
