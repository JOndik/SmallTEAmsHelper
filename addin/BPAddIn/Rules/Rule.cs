using BPAddInTry;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{
    public class Rule
    {
        public string name { get; set; }
        public string elementType { get; set; }
        public string elementStereotype { get; set; }
        public string attributeType { get; set; }
        public string attributeStereotype { get; set; }
        public string contentDefectMsg { get; set; }
        public string contentValid { get; set; }
        public string contentCorrect { get; set; }
        public string contentCond { get; set; }


        public string activate(object element, Model model)
        {
            string elStereotype = "";

            if (!(element is EA.Diagram))
            {
                elStereotype = getStringAttributeValue(element, "Stereotype");
            }

            if (elementStereotype == "-" && elStereotype != "")
            {
                return "";
            }
            else if (elementStereotype != elStereotype && elementStereotype != "*")
            {
                return "";
            }

            string[] validCall = contentValid.Split('.');

            if (contentValid.IndexOf('=') != -1)
            {
                validCall = contentValid.Split('=');
            }

            Word word = null;
            Dictionary dict = null;

            for (int i = 0; i < validCall.Length; i++)
            {
                try
                {
                    if (validCall[i] == "dict")
                    {
                        dict = new Dictionary();
                        if (validCall[i + 1].StartsWith("getWord"))
                        {
                            string value = getStringAttributeValue(element, validCall[i + 1].Split('(', ')')[1]);
                            word = dict.getWord(value);
                        }
                        i++;
                        continue;
                    }

                    if (word != null)
                    {
                        if (validCall[i].StartsWith("isNoun"))
                        {
                            return dict.isNoun(word.word.Split(' ')) ? "" : createDescription(element, getStringAttributeValue(element, attributeType));
                        }
                    }

                    if (validCall[i] == "getName")
                    {
                        if (contentCond != null)
                        {
                            string[] condCall = contentCond.Split('&');
                            bool rslt = true;
                            for (int j = 0; j < condCall.Length; j++)
                            {
                                if (condCall[j].StartsWith("Connector"))
                                {
                                    string expr = condCall[j].Split('.')[1];
                                    if (expr.StartsWith("getSrcType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "src", ownerType);
                                    }
                                    else if (expr.StartsWith("getTargetType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "target", ownerType);
                                    }
                                }
                            }

                            if (!rslt)
                            {
                                return "";
                            }
                        }
                        
                        if (String.IsNullOrEmpty(getStringAttributeValue(element, "Name")))
                        {
                            if (element is EA.Connector)
                            {
                                return createDescription(element, getConnectorOwnerName(element, model, "src"), getConnectorOwnerName(element, model, "target"));
                            }
                            else
                            {
                                return createDescription(element);
                            }
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i] == "getCardinalities")
                    {
                        if (contentCond != null)
                        {
                            string[] condCall = contentCond.Split('&');
                            bool rslt = true;
                            for (int j = 0; j < condCall.Length; j++)
                            {
                                if (condCall[j].StartsWith("Connector"))
                                {
                                    string expr = condCall[j].Split('.')[1];
                                    if (expr.StartsWith("getSrcType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "src", ownerType);
                                    }
                                    else if (expr.StartsWith("getTargetType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "target", ownerType);
                                    }
                                }
                            }

                            if (!rslt)
                            {
                                return "";
                            }
                        }

                        object objectClient = getAttributeValue(element, "ClientEnd");
                        string cardinalityClient = getStringAttributeValue(objectClient, "Cardinality");

                        object objectSupplier = getAttributeValue(element, "SupplierEnd");
                        string cardinalitySupplier = getStringAttributeValue(objectSupplier, "Cardinality");

                        if (String.IsNullOrEmpty(cardinalityClient) || String.IsNullOrEmpty(cardinalitySupplier))
                        {
                            return createDescription(element, getConnectorOwnerName(element, model, "src"), getConnectorOwnerName(element, model, "target"));
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i] == "getExtensionPointsCount")
                    {
                        int extensionPointsCount = getStringAttributeValue(element, attributeType).Count(c => c == ',');
                        int rightSide = 0;

                        if (!String.IsNullOrEmpty(getStringAttributeValue(element, attributeType)) && !getStringAttributeValue(element, attributeType).EndsWith(","))
                        {
                            extensionPointsCount++;
                        }

                        if (validCall[i + 1].StartsWith("Connector"))
                        {
                            string expr = validCall[i + 1].Split('.')[1];

                            if (expr.StartsWith("getStereotypeCount"))
                            {
                                string stereotype = expr.Split('(', ')')[1];

                                foreach (EA.Connector con in (EA.Collection)getAttributeValue(element, "Connectors"))
                                {
                                    if (stereotype == con.Stereotype)
                                    {
                                        rightSide++;
                                    }
                                }
                            }
                        }

                        if (contentValid.IndexOf('=') != -1)
                        {
                            if (extensionPointsCount < rightSide)
                            {
                                return createDescription(element, getStringAttributeValue(element, "Name"));
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }

                    if (validCall[i] == "getTransitionGuard")
                    {
                        if (contentCond != null)
                        {
                            string[] condCall = contentCond.Split('&');
                            bool rslt = true;
                            for (int j = 0; j < condCall.Length; j++)
                            {
                                if (condCall[j].StartsWith("Connector"))
                                {
                                    string expr = condCall[j].Split('.')[1];
                                    if (expr.StartsWith("getSrcType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "src", ownerType);
                                    }
                                    else if (expr.StartsWith("getTargetType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "target", ownerType);
                                    }
                                }
                            }

                            if (!rslt)
                            {
                                return "";
                            }
                        }

                        if (String.IsNullOrEmpty(getStringAttributeValue(element, attributeType)))
                        {
                            return createDescription(element, getConnectorOwnerName(element, model, "target"));
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i].StartsWith("getSrcControlFlows"))
                    {                     
                        if (contentCond != null)
                        {
                            string[] condCall = contentCond.Split('&');
                            bool rslt = true;
                            for (int j = 0; j < condCall.Length; j++)
                            {
                                if (condCall[j].StartsWith("Connector"))
                                {
                                    string expr = condCall[j].Split('.')[1];
                                    if (expr.StartsWith("getSrcType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "src", ownerType);
                                    }
                                    else if (expr.StartsWith("getTargetType"))
                                    {
                                        string ownerType = expr.Split('(', ')')[1];
                                        rslt &= checkConnectorOwnerType(element, model, "target", ownerType);
                                    }
                                }

                                if (condCall[j].StartsWith("Element"))
                                {
                                    string expr = condCall[j].Split('.')[1];

                                    if (expr.StartsWith("getType"))
                                    {
                                        string elType = expr.Split('(', ')')[1];
                                        rslt &= getStringAttributeValue(element, "Type") == elType;
                                    }
                                }
                            }

                            if (!rslt)
                            {
                                return "";
                            }
                        }

                        int numControlFlows = Convert.ToInt32(validCall[i].Split('(', ')')[1]);
                        EA.Collection collection = (EA.Collection)getAttributeValue(getConnectorOwner(element, model, "src"), "Connectors");

                        if (numControlFlows < collection.Count)
                        {
                            return createDescription(element, getStringAttributeValue(getConnectorOwner(element, model, "src"), "Name"));
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i] == "checkInitialNode")
                    {
                        bool rslt = false;

                        EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();

                        foreach (EA.DiagramObject diagramObject in diagram.DiagramObjects)
                        {
                            EA.Element el = model.getWrappedModel().GetElementByID(diagramObject.ElementID);
                            if (el.Type == "StateNode" && el.Subtype == 100)
                            {
                                rslt = true;
                            }
                        }

                        if (!rslt)
                        {
                            return createDescription(element, getStringAttributeValue(diagram, "Name"));
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i] == "checkFinalNode")
                    {
                        bool rslt = false;

                        EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();

                        foreach (EA.DiagramObject diagramObject in diagram.DiagramObjects)
                        {
                            EA.Element el = model.getWrappedModel().GetElementByID(diagramObject.ElementID);
                            if (el.Type == "StateNode" && el.Subtype == 101)
                            {
                                rslt = true;
                            }
                        }

                        if (!rslt)
                        {
                            return createDescription(element, getStringAttributeValue(diagram, "Name"));
                        }
                        else
                        {
                            return "";
                        }
                    }

                    if (validCall[i] == "checkIncludeInScenario")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");
                        
                        if (contentCond.StartsWith("Scenario"))
                        {
                            string expr = contentCond.Split('.')[1];

                            if (expr == "hasAny")
                            {
                                if (scenarios.Count == 0)
                                {
                                    return "";
                                }
                            }
                        }

                        if (scenarios.Count > 0)
                        {
                            EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                            XElement xElement = XElement.Parse(scenario.XMLContent);

                            try
                            {
                                EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                                foreach (EA.Connector con in connectors)
                                {
                                    if (con.Stereotype == "include" && con.ClientID == Convert.ToInt32(getAttributeValue(element, "ElementID")))
                                    {
                                        string targetName = getConnectorOwnerName(con, model, "target");

                                        IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                                      where ((string)el.Attribute("name")).ToLowerInvariant().Contains(targetName.ToLowerInvariant())
                                                                      select el;

                                        try
                                        {
                                            if (!steps.Any())
                                            {
                                                return createDescription(element, getStringAttributeValue(element, "Name"), targetName);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            return createDescription(element, getStringAttributeValue(element, "Name"), targetName);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { };
                        }
                    }

                    if (validCall[i] == "checkExtendInScenario")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");

                        if (contentCond.StartsWith("Scenario"))
                        {
                            string expr = contentCond.Split('.')[1];

                            if (expr == "hasAny")
                            {
                                if (scenarios.Count == 0)
                                {
                                    return "";
                                }
                            }
                        }

                        if (scenarios.Count > 0)
                        {
                            EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                            XElement xElement = XElement.Parse(scenario.XMLContent);

                            try
                            {
                                EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                                foreach (EA.Connector con in connectors)
                                {
                                    if (con.Stereotype == "extend" && con.SupplierID == Convert.ToInt32(getAttributeValue(element, "ElementID")))
                                    {
                                        string targetName = getConnectorOwnerName(con, model, "src");

                                        IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                                      where ((string)el.Attribute("name")).ToLowerInvariant().Contains(targetName.ToLowerInvariant())
                                                                      select el;

                                        try
                                        {
                                            if (!steps.Any())
                                            {
                                                return createDescription(element, getStringAttributeValue(element, "Name"), targetName);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            return createDescription(element, getStringAttributeValue(element, "Name"), targetName);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { };
                        }
                    }

                    if (validCall[i] == "checkIncludeDependency")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");

                        if (contentCond.StartsWith("Scenario"))
                        {
                            string expr = contentCond.Split('.')[1];

                            if (expr == "hasAny")
                            {
                                if (scenarios.Count == 0)
                                {
                                    return "";
                                }
                            }
                        }

                        if (scenarios.Count > 0)
                        {
                            EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                            XElement xElement = XElement.Parse(scenario.XMLContent);


                            IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                          where ((string)el.Attribute("name")).ToLowerInvariant().Contains("<include>")
                                                          select el;

                            try
                            {
                                EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                                bool rslt = false;

                                foreach (XElement e in steps)
                                {
                                    string name = (string)e.Attribute("name");

                                    if (findElementInCurDiagramtByName(model, name.Substring(name.IndexOf("UC"))) == null)
                                    {
                                        continue;
                                    }

                                    foreach (EA.Connector con in connectors)
                                    {
                                        if (con.Stereotype == "include" && con.ClientID == Convert.ToInt32(getAttributeValue(element, "ElementID")))
                                        {
                                            string targetName = getConnectorOwnerName(con, model, "target");

                                            rslt |= targetName == name.Substring(name.IndexOf("UC"));
                                        }
                                    }

                                    if (!rslt)
                                    {
                                        return createDescription(element, getStringAttributeValue(element, "Name"), name.Substring(name.IndexOf("UC")));
                                    }
                                }
                            }
                            catch (Exception ex) { };
                        }
                    }
                }
                catch (Exception ex) { }
            }

            return "";
        }

        /// <summary>
        /// Correct detected defect based on value in contentCorrect attribute
        /// </summary>
        /// <param name="element">Defected element</param>
        /// <param name="model">Wrapper EA.Repository object</param>
        /// <returns>true if correction was successful, else false</returns>
        public bool correct(object element, Model model)
        {
            string[] correctCall = contentCorrect.Split('@');

            for (int i = 0; i < correctCall.Length; i++)
            {
                try
                {
                    if (correctCall[i].StartsWith("setName"))
                    {
                        string[] nameExpression = correctCall[i].Split('(', ')')[1].Split('.');
                        for (int j = 0; j < nameExpression.Length; j++)
                        {
                            if (nameExpression[j] == "dict")
                            {
                                Dictionary dict = new Dictionary();
                                if (nameExpression[j + 1].StartsWith("getBaseNoun"))
                                {
                                    string attributeName = correctCall[i].Split('(', ')')[2];
                                    string value = getStringAttributeValue(element, attributeName);
                                    string word = dict.getBaseNoun(value);

                                    if (word == "")
                                    {
                                        return false;
                                    }

                                    setStringAttributeValue(element, attributeName, word);
                                    try
                                    {
                                        model.adviseChange((new ElementWrapper(model, (EA.Element)element)));
                                        RuleService.active.Remove(getStringAttributeValue(element, "GUID"));
                                    }
                                    catch (Exception ex) { }

                                    return true;
                                }
                            }
                            else
                            {
                                setStringAttributeValue(element, attributeType, nameExpression[j]);
                                try
                                {
                                    if (element is EA.Connector)
                                    {
                                        model.adviseChange(new ConnectorWrapper(model, (EA.Connector)element));
                                    }
                                    else
                                    {
                                        model.adviseChange(new ElementWrapper(model, (EA.Element)element));
                                    }
                                }
                                catch (Exception ex) { }

                                return true;
                            }
                        }
                    }


                    if (correctCall[i].StartsWith("setCardinalities"))
                    {
                        string nameExpression = correctCall[i].Split('(', ')')[1];

                        object objectClient = getAttributeValue(element, "ClientEnd");
                        string cardinalityClient = getStringAttributeValue(objectClient, "Cardinality");

                        object objectSupplier = getAttributeValue(element, "SupplierEnd");
                        string cardinalitySupplier = getStringAttributeValue(objectSupplier, "Cardinality");

                        if (String.IsNullOrEmpty(cardinalityClient))
                        {
                            setStringAttributeValue(objectClient, "Cardinality", nameExpression);
                        }

                        if (String.IsNullOrEmpty(cardinalitySupplier))
                        {
                            setStringAttributeValue(objectSupplier, "Cardinality", nameExpression);
                        }

                        try
                        { 
                            model.adviseChange(new ConnectorWrapper(model, (EA.Connector)element));
                        }
                        catch (Exception ex) { }
                    }

                    if (correctCall[i].StartsWith("setExtensionPointsCount"))
                    {
                        string nameExpression = correctCall[i].Split('(', ')')[1];
                        if (nameExpression.StartsWith("Connector"))
                        {
                            string expr = nameExpression.Split('.')[1];
                            if (expr == "getStereotypeCount")
                            {
                                string stereotype = correctCall[i].Split('(', ')')[2];
                                int stereotypeCount = 0;

                                foreach (EA.Connector con in (EA.Collection)getAttributeValue(element, "Connectors"))
                                {
                                    if (stereotype == con.Stereotype)
                                    {
                                        stereotypeCount++;
                                    }
                                }

                                string extensionPoints = getStringAttributeValue(element, attributeType);
                                int num = Math.Abs(extensionPoints.Count(c => c == ',') - stereotypeCount);

                                for (int k = 0; k < num; k++)
                                {
                                    extensionPoints += "," + k + 1 + " extension point";
                                }

                                setStringAttributeValue(element, attributeType, extensionPoints);

                                try
                                {
                                    model.adviseChange((new ElementWrapper(model, (EA.Element)element)));
                                    RuleService.active.Remove(getStringAttributeValue(element, "GUID"));
                                }
                                catch (Exception ex) { }

                                return true;
                            }
                        }
                    }

                    if (correctCall[i].StartsWith("setTransitionGuard"))
                    {
                        string nameExpression = correctCall[i].Split('(', ')')[1];

                        setStringAttributeValue(element, attributeType, nameExpression);

                        try
                        {
                            model.adviseChange(new ConnectorWrapper(model, (EA.Connector)element));
                        }
                        catch (Exception ex) { }
                    }

                    if (correctCall[i] == "createDecisionNode")
                    {
                        object owner = getConnectorOwner(element, model, "src");
                        object target = getConnectorOwner(element, model, "target");

                        EA.Collection collection = (EA.Collection)getAttributeValue(owner, "Connectors");
                        EA.Collection elements = model.getWrappedModel().GetPackageByID(Convert.ToInt32(getStringAttributeValue(owner, "PackageID"))).Elements;
                        EA.Collection connectors = model.getWrappedModel().GetPackageByID(Convert.ToInt32(getStringAttributeValue(owner, "PackageID"))).Connectors;

                        EA.Element decisionNode = (EA.Element)elements.AddNew("", "Decision");
                        decisionNode.Update();
                        elements.Refresh();

                        EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();
                        EA.DiagramObject ownerDO = new Diagram(model, diagram).getdiagramObjectForElement(new ElementWrapper(model, (EA.Element)owner));

                        int l = ownerDO.left;
                        int b = ownerDO.bottom;
                        string coordinates = "l=" + (l + 20) + "r=" + (l + 60) + "t=" + (b - 50) + "b=" + (b - 90) + ";";

                        EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew(coordinates, "");

                        displayElement.ElementID = decisionNode.ElementID;
                        displayElement.Sequence = 1;
                        displayElement.Update();
                        diagram.DiagramObjects.Refresh();
                        diagram.Update();

                        EA.Connector connector = (EA.Connector)connectors.AddNew("", "ControlFlow");
                        connectors.Refresh();

                        connector.ClientID = Convert.ToInt32(getStringAttributeValue(owner, "ElementID"));
                        connector.SupplierID = decisionNode.ElementID;
                        connector.Update();

                        foreach (EA.Connector con in (EA.Collection)getAttributeValue(owner, "Connectors"))
                        {
                            con.ClientID = decisionNode.ElementID;
                            con.TransitionGuard = "podmienka";
                            con.Update();
                        }

                        model.refreshDiagram(new Diagram(model, diagram));

                        return true;
                    }

                    if (correctCall[i] == "createInitialNode")
                    {
                        EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();
                        EA.Collection elements = model.getWrappedModel().GetPackageByID(Convert.ToInt32(getStringAttributeValue(diagram, "PackageID"))).Elements;

                        EA.Element initialNode = (EA.Element)elements.AddNew("", "StateNode");
                        initialNode.Subtype = 100;
                        initialNode.Name = "pociatocny uzol";
                        initialNode.Update();
                        elements.Refresh();

                        EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew("l=0;r=0;t=0;b=0;", "");
                        displayElement.ElementID = initialNode.ElementID;
                        displayElement.Sequence = 1;
                        displayElement.Update();
                        diagram.DiagramObjects.Refresh();
                        diagram.Update();

                        model.refreshDiagram(new Diagram(model, diagram));

                        return true;
                    }

                    if (correctCall[i] == "createFinalNode")
                    {
                        EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();
                        EA.Collection elements = model.getWrappedModel().GetPackageByID(Convert.ToInt32(getStringAttributeValue(diagram, "PackageID"))).Elements;

                        EA.Element initialNode = (EA.Element)elements.AddNew("", "StateNode");
                        initialNode.Subtype = 101;
                        initialNode.Name = "koncovy uzol";
                        initialNode.Update();
                        elements.Refresh();

                        EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew("l=0;r=0;t=0;b=0;", "");
                        displayElement.ElementID = initialNode.ElementID;
                        displayElement.Sequence = 1;
                        displayElement.Update();
                        diagram.DiagramObjects.Refresh();
                        diagram.Update();

                        model.refreshDiagram(new Diagram(model, diagram));

                        return true;
                    }

                    if (correctCall[i] == "addIncludeToScenario")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");
                        EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                        XElement xElement = XElement.Parse(scenario.XMLContent);

                        try
                        {
                            EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                            IEnumerable<XElement> max = from el in xElement.Elements("step") select el;
                            int maxLevel = max.Max(x => Convert.ToInt32((string)x.Attribute("level")));

                            foreach (EA.Connector con in connectors)
                            {
                                if (con.Stereotype == "include")
                                {
                                    string targetName = getConnectorOwnerName(con, model, "target");

                                    IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                                  where ((string)el.Attribute("name")).Contains(targetName)
                                                                  select el;
                                    try
                                    {
                                        steps.First();
                                    }
                                    catch (Exception ex)
                                    {
                                        xElement.Add(new XElement("step", new XAttribute("name", "<include> " + targetName), new XAttribute("guid", "{" + Guid.NewGuid().ToString().ToUpper() + "}"),
                                            new XAttribute("level", maxLevel + 1), new XAttribute("uses", ""), new XAttribute("useslist", ""), new XAttribute("result", ""), new XAttribute("state", ""),
                                            new XAttribute("trigger", "0"), new XAttribute("link", "")));

                                        scenario.XMLContent = xElement.ToString();
                                        scenario.Update();

                                        model.adviseChange(new ElementWrapper(model, (EA.Element)element));

                                        return true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }

                    if (correctCall[i] == "addExtendToScenario")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");
                        EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                        XElement xElement = XElement.Parse(scenario.XMLContent);

                        try
                        {
                            EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                            IEnumerable<XElement> max = from el in xElement.Elements("step") select el;
                            int maxLevel = max.Max(x => Convert.ToInt32((string)x.Attribute("level")));

                            foreach (EA.Connector con in connectors)
                            {
                                if (con.Stereotype == "extend")
                                {
                                    string sourceName = getConnectorOwnerName(con, model, "src");

                                    IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                                  where ((string)el.Attribute("name")).Contains(sourceName)
                                                                  select el;
                                    try
                                    {
                                        steps.First();
                                    }
                                    catch (Exception ex)
                                    {
                                        xElement.Add(new XElement("step", new XAttribute("name", "<extend> " + sourceName), new XAttribute("guid", "{" + Guid.NewGuid().ToString().ToUpper() + "}"),
                                            new XAttribute("level", maxLevel + 1), new XAttribute("uses", ""), new XAttribute("useslist", ""), new XAttribute("result", ""), new XAttribute("state", ""),
                                            new XAttribute("trigger", "0"), new XAttribute("link", "")));

                                        scenario.XMLContent = xElement.ToString();
                                        scenario.Update();

                                        model.adviseChange(new ElementWrapper(model, (EA.Element)element));

                                        return true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }

                    if (correctCall[i] == "addIncludeDependency")
                    {
                        EA.Collection scenarios = (EA.Collection)getAttributeValue(element, "Scenarios");
                        EA.Scenario scenario = (EA.Scenario)scenarios.GetAt(0);
                        XElement xElement = XElement.Parse(scenario.XMLContent);


                        IEnumerable<XElement> steps = from el in xElement.Elements("step")
                                                      where ((string)el.Attribute("name")).ToLowerInvariant().Contains("<include>")
                                                      select el;

                        try
                        {
                            EA.Collection connectors = (EA.Collection)getAttributeValue(element, "Connectors");
                            bool rslt = false;

                            foreach (XElement e in steps)
                            {
                                string name = (string)e.Attribute("name");

                                if (findElementInCurDiagramtByName(model, name.Substring(name.IndexOf("UC"))) == null)
                                {
                                    continue;
                                }

                                foreach (EA.Connector con in connectors)
                                {
                                    if (con.Stereotype == "include" && con.ClientID == Convert.ToInt32(getAttributeValue(element, "ElementID")))
                                    {
                                        string targetName = getConnectorOwnerName(con, model, "target");

                                        rslt |= targetName == name.Substring(name.IndexOf("UC"));
                                    }
                                }

                                if (!rslt)
                                {
                                    EA.Element el = findElementInCurDiagramtByName(model, name.Substring(name.IndexOf("UC")));
                                    EA.Collection connectorsPcg = model.getWrappedModel().GetPackageByID(Convert.ToInt32(getStringAttributeValue(element, "PackageID"))).Connectors;

                                    EA.Connector connector = (EA.Connector)connectors.AddNew("", "Dependency");
                                    connector.Stereotype = "include";
                                    connector.ClientID = Convert.ToInt32(getStringAttributeValue(element, "ElementID"));
                                    connector.SupplierID = el.ElementID;
                                    connector.Update();
                                    connectors.Refresh();

                                    model.refreshDiagram(new Diagram(model, model.getWrappedModel().GetCurrentDiagram()));
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex) { };
                    }
                }
                catch (Exception ex) { }
            }

            return false;
        }

        public object getAttributeValue(object element, string attributeName)
        {
            return element.GetType().GetProperty(attributeName).GetValue(element);
        }

        public void setAttributeValue(object element, string attributeName, object value)
        {
            element.GetType().GetProperty(attributeName).SetValue(element, value);
            element.GetType().GetMethod("Update").Invoke(element, null);
        }

        public string getStringAttributeValue(object element, string attributeName)
        {
            return element.GetType().GetProperty(attributeName).GetValue(element).ToString();
        }

        public void setStringAttributeValue(object element, string attributeName, string value)
        {
            element.GetType().GetProperty(attributeName).SetValue(element, value);
            element.GetType().GetMethod("Update").Invoke(element, null);
        }

        public string createDescription(object element, params string[] attr)
        {
            switch (attr.Length)
            {
                case 0:
                    return contentDefectMsg;
                case 1:
                    return String.Format(contentDefectMsg, attr[0]);
                case 2:
                    return String.Format(contentDefectMsg, attr[0], attr[1]);
            }

            return "";
        }

        public bool checkConnectorOwnerType(object connector, Model model, string type, string owner)
        {
            ConnectorWrapper connectorWrapper = new ConnectorWrapper(model, (EA.Connector)connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.owner;
            EA.Element element;

            if (type == "src")
            {
                element = ((ElementWrapper)connectorWrapper.source).WrappedElement;
            }
            else
            {
                element = ((ElementWrapper)connectorWrapper.target).WrappedElement;
            }
            

            return element.Type == owner;
        }

        public string getConnectorOwnerName(object connector, Model model, string type)
        {
            ConnectorWrapper connectorWrapper = new ConnectorWrapper(model, (EA.Connector)connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.owner;
            EA.Element element;

            if (type == "src")
            {
                element = ((ElementWrapper)connectorWrapper.source).WrappedElement;
            }
            else
            {
                element = ((ElementWrapper)connectorWrapper.target).WrappedElement;
            }

            return element.Name;
        }

        public object getConnectorOwner(object connector, Model model, string type)
        {
            ConnectorWrapper connectorWrapper = new ConnectorWrapper(model, (EA.Connector)connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.owner;
            EA.Element element;

            if (type == "src")
            {
                element = ((ElementWrapper)connectorWrapper.source).WrappedElement;
            }
            else
            {
                element = ((ElementWrapper)connectorWrapper.target).WrappedElement;
            }

            return element;
        }
        
        private EA.Element findElementInCurDiagramtByName(Model model, string targetName)
        {
            EA.Diagram diagram = model.getWrappedModel().GetCurrentDiagram();

            foreach (EA.DiagramObject diagObj in diagram.DiagramObjects)
            {
                EA.Element el = model.getWrappedModel().GetElementByID(diagObj.ElementID);
                if (el.Name == targetName)
                {
                    return el;
                }
            }

            return null;
        }
    }
}
