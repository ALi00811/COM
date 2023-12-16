using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Json
{
    public class JsonModel
    {
        public string HashCode { get; set; }
        public string Header { get; set; }
        public int FrameCounter { get; set; }
        public string OpCode { get; set; }
        public string Data { get; set; }
        public string Crc { get; set; }
    }
}
