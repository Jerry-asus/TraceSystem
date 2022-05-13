using Meziantou.Framework.WPF.Collections;
using Prism.Commands;
using Prism.Mvvm;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Core.Utility;
using SciChart.Data.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TraceSystem.CommCore.OPCDa;
using TraceSystem.CommCore.S7Core;
using TraceSystem.Extension;
using TraceSystem.Models;

namespace TraceSystem.ViewModels
{


    public class CommViewModel : BindableBase
    {

        public CommViewModel()
        {

        }

        //Is7Client s7Client
        public CommViewModel(IOPCClient opcclient, Is7Client s7Client)
        {
            Opcclient = opcclient;
            S7Client = s7Client;

            GetOPCSerListCommand = new DelegateCommand(GetOPCSerList);
            ConnectOPCServCommand = new DelegateCommand(ConnectOPCServ);
            DisconnectOPCServCommand = new DelegateCommand<string>(DisconnectOPCSvr);
            UpDateOPCItemCommand = new DelegateCommand<string>(UpdateOPCItemCMD);
            StartUpdateCommand = new DelegateCommand<string>(StartUpdate);

            S7ConnectCommand = new DelegateCommand<string>(S7Connect);
            S7AddItemCommand = new DelegateCommand(S7AddItem);

            PreviewMouseDownCommad = new DelegateCommand(PreviewMouseDown);
            PreviewMouseDoubleClickCommand = new DelegateCommand(PreviewMouseDoubleClick);

            ServerListChangeCommand = new DelegateCommand(ServerListChange);



            ItemValues = new ObservableCollection<ItemValueModel>();
            RenderableSeries = new ObservableCollection<IRenderableSeriesViewModel>();
            XVisibleRange = new DateTimeAxis();


            OPCClientModel = new OPCClientModel();
            S7Model = new S7Model();
            EnableMouseWheel = false;

            GenerateData();

            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += (s, e) =>
            {
                UpdateSerial();
            };

        }





        #region 更新SciChart数据
        public void AddSerial(string serialName)
        {

            var lineData = new XyDataSeries<DateTime, double>() { SeriesName = serialName, FifoCapacity = 20000 };
            RenderableSeries.Add(new LineRenderableSeriesViewModel()
            {
                StrokeThickness = 2,
                Stroke = GetRandomColor(),
                DataSeries = lineData,
                StyleKey = "LineSeriesStyle",
                


            });
            lineData.Append(DateTime.Now, 0);
        }

        private Color GetRandomColor()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }

        private const double Dt = 0.02;
        private static readonly Mutex mut = new Mutex();

        #region Chart 参数定义
        private string chartTitle;
        private string xAxisTitle;
        private string yAxisTitle;


        DispatcherTimer timer;


        private ConcurrentObservableCollection<DataSerials> lineDatas;

