using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Windows.Forms;
using System.Threading;
using BPAddIn;
using System.Globalization;

namespace BPAddInTry
{
    public class RuleClass : Rule
    {

        public RuleClass() : base()
        {
            this.name = "ClassDiagram::Class::BadName";
            this.actions = -1;
        } 

        public override void activate(Wrapper.Model modelArg, string GUID)
        {
            objGUID = "CNAME" + GUID;
            element = modelArg.getWrappedModel().GetElementByGuid(GUID);
            model = modelArg;
            string type = element.Type;
            BPAddIn.Dictionary dict = new BPAddIn.Dictionary();
            defectDescription = "Nespravny nazov pre triedu '" + element.Name + "'.";
            listBoxObject = new ListBoxObject(defectDescription, defectDescription, defectDescription, this);

            if ("Class".Equals(type))
            {
                if (!dict.isNoun(element.Name.Split(" ".ToCharArray())))
                {
                    if (!isActive)
                    {
                        this.actions = 0;
                        this.ruleGUID = Guid.NewGuid().ToString();
                    }
                    this.isActive = true;
                    //this.ping();
                    showErrorWindow();
                    BPAddIn.BPAddIn.defectsWindow.addToList(listBoxObject);
                }
                else
                {
                    //MessageBox.Show("nie je chyba");
                    BPAddIn.BPAddIn.defectsWindow.removeFromListSpecial(this);
                    BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
                    this.isActive = false;
                }
            }
        }

        public override void correct()
        {
            string[] splitWord = element.Name.Split(" ".ToCharArray());
            string word = "";

            if (splitWord.Length == 1)
            {
                word = splitWord[0];
            }
            else if (splitWord.Length > 1)
            {
                word = splitWord[1];
            }
            else
            {
                return;
            }

            using (LocalDBContext context = new LocalDBContext())
            {
                var words = from w in context.dictionary
                            where w.word == word.ToLower()
                            select w.wordBase;

                try
                {
                    string wordBase = words.FirstOrDefault<String>();

                    if (String.IsNullOrEmpty(wordBase))
                    {
                        MessageBox.Show("Chybu sa nepodarilo opravit.");
                        return;
                    }

                    if (splitWord.Length == 1)
                    {
                        splitWord[0] = wordBase;
                    }
                    else if (splitWord.Length > 1)
                    {
                        splitWord[1] = wordBase;
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            splitWord[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(splitWord[0].ToLower());

            element.Name = String.Join(" ", splitWord);
            element.Update();
            model.adviseChange(new ElementWrapper(model, element));
            BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
            BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
            this.isActive = false;
        }

        public override void selectInDiagram()
        {
            connectorWrapper.select();
            connectorWrapper.selectInCurrentDiagram();
        }

        public override void activate(EA.Element element)
        {

        }

        public override void showErrorWindow()
        {
            if (BPAddIn.BPAddIn.defectsWindow == null)
            {
                BPAddIn.BPAddIn.defectsWindow = model.getWrappedModel().AddWindow("Detekované chyby", "BPAddIn.DefectsWindow") as DefectsWindow;
            }

            model.getWrappedModel().ShowAddinWindow("Detekované chyby");
        }

        public override string ToString()
        {
            return defectDescription;
        }
    }
}
