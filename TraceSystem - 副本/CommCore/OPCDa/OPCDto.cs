using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceSystem.Extension;

namespace TraceSystem.CommCore.OPCDa
{
    public class OPCDto
    {

        /// <summary>
        /// 添加或者删除Item方法标记
        /// </summary>
        public UpdateMethod ItemMethod { get; set; }
        public string HostName { get; set; }

        public string ProgID { get; set; }

        public string DA_Version { get; set; }

        public int Rate { get; set; }

        public string ItemName { get; set; }

        public bool  IsConnect { get; set; }

        public bool Start { get; set; }
    }


   
}
