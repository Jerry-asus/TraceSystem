using Common;
using Opc.Da;
using OPCDaMutithreadCore.OPCDa;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OPCDaMutithreadCore.Models
{
    public class ClientModel
    {


        public ClientModel()
        {
            Cto = new CancellationTokenSource();
        }
        public Server Server { get; set; }

        public Subscription Subscription { get; set; }

        public SubscriptionState SubscriptionState { get; set; }


        public ConcurrentDictionary<string, OPCItemModel> ItemsDic { get; set; }

        public List<string> mItempNameList { get; set; }


        public Task<Responsitory> MyTask { get; set; }

        public CancellationTokenSource Cto { get; set; }
    }
}
