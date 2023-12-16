using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace WpfApp1.Json
{
    public class DeserializeJson
    {
        public string Result { get; set; }
        public JsonModel Deserialize()
        {
            try
            {
                Result = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/inffo.json";
                return Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(File.ReadAllText(Result));
            }
            catch
            {
                MessageBox.Show("Error", "Cant Read File Json");
                return null;
            }

        }
    }
}
