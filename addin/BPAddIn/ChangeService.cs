using BPAddIn.DataContract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class ChangeService
    {
        public void saveAndSendChange(ModelChange change)
        {
            string jsonChange = serializeChange(change);

            MessageBox.Show(jsonChange);
            using (LocalDBContext context = new LocalDBContext())
            {
                context.modelChanges.Add(change);
                context.SaveChanges();
            }
        }

        public string serializeChange(ModelChange change)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            return JsonConvert.SerializeObject(change, settings);
        }
    }
}