        public ConcurrentObservableCollection<DataSerials> LineDatas
        {
            get { return lineDatas; }
            set { lineDatas = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<IRenderableSeriesViewModel> _renderableSeries;
        /// <summary>
        /// 趋势集合
        /// </summary>
        public ObservableCollection<IRenderableSeriesViewModel> RenderableSeries
        {
            get { return _renderableSeries; }
            set
            {
                _renderableSeries = value;
                RaisePropertyChanged();
            }
        }



        public string ChartTitle
        {
            get { return chartTitle; }
            set { chartTitle = value; RaisePropertyChanged(); }
        }
        public string XAxisTitle
        {
            get { return xAxisTitle; ; }
            set { xAxisTitle = value; RaisePropertyChanged(); }
        }

        public string YAxisTitle
        {
            get { return yAxisTitle; ; }
            set { yAxisTitle = value; RaisePropertyChanged(); }
        }

        private bool enableMouseWheel;

        public bool EnableMouseWheel
        {
            get { return enableMouseWheel; }
            set { enableMouseWheel = value; }
        }


        private ObservableCollection<ItemValueModel> itemValues;

        public ObservableCollection<ItemValueModel> ItemValues
        {
            get { return itemValues; }
            set { itemValues = value; }
        }

        private AutoRange autoRange;

        public AutoRange AutoRange1
        {
            get { return autoRange; }
            set { autoRange = value;RaisePropertyChanged(); }
        }

        private DateTimeAxis _xVisibleRange;
        public DateTimeAxis XVisibleRange
        {
            get { return _xVisibleRange; }
            set
            {
                if (_xVisibleRange != value)
                {
                    _xVisibleRange = value;
                    RaisePropertyChanged("XVisibleRange");
                }
            }
        }

        private double _windowSize = 10;
        private double _timeNow = 0;
        private bool _showLatestWindow = true;
        private bool _thatWasADoubleClick;



        #endregion


        private void PreviewMouseDown()
        {
            // On mouse down (but not double click), freeze our last N seconds window 
            if (!_thatWasADoubleClick) _showLatestWindow = false;

            _thatWasADoubleClick = false;
        }

        private void PreviewMouseDoubleClick()
        {
            _showLatestWindow = true;
            _thatWasADoubleClick = true; // (8): Prevent contention between double click and single click event

            // Restore our last N seconds window on double click
            // XVisibleRange=new DoubleRange(0, 1), TimeSpan.FromMilliseconds(200);
            // XVisibleRange=new DoubleRange(_timeNow - _windowSize, _timeNow), TimeSpan.FromMilliseconds(200);

            //XVisibleRange=

           // var dateTimeAxis = new DateTimeAxis();
            XVisibleRange.VisibleRange = new DateRange(DateTime.Now, DateTime.Now.AddMinutes(10));
        }


        private void StartUpdate(string obj)
        {
            if (obj == "Run")
                timer.Start();
            AutoRange1 = AutoRange.Always;
            EnableMouseWheel =false;
            //XVisibleRange = new TimeSpanRange(TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
            if (obj == "Stop")
                timer?.Stop();
            EnableMouseWheel = true;
            AutoRange1 = AutoRange.Never;
            if (obj == "Reset")
            {
                mut.WaitOne();
                Opcclient.Disconnect();
                S7Client.Disconnect();
                S7Model.ValidHostCollect.Clear();
                OPCClientModel.Items.Clear();
                RenderableSeries.Clear();
                mut.ReleaseMutex();
            }
        }




        private void UpDateChart(string ItemName, UpdateMethod method)
        {
            mut.WaitOne();
            switch (method)
            {
                case UpdateMethod.Add:
                    if (ItemName != null)
                    {
                        AddSerial(ItemName);
                    }
                    mut.ReleaseMutex();
                    break;
                case UpdateMethod.Delete:
                    if (ItemName != null && RenderableSeries.Count > 0)
                    {
                        for (int i = 0; i < RenderableSeries.Count; i++)
                        {
                            if (RenderableSeries[i].DataSeries.SeriesName == ItemName)
                            {
                                RenderableSeries.RemoveAt(i);
                                //LineDataCollection.RemoveAt(i);
                            }
                        }

                    }
                    mut.ReleaseMutex();
                    break;
                default:
                    mut.ReleaseMutex();
                    break;
            }
        }
        /// <summary>
        /// 循环从各个IO空气取回数据到本地
        /// </summary>
        private void GenerateData()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(20);
                    mut.WaitOne();
                    var date = DateTime.Now;
                    if (ItemValues != null)
                        ItemValues.Clear();
                    var opcRes = Opcclient.GetValues();
                    if (opcRes.Count > 0)
                    {
                        for (int i = 0; i < opcRes.Count; i++)
                        {
                            if (opcRes[i] != null)
                            {
                                ItemValues.Add(new ItemValueModel() { ItemName = opcRes[i].ItemName, value = Convert.ToDouble(opcRes[i].Value) });
                            }

                        }
                    }
                    var S7Res = S7Client.GetValues();
                    if(S7Res.Count > 0)
                    {
                        for (int i = 0; i < S7Res.Count; i++)
                        {
                            if (S7Res[i]!=null)
                            {
                                ItemValues.Add(new ItemValueModel() { ItemName = S7Res[i].ItemName, value = Convert.ToDouble(S7Res[i].Value) });
                            }
                        }
                    }

                   // List<S7ItemModel> GetValues()

                    mut.ReleaseMutex();
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 更新曲线值
        /// </summary>
        /// <param name="newValues"></param>
        private void UpdateSerial()
        {
            mut.WaitOne();
            _timeNow++;
            if (ItemValues.Count > 0)
            {
                for (int i = 0; i < ItemValues.Count; i++)
                {
                    if (RenderableSeries.Count > 0)
                    {
                        for (int n = 0; n < RenderableSeries.Count; n++)
                        {
                            var date = DateTime.Now;
                            if (RenderableSeries[n].DataSeries.SeriesName == ItemValues[i].ItemName)
                            {
                                var s = (IXyDataSeries<DateTime, double>)RenderableSeries[n].DataSeries;
                                s.Append(date, itemValues[i].value);
                                if (_showLatestWindow)
                                {
                                   // XVisibleRange.VisibleRange = new DateRange(DateTime.Now, DateTime.Now.AddMilliseconds(10));// new DoubleRange(_timeNow - _windowSize, _timeNow), TimeSpan.FromMilliseconds(280));
                                }
                            }
                        }
                    }

                }
            }
            mut.ReleaseMutex();
        }
        #endregion

        #region OPC Comm

        private string LocalServerTemp;
        private void ServerListChange()
        {
            if(LocalServerTemp!= OPCClientModel.SelectServer)
            {
                OPCClientModel.Items.Clear();
            }
            LocalServerTemp = OPCClientModel.SelectServer;
        }


        /// <summary>
        /// 更新OPCItem
        /// </summary>
        /// <param name="updateMethod"></param>
        private void UpdateOPCItemCMD(string updateMethod)
        {
            if (string.IsNullOrEmpty(OPCClientModel.SelectServer)) return;
            if (string.IsNullOrEmpty(OPCClientModel.HostName)) return;
            if (string.IsNullOrEmpty(OPCClientModel.SelectItemName)) return;
            OPCDto dto = new OPCDto();
            dto.HostName = OPCClientModel.HostName;
            dto.ProgID = OPCClientModel.SelectServer;
            dto.ItemName = OPCClientModel.SelectItemName;
            dto.ItemMethod = (UpdateMethod)Enum.Parse(typeof(UpdateMethod), updateMethod);

            UpdateOPCItem(dto);
        }




        /// <summary>
        /// 注册或者删除OPCItem
        /// </summary>
        /// <param name="dto"></param>
        private void UpdateOPCItem(OPCDto dto)
        {

            if (dto == null) return;
            Respostory res = Opcclient.UpdateItem(dto);
            if (res.Status)
            {
                UpDateChart(res.Obj.ToString(), dto.ItemMethod);
                //todo  弹出通知
                //标记使能 可以启动更新
            }
        }

        /// <summary>
        /// 连接OPC服务器
        /// </summary>
        private void ConnectOPCServ()
        {
            if (string.IsNullOrEmpty(OPCClientModel.SelectServer)) return;
            if (string.IsNullOrEmpty(OPCClientModel.HostName)) return;
            OPCDto dto = new OPCDto();
            dto.ProgID = OPCClientModel.SelectServer;
            dto.HostName = OPCClientModel.HostName;
            Respostory res = Opcclient.ConnectServer(dto);
            OPCClientModel.Items.Clear();
            if (res.Status)
            {
                IList<string> ServerList = (IList<string>)res.Obj;
                //List<string> list = (List<string>)res.Obj;
                if (ServerList!=null && ServerList.Count > 0)
                {
                    for (int i = 0; i < ServerList.Count; i++)
                    {
                        OPCClientModel.Items.Add(ServerList[i]);
                        OPCClientModel.SelectItemName = ServerList.FirstOrDefault();
                       
                    }
                }
            }
            else
            {
                //不能连接原因
            }

        }


        private void DisconnectOPCSvr(string serverName)
        {
            if(string.IsNullOrEmpty(serverName)) return;
            OPCDto dto = new OPCDto();
            dto.HostName = OPCClientModel.HostName;
            dto.ProgID = serverName;

           Respostory res= Opcclient.Disconnect(dto);
            if(res.Status)
            {
                OPCClientModel.Items.Clear();
            }
        }

        /// <summary>
        /// 获取OPC服务器列表
        /// </summary>
        private void GetOPCSerList()
        {
            if (string.IsNullOrEmpty(OPCClientModel.HostName)) OPCClientModel.HostName = "127.0.0.1";
            OPCDto dto = new OPCDto();
            dto.HostName = OPCClientModel.HostName;
            Respostory res = Opcclient.GetOPCServerList(dto);
            OPCClientModel.ServerList.Clear();
            if (res.Status)
            {
                List<string> list = (List<string>)res.Obj;
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        OPCClientModel.ServerList.Add(list[i]);
                        OPCClientModel.SelectServer = list.FirstOrDefault();
                        LocalServerTemp = OPCClientModel.SelectServer;


                    }
                }
            }
            else
            {
                //Todo,展示失败信息
            }
        }



