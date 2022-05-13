using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceSystem.Models
{
    public class OPCClientModel : BindableBase
    {

        public OPCClientModel()
        {
            ServerList = new ObservableCollection<string>();
            Items = new ObservableCollection<string>();
            DAVerList = new List<string>();
            DAVerList.Add("DA10");
            DAVerList.Add("DA20");
            DAVerList.Add("DA30");


           
        }

        private string hostName;
        private ObservableCollection<string> serverList;
        private ObservableCollection<string> items;
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
        public ObservableCollection<string> ServerList
        {
            get { return serverList; }
            set { serverList = value; }
        }

        /// <summary>
        /// 变量集合
        /// </summary>
        public ObservableCollection<string> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(); }
        }

        private string daVer;

        public string DAVer
        {
            get { return daVer; }
            set { daVer = value;RaisePropertyChanged(); }
        }

        public List<string> DAVerList { get; set; }


        private string selectServer;

        public string SelectServer
        {
            get { return selectServer; }
            set { selectServer = value; RaisePropertyChanged(); }
        }

        private string selestItemName;

        public string SelectItemName
        {
            get { return selestItemName; }
            set { selestItemName = value;RaisePropertyChanged(); }
        }



    }
    

    public enum DaVer
    {
        DA10,
        DA20,
        DA30
    }

}
