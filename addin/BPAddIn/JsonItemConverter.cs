using BPAddIn.DataContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class JsonItemConverter : Newtonsoft.Json.Converters.CustomCreationConverter<ModelChange>
    {
        public override ModelChange Create(Type objectType)
        {
            /*MessageBox.Show(objectType.ToString());
            return new PropertyChange();*/
            throw new NotImplementedException();
        }

        public ModelChange Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("classType");
            switch (type)
            {
                case "ItemCreation":
                    return new ItemCreation();
                case "PropertyChange":
                    return new PropertyChange();
                case "ScenarioChange":
                    return new ScenarioChange();
                case "StepChange":
                    return new StepChange();
            }
            
            /*var type = (string)jObject.Property("valueType");
            MessageBox.Show(type.ToString());
            switch (type)
            {
                case "int":
                    return new ItemCreation();
                case "string":
                    return new PropertyChange();
            }*/

           /* if (FieldExists("propertyType", jObject))
            {
                MessageBox.Show("1");
                return new PropertyChange();
            }
            else if (FieldExists("stepGUID", jObject))
            {
                MessageBox.Show("2");
                return new StepChange();
            }
            else if (FieldExists("srcGUID", jObject))
            {
                MessageBox.Show("3");
                return new ItemCreation();
            }*/

            throw new ApplicationException(String.Format("The given vehicle type {0} is not supported!"));
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }
}
