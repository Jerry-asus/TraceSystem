using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCDaClient
{
    public class OPCDto
    {
        public string HostName { get; set; }

        public string ProgID { get; set; }

        public string DA_Version { get; set; }

        public int Rate { get; set; }

        public string ItemName { get; set; }

        public bool  IsConnect { get; set; }

        public bool Start { get; set; }
    }
}
