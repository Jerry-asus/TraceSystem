using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TraceSystem.Models;

namespace TraceSystem.ViewModels
{
    public class OPCClientViewModel:BindableBase
    {

        public OPCClientViewModel()
        {
            OPCClientInfo = new OPCCLientModel();
          //  BrowserServerCommand = new DelegateCommand(BroswerCmd);
           // Opcclient = opcclient;
        }

        private void BroswerCmd()
        {
            //if (string.IsNullOrEmpty(OPCClientInfo.HostName)) return;
            //List<string> listObj = (List<string>)Opcclient.GetOPCServerList("DA20", OPCClientInfo.HostName).Obj;
            //OPCClientInfo.ServerProgID.Add(listObj);
        }

        private OPCCLientModel _OPCClientInfo;

        public OPCCLientModel OPCClientInfo
        {
            get { return _OPCClientInfo; }
            set { _OPCClientInfo = value; RaisePropertyChanged(); }
        }

        public DelegateCommand BrowserServerCommand { get; set; }
        //public IOPCClient Opcclient { get; }
    }
}
