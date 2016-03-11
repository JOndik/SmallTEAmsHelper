using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using EA;
using System.Threading;
using BPAddInTry;

namespace BPAddIn
{
    class ContextWrapper
    {
        public Wrapper.Model model;
        public Wrapper.Element currentItem { get; set; }
        public Wrapper.Diagram currentDiagram { get; set; }
        public Wrapper.ConnectorWrapper currentConnector { get; set; }
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

        public void handleDblClick(string GUID, ObjectType ot)
        {
            switch (ot)
            {
                case ObjectType.otElement:
                    this.currentItem = (Wrapper.Element)model.selectedElement;
                    this.currentConnector = null;
                    this.currentDiagram = null;
                    //MessageBox.Show(this.printCurrentItem());
                    changed = false;
                    break;
                case ObjectType.otPackage:
                    this.currentItem = (Wrapper.Element)model.selectedElement;
                    //MessageBox.Show(this.printCurrentItem());
                    break;
                case ObjectType.otConnector:
                    this.currentConnector = (Wrapper.ConnectorWrapper)model.getRelationByGUID(GUID);
                    //MessageBox.Show(this.printCurrentConnector());
                    break;
                case ObjectType.otDiagram:
                    this.currentDiagram = (Wrapper.Diagram)model.selectedDiagram;
                    //MessageBox.Show(this.printCurrentDiagram());
                    break;
            }

        }

        public void handleChange(string GUID)
        {
            if (changed == false && currentItem != null)
            {
                handleElementChange(GUID);
                this.changed = true;
            }
        }

        public void handleElementChange(string GUID)
        {
            Wrapper.Element changedElement = (Wrapper.Element)model.getElementByGUID(GUID);

            if (currentItem.name != changedElement.name)
            {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.itemGUID = GUID;
                propertyChange.propertyType = 0;
                propertyChange.propertyBody = changedElement.name;

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
        public string printCurrentConnector()
        {
            return currentConnector.ToString();
        }
    }
}