        public IOPCClient Opcclient { get; }
        public Is7Client S7Client { get; }

        public DelegateCommand GetOPCSerListCommand { get; set; }

        public DelegateCommand ConnectOPCServCommand { get; set; }
        public DelegateCommand<string> DisconnectOPCServCommand { get; set; }

        public DelegateCommand<string> UpDateOPCItemCommand { get; set; }

        public DelegateCommand<string> StartUpdateCommand { get; set; }

        public DelegateCommand PreviewMouseDownCommad { get; set; }

        public DelegateCommand PreviewMouseDoubleClickCommand { get; set; }

        public DelegateCommand ServerListChangeCommand { get; set; }
        public OPCClientModel OPCClientModel { get; set; }

        public S7Model S7Model { get; set; }

        #endregion




        #region S7Comm

        public DelegateCommand<string> S7ConnectCommand { get; set; }

        public DelegateCommand S7AddItemCommand { get; set; }


        private void S7Connect(string MethodSingle)
        {
            S7Dto dto = new S7Dto();
            if (MethodSingle=="Connect")
            {
                if (string.IsNullOrEmpty(S7Model.HostIP)) S7Model.HostIP = "192.168.10.21";
                dto.HostIP = S7Model.HostIP;
                dto.Rock = int.Parse(S7Model.SelectRock);
                dto.Slot = int.Parse(S7Model.SeletcSlot);
                Respostory res = S7Client.ConnectServer(dto);
                if (res.Status)
                {
                    S7Model.ValidHostCollect.Clear();
                    S7Model.ValidHostCollect.Add(dto.HostIP + "_" + dto.Rock + "_" + dto.Slot);
                    S7Model.SelectHostName = S7Model.ValidHostCollect.FirstOrDefault();
                }
            }
            if(MethodSingle == "DisConnect")
            {
                string str = S7Model.SelectHostName;
                if (!string.IsNullOrEmpty(str))
                {
                    int p1 = str.IndexOf("_");
                    int p2 = str.LastIndexOf("_");
                    int p3 = str.Length;
                    dto.HostIP = str.Substring(0, p1);
                    dto.Rock = short.Parse(str.Substring(p1 + 1, 1));
                    dto.Slot = short.Parse(str.Substring(p2 + 1, 1));

                    Respostory res = S7Client.Disconnect(dto);
                    if (res.Status)
                    {
                        S7Model.ValidHostCollect.Remove(str);
                    }
                    else
                    {
                        //todo  警告信息
                    }
                }
            }

           
        }

