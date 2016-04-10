using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{
    public class ItemTypes
    {
        private Wrapper.Model model;

        public ItemTypes(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
        }

        public int getElementType(string GUID)
        {
            EA.Element element = (EA.Element)model.getWrappedModel().GetElementByGuid(GUID);
            string type = model.getWrappedModel().GetElementByGuid(GUID).Type;

            switch (type)
            {
                case "Class":
                    return 0;
                case "Activity":
                    if (element.Stereotype == "process")
                    {
                        return 4;
                    }
                    return 1;
                case "UseCase":
                    return 2;
                case "Package":
                    return 3;
                case "Interface":
                    return 5;
                case "State":
                    return 6;
                case "StateNode":
                    if (element.Subtype == 3)               //Initial (statechart diagram)
                    {
                        return 7;
                    }
                    if (element.Subtype == 4)               //Final (statechart diagram)
                    {
                        return 8;
                    }
                    if (element.Subtype == 100)             //Initial (activity diagram)
                    {
                        return 9;
                    }
                    if (element.Subtype == 101)             //Final (activity diagram)
                    {
                        return 10;
                    }
                    return -1;
                case "ActivityPartition":
                    return 11;
                case "Decision":
                    return 12;
                case "MergeNode":
                    return 13;
                case "Object":
                    if (element.Stereotype == "resource")
                    {
                        return 15;
                    }
                    if (element.Stereotype == "goal")
                    {
                        return 16;
                    }
                    if (element.Stereotype == "information")
                    {
                        return 17;
                    }
                    return 14;
                case "Event":
                    if (element.Subtype == 1)           //receiver
                    {
                        return 18;
                    }
                    if (element.Subtype == 0)           //sender
                    {
                        return 19;
                    }
                    return -1;
                case "Sequence":                        //lifeline
                    return 20;
                case "InteractionFragment":
                    if (element.Subtype == 0)           //alt
                    {
                        return 21;
                    }
                    if (element.Subtype == 1)           //opt
                    {
                        return 22;
                    }
                    if (element.Subtype == 2)           //break
                    {
                        return 23;
                    }
                    if (element.Subtype == 4)           //loop
                    {
                        return 24;
                    }
                    return -1;
                case "Component":
                    return 25;
                case "Screen":
                    return 26;
                case "Actor":
                    return 30;
                case "GUIElement":
                    if (element.Stereotype == "list")
                    {
                        return 31;
                    }
                    if (element.Stereotype == "table")
                    {
                        return 32;
                    }
                    if (element.Stereotype == "text")
                    {
                        return 33;
                    }
                    if (element.Stereotype == "label")
                    {
                        return 34;
                    }
                    if (element.Stereotype == "form")
                    {
                        return 35;
                    }
                    if (element.Stereotype == "panel")
                    {
                        return 36;
                    }
                    if (element.Stereotype == "button")
                    {
                        return 37;
                    }
                    if (element.Stereotype == "combobox")
                    {
                        return 38;
                    }
                    if (element.Stereotype == "checkbox")
                    {
                        return 39;
                    }
                    if (element.Stereotype == "lcheckbox")
                    {
                        return 40;
                    }
                    if (element.Stereotype == "radio")
                    {
                        return 41;
                    }
                    if (element.Stereotype == "lradio")
                    {
                        return 42;
                    }
                    if (element.Stereotype == "vline")
                    {
                        return 43;
                    }
                    if (element.Stereotype == "hline")
                    {
                        return 44;
                    }
                    return -1;
                case "Note":
                    return 45;
                case "InteractionOccurrence":
                    return 46;
                case "Requirement":
                    return 47;
                default:
                    return -1;
            }
        }

        public int getDiagramType(string GUID)
        {
            string type = ((EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID)).Type;
            string metatype = ((EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID)).MetaType;

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
                    if (metatype == "Extended::User Interface")
                    {
                        return 58;
                    }
                    if (metatype == "Extended::Requirements")
                    {
                        return 59;
                    }
                    return -1;
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
                case "ObjectFlow":
                    return 75;
                case "ControlFlow":
                    return 76;
                case "StateFlow":
                    return 77;
                case "Sequence":
                    return 78;
                case "Dependency":
                    return 79;
                default:
                    return -1;
            }
        }

        /*public int getElementType(string GUID, EA.Repository repository)
        {
            EA.Element element = (EA.Element)repository.GetElementByGuid(GUID);
            string type = repository.GetElementByGuid(GUID).Type;

            switch (type)
            {
                case "Class":
                    return 0;
                case "Activity":
                    if (element.Stereotype == "process")
                    {
                        return 4;
                    }
                    return 1;
                case "UseCase":
                    return 2;
                case "Package":
                    return 3;
                case "Interface":
                    return 5;
                case "State":
                    return 6;
                case "StateNode":
                    if (element.Subtype == 3)               //Initial (statechart diagram)
                    {
                        return 7;
                    }
                    if (element.Subtype == 4)               //Final (statechart diagram)
                    {
                        return 8;
                    }
                    if (element.Subtype == 100)             //Initial (activity diagram)
                    {
                        return 9;
                    }
                    if (element.Subtype == 101)             //Final (activity diagram)
                    {
                        return 10;
                    }
                    return -1;
                case "ActivityPartition":
                    return 11;
                case "Decision":
                    return 12;
                case "MergeNode":
                    return 13;
                case "Object":
                    if (element.Stereotype == "resource")
                    {
                        return 15;
                    }
                    if (element.Stereotype == "goal")
                    {
                        return 16;
                    }
                    if (element.Stereotype == "information")
                    {
                        return 17;
                    }
                    return 14;
                case "Event":
                    if (element.Subtype == 1)           //receiver
                    {
                        return 18;
                    }
                    if (element.Subtype == 0)           //sender
                    {
                        return 19;
                    }
                    return -1;
                case "InteractionFragment":
                    if (element.Subtype == 0)           //alt
                    {
                        return 20;
                    }
                    if (element.Subtype == 1)           //opt
                    {
                        return 21;
                    }
                    if (element.Subtype == 2)           //break
                    {
                        return 22;
                    }
                    if (element.Subtype == 4)           //loop
                    {
                        return 23;
                    }
                    return -1;
                case "Sequence":                        //lifeline
                    return 24;
                case "Component":
                    return 25;
                case "Screen":
                    return 26;
                case "Actor":
                    return 30;
                case "GUIElement":
                    if (element.Stereotype == "list")
                    {
                        return 31;
                    }
                    if (element.Stereotype == "table")
                    {
                        return 32;
                    }
                    if (element.Stereotype == "text")
                    {
                        return 33;
                    }
                    if (element.Stereotype == "label")
                    {
                        return 34;
                    }
                    if (element.Stereotype == "form")
                    {
                        return 35;
                    }
                    if (element.Stereotype == "panel")
                    {
                        return 36;
                    }
                    if (element.Stereotype == "button")
                    {
                        return 37;
                    }
                    if (element.Stereotype == "combobox")
                    {
                        return 38;
                    }
                    if (element.Stereotype == "checkbox")
                    {
                        return 39;
                    }
                    if (element.Stereotype == "lcheckbox")
                    {
                        return 40;
                    }
                    if (element.Stereotype == "radio")
                    {
                        return 41;
                    }
                    if (element.Stereotype == "lradio")
                    {
                        return 42;
                    }
                    if (element.Stereotype == "vline")
                    {
                        return 43;
                    }
                    if (element.Stereotype == "hline")
                    {
                        return 44;
                    }
                    return -1;
                default:
                    return -1;
            }
        }

        public int getDiagramType(string GUID, EA.Repository repository)
        {
            string type = ((EA.Diagram)repository.GetDiagramByGuid(GUID)).Type;
            string metatype = ((EA.Diagram)repository.GetDiagramByGuid(GUID)).MetaType;

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
                    if (metatype == "Extended::User Interface")
                    {
                        return 58;
                    }
                    return -1;
                default:
                    return -1;
            }
        }

        public int getConnectorType(string GUID, EA.Repository repository)
        {
            EA.Connector connector = (EA.Connector)repository.GetConnectorByGuid(GUID);
            string type = ((EA.Connector)repository.GetConnectorByGuid(GUID)).Type;

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
                case "ObjectFlow":
                    return 75;
                case "ControlFlow":
                    return 76;
                case "StateFlow":
                    return 77;
                case "Sequence":
                    return 78;
                default:
                    return -1;
            }
        }*/
    }
}
