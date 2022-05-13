using Opc.Da;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPCDaMutithreadCore.Models
{
    public class OPCItemModel
    {
        public ItemValue ItemValue { get; set; }

        public Item Item { get; set; }

        public string DataType { get; set; }

    }
}
