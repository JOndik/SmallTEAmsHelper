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
