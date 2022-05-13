using System;
using System.Collections.Generic;
using System.Text;

namespace TraceSystem.CommCore.S7Core
{
    public class S7ClientModel
    {

        public S7ClientModel()
        {

            ItemData = new List<S7ItemModel>();
            PLC = new Sharp7.S7Client();
        }

        public Sharp7.S7Client PLC { get; set; }

        public List<S7ItemModel> ItemData { get; set; }

    }
}
