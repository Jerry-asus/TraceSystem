using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceSystem.Models
{
    public class OPCCLientModel : BindableBase
    {


        public OPCCLientModel()
        {
            ServerList = new ObservableCollection<OPCCombox>();
            Connect = true;
        }

        private string hostName;
        private ObservableCollection<OPCCombox> serverList;
        private int rate;
        private ObservableCollection<string> items;
        private string itemName;
        private OPCCombox selectServer;
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName
        {
            get { return hostName; }
            set { hostName = value;
                if(hostName!=null)
                RaisePropertyChanged(); }
        }

       /// <summary>
       /// OPC服务器ID
       /// </summary>
        public ObservableCollection<OPCCombox> ServerList
        {
            get { return serverList; }
            set { serverList = value; }
        }
    
        /// <summary>
        /// 更新速度
        /// </summary>
        public int Rate
        {
            get { return rate; }
            set { rate = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Item名
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { itemName = value;RaisePropertyChanged(); }
        }

        /// <summary>
        /// 变量集合
        /// </summary>
        public ObservableCollection<string> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(); }
        }

       

        public OPCCombox SelectServer
        {
            get { return selectServer; }
            set { selectServer = value;RaisePropertyChanged(); }
        }

        private bool connect;

        public bool Connect
        {
            get { return connect;; }
            set { connect= value;RaisePropertyChanged(); }
        }


    }
    public class OPCCombox:BindableBase
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value;RaisePropertyChanged(); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value;RaisePropertyChanged(); }
        }


    }
}