        private void S7AddItem()
        {
            S7Dto dto = new S7Dto();
            string str = S7Model.SelectHostName;
            if (!string.IsNullOrEmpty(S7Model.SelectHostName))
            {
                int p1 = str.IndexOf("_");
                int p2 = str.LastIndexOf("_");
                int p3 = str.Length;
                dto.HostIP = str.Substring(0, p1);
                dto.Rock = short.Parse(str.Substring(p1 + 1, 1));
                dto.Slot = short.Parse(str.Substring(p2 + 1, 1));
            }
            dto.ItemData = new S7ItemModel();
            dto.ItemData.Datatype = (S7DataType)Enum.Parse(typeof(S7DataType), S7Model.SelectDatatype);
            if (string.IsNullOrWhiteSpace(S7Model.DBNum) || string.IsNullOrWhiteSpace(S7Model.StartAddress) || string.IsNullOrWhiteSpace(S7Model.Length))
            {
                return;
            }
            if (!Regex.IsMatch(S7Model.DBNum, @"^-?[1-9999]\d*$|^0$"))
            {
                return;
            }
            if (!Regex.IsMatch(S7Model.StartAddress, @"^-?[1-9999]\d*$|^0$"))
            {
                return;
            }
            if (!Regex.IsMatch(S7Model.Length, @"^-?[1-9999]\d*$|^0$"))
            {
                return;
            }
            dto.ItemData.DBNumber = int.Parse(S7Model.DBNum);
            dto.ItemData.StartAddr = int.Parse(S7Model.StartAddress);
            dto.ItemData.Length = int.Parse(S7Model.Length);
            dto.ItemData.ItemName = dto.ItemData.DBNumber + "_" + dto.ItemData.StartAddr + "_" + dto.ItemData.Length;
            dto.Mehtod = UpdateMethod.Add;

            Respostory res = S7Client.UpDateItemList(dto);
            if(res.Status)
            {
                UpDateChart(((S7ItemModel)res.Obj).ItemName, dto.Mehtod);
            }


        }



        #endregion


    }
}
