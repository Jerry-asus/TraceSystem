using System;
using System.Collections.Generic;
using System.Text;

namespace TraceSystem.Context
{
    public class ItemNames:BaseContext
    {
        public string ItemName { get; set; }

        public List<ItemValues> ItemValue { get; set; }
    }
}
