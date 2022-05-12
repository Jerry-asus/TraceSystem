using Opc.Da;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TraceSystem.CommCore.OPCDa
{
    public class ClientModel
    {
        public string ServerName { get; set; }

        public Opc.Da.Server Server { get; set; }

        public Opc.Da.Subscription Subscription { get; set; }

        public Opc.Da.SubscriptionState SubscriptionState { get; set; }

        public SubscriptionState GroupInfo { get; set; }


        public ConcurrentDictionary<string, ItemModel> ItemsDic { get; set; }

        public Thread MyThread { get; set; }
    }
}
