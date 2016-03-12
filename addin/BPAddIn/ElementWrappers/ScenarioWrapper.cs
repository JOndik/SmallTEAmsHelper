using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.ElementWrappers
{
    class ScenarioWrapper : IEquatable<ScenarioWrapper>
    {
        public EA.Scenario scenario { get; set; }

        public ScenarioWrapper(EA.Scenario scenario)
        {
            this.scenario = scenario;
        }

        public bool Equals(ScenarioWrapper other)
        {
            bool stepsEqual = scenario.Steps.Count == other.scenario.Steps.Count;

            if (stepsEqual)
            {
                for (short i = 0; i < scenario.Steps.Count; i++)
                {
                    EA.ScenarioStep thisStep = (EA.ScenarioStep)scenario.Steps.GetAt(i);
                    EA.ScenarioStep otherStep = (EA.ScenarioStep)other.scenario.Steps.GetAt(i);

                    if (thisStep.Name != otherStep.Name || thisStep.Results != otherStep.Results || thisStep.State != otherStep.State
                        || thisStep.Uses != otherStep.Uses || thisStep.StepType != otherStep.StepType || thisStep.Pos != otherStep.Pos
                        || thisStep.StepGUID != otherStep.StepGUID)
                    {
                        stepsEqual = false;
                        break;
                    }

                    string thisExtensionGUID = "";
                    string thisJoiningStepGUID = "";
                    string thisJoiningStepPosition = "";

                    foreach (EA.ScenarioExtension ext in thisStep.Extensions)
                    {
                        thisExtensionGUID += ext.ExtensionGUID + ",";
                        thisJoiningStepGUID += ext.Join + ",";
                        thisJoiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                    }


                    string otherExtensionGUID = "";
                    string otherJoiningStepGUID = "";
                    string otherJoiningStepPosition = "";

                    foreach (EA.ScenarioExtension ext in otherStep.Extensions)
                    {
                        otherExtensionGUID += ext.ExtensionGUID + ",";
                        otherJoiningStepGUID += ext.Join + ",";
                        otherJoiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                    }

                    string thisExtension = thisExtensionGUID + thisJoiningStepGUID + thisJoiningStepPosition;
                    string otherExtension = otherExtensionGUID + otherJoiningStepGUID + otherJoiningStepPosition;

                    if (thisExtension != otherExtension)
                    {
                        stepsEqual = false;
                        break;
                    }
                }
            }

            return scenario.Name.Equals(other.scenario.Name)
                && scenario.Type.Equals(other.scenario.Type)
                && scenario.ScenarioGUID.Equals(other.scenario.ScenarioGUID)
                && stepsEqual;
        }
        public override bool Equals(object other)
        {
            if (other is ScenarioWrapper)
                return this.Equals((ScenarioWrapper)other);
            else
                return false;
        }
        public override int GetHashCode()
        {
            int hashKeySteps = scenario.Steps.Count.GetHashCode();

            for (short i = 0; i < scenario.Steps.Count; i++)
            {
                EA.ScenarioStep thisStep = (EA.ScenarioStep)scenario.Steps.GetAt(i);

                hashKeySteps ^= thisStep.Name.GetHashCode();
                hashKeySteps ^= thisStep.Results.GetHashCode();
                hashKeySteps ^= thisStep.State.GetHashCode();
                hashKeySteps ^= thisStep.Uses.GetHashCode();
                hashKeySteps ^= thisStep.StepType.GetHashCode();
                hashKeySteps ^= thisStep.Pos.GetHashCode();
                hashKeySteps ^= thisStep.StepGUID.GetHashCode();


                string thisExtensionGUID = "";
                string thisJoiningStepGUID = "";
                string thisJoiningStepPosition = "";

                foreach (EA.ScenarioExtension ext in thisStep.Extensions)
                {
                    thisExtensionGUID += ext.ExtensionGUID + ",";
                    thisJoiningStepGUID += ext.Join + ",";
                    thisJoiningStepPosition += ext.JoiningStep == null ? "" : ext.JoiningStep.Pos + ",";
                }

                string thisExtension = thisExtensionGUID + thisJoiningStepGUID + thisJoiningStepPosition;
                hashKeySteps ^= thisExtension.GetHashCode();
            }


            int hashKeyName = scenario.Name == null ? 0 : scenario.Name.GetHashCode();
            int hashKeyType = scenario.Type.GetHashCode();
            int hashKeyGUID = scenario.ScenarioGUID.GetHashCode();
            int hashKeyStepCount = scenario.Steps.Count.GetHashCode();

            return hashKeyName ^ hashKeyType ^ hashKeyGUID ^ hashKeyStepCount ^ hashKeySteps;
        }

        public string getGUID()
        {
            return scenario.ScenarioGUID;
        }

        public override string ToString()
        {
            return scenario.Name + ";" + scenario.Type;
        }
    }
}
