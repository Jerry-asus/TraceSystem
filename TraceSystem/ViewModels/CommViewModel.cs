using Common;
using OPCDaMutithreadCore.OPCDa;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using S7Core;
using S7Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using TraceSystem.Models;

namespace TraceSystem.ViewModels
{


    public class CommViewModel : BindableBase
    {

        
        public CommViewModel(IOPCClient opcclient, Is7Client s7Client)
        {
            Opcclient = opcclient;
            S7Client = s7Client;
            ///Command
            GetOPCSerListCommand = new DelegateCommand(GetOPCSerList);
            ConnectOPCServCommand = new DelegateCommand(ConnectOPCServ);
            DisconnectOPCServCommand = new DelegateCommand<string>(DisconnectOPCSvr);
            UpDateOPCItemCommand = new DelegateCommand<string>(UpdateOPCItemCMD);
            StartUpdateCommand = new DelegateCommand<string>(StartUpdate);
            S7ConnectCommand = new DelegateCommand<string>(S7Connect);
            S7AddItemCommand = new DelegateCommand(S7AddItem);
            ServerListChangeCommand = new DelegateCommand(ServerListChange);
            ItemValues = new ObservableCollection<ItemValueModel>();

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
            //t1 = new UltraHighAccurateTimer();

            Model = new PlotModel();

            Cratechart();

        }




        //UltraHighAccurateTimer t1;


        


        #region Dependency Properties

        private PlotModel model;

        public PlotModel Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
        }

        private Series series;

        public Series Series
        {
            get { return series; }
            set { series = value; }
        }


