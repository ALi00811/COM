using ComProject;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;

namespace WpfApp1.Json
{
    public class SerializeJson
    {
        private string hashCode { get; set; }
        private string header { get; set; }
        private int frameCounter { get; set; }
        private string opCode { get; set; }
        private string data { get; set; }
        private string crc { get; set; }

        public JsonModel Data { get; set; }
        bool Access;
        public SerializeJson(ClassComConnection com, int value, string opcode,bool SaveFileJson)
        {
            Access = SaveFileJson;
            try
            {
                foreach (var item in com.GetInffo)
                {
                    hashCode += " " + item.ToString();
                }

                for (int i = 0; i < com.GetInffo.Count - 16; i++)
                {
                    header += "0X" + com.GetInffo[i].ToString("X") + " ";
                }

                frameCounter = com.GetInffo[0].ToString().Count();
                opCode = value.ToString("x10");
                data = opcode;

                for (int i = 16; i < com.GetInffo.Count; i++)
                {
                    crc += "0X" + com.GetInffo[i].ToString("X") + " ";
                }

                // ---------------------
                var Createfile = new JsonModel()
                {
                    HashCode = hashCode,
                    Header = header,
                    FrameCounter = frameCounter,
                    OpCode = opCode,
                    Data = data,
                    Crc = crc
                };

                // --------------------- 
                if (Access)
                {
                    string json = JsonConvert.SerializeObject(Createfile);
                    string exePath = Assembly.GetEntryAssembly().Location;
                    string directory = Path.GetDirectoryName(exePath);
                    string filePath = Path.Combine(directory, "inffo.json");
                    File.WriteAllText(filePath, json);
                }
                

                // ---------------------
                Data = Createfile;

            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Cant Write File Json");
            }
        }
    }
}
