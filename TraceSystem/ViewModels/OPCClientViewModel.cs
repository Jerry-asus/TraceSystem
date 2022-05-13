using Prism.Commands;
using Prism.Mvvm;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.NetCore.WPF;
using RealTimeGraphX.Renderers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Industry.Extenal;
using TraceSystem.CommCore.OPCDa;
using TraceSystem.Models;

namespace TraceSystem.ViewModels
{
    public class OPCClientViewModel : BindableBase
    {

        public OPCClientViewModel(IOPCClient client)
        {
            OPCClientInfo = new OPCCLientModel();
            this.Client = client;
            BrowserServerCommand = new DelegateCommand(BroswerCmd);

            ConnectCmd = new DelegateCommand<string>(ConnectCommand);
            AddItemCmd = new DelegateCommand<string>(AddItemCommand);

            MultiController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            MultiController.Renderer = new ScrollingLineRenderer<WpfGraphDataSeries>();

            MultiController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            MultiController.Range.MinimumY = 0;
            MultiController.Range.MaximumY = 1080;
            MultiController.Range.MaximumX = TimeSpan.FromSeconds(10);
            MultiController.Range.AutoY = true;
        }

        private void AddItemCommand(string obj)
        {
           if (!OPCClientInfo.Connect)
            {
                Client.AddItem();
            }
        }

        private void ConnectCommand(string obj)
        {
            switch (obj)
            {
                case "Connect":
                    Respostory res1 = Client.ConnectServer(OPCClientInfo.SelectServer.Name, OPCClientInfo.HostName, OPCClientInfo.Rate);
                    if(res1.Status)
                    {
                        OPCClientInfo.Connect = false;
                    }
                    else
                    {

                    }



                    break;
                case "Disconnect":
                    Respostory res2 = Client.Disconnect(OPCClientInfo.SelectServer.Name, OPCClientInfo.HostName);
                    if(res2.Status)
                    {
                        OPCClientInfo.Connect = true;
                    }
                    break;
            }            
        }

        private void BroswerCmd()
        {
            if (string.IsNullOrEmpty(OPCClientInfo.HostName)) return;
            object listObj = Client.GetOPCServerList("DA20", OPCClientInfo.HostName).Obj;
            IList li = (IList)listObj;
            if (OPCClientInfo.ServerList.Count > 0)
                OPCClientInfo.ServerList.Clear();
            for (int i = 0; i < li.Count; i++)
            {
                OPCClientInfo.ServerList.Add(new OPCCombox() { ID = i, Name = li[i].ToString() });
            }
           ;
        }

        private OPCCLientModel _OPCClientInfo;
        private readonly IOPCClient Client;

        public OPCCLientModel OPCClientInfo
        {
            get { return _OPCClientInfo; }
            set { _OPCClientInfo = value; RaisePropertyChanged(); }
        }

        public DelegateCommand BrowserServerCommand { get; set; }

        public DelegateCommand<string> ConnectCmd { get; set; }

        public DelegateCommand<string> AddItemCmd { get; set; }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> MultiController { get; set; }

    }
}