        private void Cratechart()
        {
            model.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                IsZoomEnabled = true,
                Position = AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd MMM",
                //StartPosition = DateTime.Now.ToOADate(),
                //EndPosition = DateTime.Now.AddMinutes(10).ToOADate(),


            });
            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                IsZoomEnabled = true,
            });

        }

        public void AddSerial(string serialName)
        {
            if (Model.Series == null) return;
            for (int i = 0; i < Model.Series.Count; i++)
            {
                if (Model.Series[i].Title == serialName)
                    return;
            }
            Model.Series.Add(new LineSeries()
            {
           
               Title = serialName,
               LineLegendPosition=LineLegendPosition.End,

            });
            //for (int i = 0; i < Series.Count; i++)
            //{
            //    if (Series[i].Name == serialName) return;
            //}
            //Brush brush = GetRandomColor();
            //Series.Add(new LineSeries()
            //{
            //    Tag = serialName,
            //    Title = serialName,
            //    PointGeometry = DefaultGeometries.Circle,
            //    PointGeometrySize = 3,
            //    LineSmoothness = 1,
            //    StrokeThickness = 2,
            //    Stroke = brush,
            //    Fill = new SolidColorBrush(Colors.Transparent),
            //    DataLabels = true,
            //    Foreground = brush,
            //    Values = new GearedValues<double>().WithQuality(Quality.Highest)
            //}); ;
        }

        public void DeletSerial(string serialName)
        {
            if (Model.Series == null) return;
            for (int i = 0; i < Model.Series.Count; i++)
            {
                if (Model.Series[i].Title == serialName)
                    Model.Series.RemoveAt(i);
            }


            //for (int i = 0; i < Series.Count; i++)
            //{
            //    //if (Series[i].Name == serialName)
            //    //{
            //    //    Series.RemoveAt(i);
            //    //}
            //}

        }
        private Brush GetRandomColor()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            return new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));
        }

        private const double Dt = 0.02;
        private static readonly Mutex mut = new Mutex();

        #region Chart 参数定义
        private string chartTitle;
        private string xAxisTitle;
        private string yAxisTitle;


        DispatcherTimer timer;

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

        #endregion

        private void InitChart()
        {
            ChartTitle = "数据跟踪";
            XAxisTitle = "时间";
            YAxisTitle = "数值";

        }
        private void StartUpdate(string obj)
        {
            if (obj == "Run")
                timer.Start();
            // AutoRange1 = AutoRange.Always;
            EnableMouseWheel = false;
            //XVisibleRange = new TimeSpanRange(TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
            if (obj == "Stop")
                timer?.Stop();
            EnableMouseWheel = true;
            // AutoRange1 = AutoRange.Never;
            if (obj == "Reset")
            {
                mut.WaitOne();
                Opcclient.Disconnect();
                S7Client.Disconnect();
                S7Model.ValidHostCollect.Clear();
                OPCClientModel.Items.Clear();
                // RenderableSeries.Clear();
                Model.Series.Clear();
                Model.InvalidatePlot(true);
                mut.ReleaseMutex();
            }
        }
        private void UpDateChart(string ItemName, Method method)
        {
            if (string.IsNullOrEmpty(ItemName)) return;
            mut.WaitOne();
            switch (method)
            {
                case Method.Add:
                    AddSerial(ItemName);
                    mut.ReleaseMutex();
                    break;
                case Method.Delete:
                    DeletSerial(ItemName);
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
                    Responsitory opcRes = Opcclient.GetValues();
                    if (opcRes.Items.Count > 0)
                    {
                        for (int i = 0; i < opcRes.Items.Count; i++)
                        {
                            if (opcRes.Items[i] != null)
                            {
                                ItemValues.Add(new ItemValueModel() { ItemName = opcRes.Items[i].ItemName, value = Convert.ToDouble(opcRes.Items[i].Value) });
                            }
                        }
                    }
                    var S7Res = S7Client.GetValues();
                    if (S7Res.Count > 0)
                    {
                        for (int i = 0; i < S7Res.Count; i++)
                        {
                            if (S7Res[i] != null)
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
            //_timeNow++;
            double date = DateTime.Now.ToOADate();
            if (ItemValues.Count > 0)
            {
                for (int i = 0; i < ItemValues.Count; i++)
                {
                    //Model.SyncRoot;
                    if(Model.Series!=null && Model.Series.Count > 0)
                    {
                        for (int n = 0; n < Model.Series.Count; n++)
                        {
                           if( Model.Series[n].Title== ItemValues[i].ItemName)
                            {
                                LineSeries s = (LineSeries)Model.Series[n];
                                s.Points.Add(new DataPoint(date, ItemValues[i].value));
                                if(s.Points.Count>200)
                                {
                                    s.Points.RemoveAt(0);
                                }
                                Model.InvalidatePlot(true);
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
            if (LocalServerTemp != OPCClientModel.SelectServer)
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
            dto.ItemMethod = (Method)Enum.Parse(typeof(Method), updateMethod);

            UpdateOPCItem(dto);
        }
        /// <summary>
        /// 注册或者删除OPCItem
        /// </summary>
        /// <param name="dto"></param>
        private void UpdateOPCItem(OPCDto dto)
        {

            if (dto == null) return;
            var res = Opcclient.UpdateItem(dto);
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
            Responsitory res = Opcclient.ConnectServer(dto);
            OPCClientModel.Items.Clear();
            if (res.Status)
            {
                IList<string> ServerList = (IList<string>)res.Obj;
                //List<string> list = (List<string>)res.Obj;
                if (ServerList != null && ServerList.Count > 0)
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
            if (string.IsNullOrEmpty(serverName)) return;
            OPCDto dto = new OPCDto();
            dto.HostName = OPCClientModel.HostName;
            dto.ProgID = serverName;

            Responsitory res = Opcclient.Disconnect(dto);
            if (res.Status)
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
            dto.DA_Version = OPCClientModel.SelectDaver;
           Responsitory res = Opcclient.GetOPCServerList(dto);
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



        #endregion

        #region S7Comm
        public S7Model S7Model { get; set; }
        public DelegateCommand<string> S7ConnectCommand { get; set; }

        public DelegateCommand S7AddItemCommand { get; set; }


        private void S7Connect(string MethodSingle)
        {
            S7Dto dto = new S7Dto();
            if (MethodSingle == "Connect")
            {
                if (string.IsNullOrEmpty(S7Model.HostIP)) S7Model.HostIP = "192.168.10.21";
                dto.HostIP = S7Model.HostIP;
                dto.Rock = int.Parse(S7Model.SelectRock);
                dto.Slot = int.Parse(S7Model.SeletcSlot);
                Responsitory res = S7Client.ConnectServer(dto);
                if (res.Status)
                {
                    S7Model.ValidHostCollect.Clear();
                    S7Model.ValidHostCollect.Add(dto.HostIP + "_" + dto.Rock + "_" + dto.Slot);
                    S7Model.SelectHostName = S7Model.ValidHostCollect.FirstOrDefault();
                }
            }
            if (MethodSingle == "DisConnect")
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

                    Responsitory res = S7Client.Disconnect(dto);
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
            dto.Mehtod = Method.Add;

            var res = S7Client.UpDateItemList(dto);
            if (res.Status)
            {
                UpDateChart(((S7ItemModel)res.Obj).ItemName, dto.Mehtod);
            }


        }



        #endregion


    }
}
