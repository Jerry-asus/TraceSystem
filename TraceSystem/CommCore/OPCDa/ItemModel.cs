using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceSystem.CommCore.OPCDa
{
    public class ItemModel
    {
        public Opc.Da.ItemValue Value { get; set; } = null;

        public Opc.Da.Item Item { get; set; } = null;

    }
}
