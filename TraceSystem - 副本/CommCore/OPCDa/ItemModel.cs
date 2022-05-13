using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Da;

namespace TraceSystem.CommCore.OPCDa
{
    [Serializable]
    public class ItemModel
    {
        public ItemValue Value { get; set; }

        public Opc.Da.Item Item { get; set; }

        public string DataType { get; set; }

       
    }
}
