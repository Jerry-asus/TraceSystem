using System;
using System.Collections.Generic;
using System.Text;
using TraceSystem.Extension;

namespace TraceSystem.CommCore.S7Core
{
    public class S7Dto
    {
        public string HostIP { get; set; }

        public int Rock { get; set; }

        public int Slot { get; set; }

        public S7ItemModel ItemData { get; set; }

        public UpdateMethod Mehtod { get; set; }

        public string CpuType { get; set; }


    }

    
}
