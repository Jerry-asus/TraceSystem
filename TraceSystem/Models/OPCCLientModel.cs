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

        private string hostName;
        private ObservableCollection<List<string>> serverProgID;
        private int rate;
        private ObservableCollection<string> items;
        private string itemName;
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; RaisePropertyChanged(); }
        }

       /// <summary>
       /// OPC服务器ID
       /// </summary>
        public ObservableCollection<List<string>> ServerProgID
        {
            get { return serverProgID; }
            set { serverProgID = value; RaisePropertyChanged(); }
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



    }
}
