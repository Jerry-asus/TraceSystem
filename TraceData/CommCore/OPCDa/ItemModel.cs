using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Da;

namespace TraceData.CommCore.OPCDa
{
    [Serializable]
    public class ItemModel
    {
        public ItemValue Value { get; set; }

        public Opc.Da.Item Item { get; set; }

        public string DataType { get; set; }

       
    }
}
