using Opc.Da;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraceSystem.Extension;

namespace TraceSystem.CommCore.OPCDa
{
    public class ClientModel
    {


        public ClientModel()
        {
            Cto = new CancellationTokenSource();
        }
      //  public string ServerName { get; set; }
        public Opc.Da.Server Server { get; set; }

        public Opc.Da.Subscription Subscription { get; set; }

        public Opc.Da.SubscriptionState SubscriptionState { get; set; }

        //public SubscriptionState GroupInfo { get; set; }

        public ConcurrentDictionary<string, ItemModel> ItemsDic { get; set; }

        public List<string> mItempNameList { get; set; }

        //public OPCNode Nodes { get; set; }

        public Task<Respostory> MyTask { get; set; }

        public CancellationTokenSource Cto { get; set; }
    }
}
