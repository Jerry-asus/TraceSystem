using Opc.Da;
using System;

namespace OPCDaClient
{
    public class OPCItemModel
    {
        public int ClientHandle { get; set; }

        public string TagName { get; set; }

        public Item Item { get; set; }

        public Object Value { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime Timestamp { get; set; }

        public int Quality { get; set; }



    }
}
