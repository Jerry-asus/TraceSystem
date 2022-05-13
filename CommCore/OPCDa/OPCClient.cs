using Opc;
using Opc.Da;
using OpcCom;
using OpcRcw.Da;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Trace.Industry.Extenal;

namespace TraceSystem.CommCore.OPCDa
{
    public class OpcClient : IOPCClient
    {
        public OpcClient()
        {

        }

        private OpcCom.Factory Factory = new OpcCom.Factory();
        /// <summary>
        /// 服务器枚举器
        /// </summary>
        private OpcCom.ServerEnumerator ServerEnumerator = new ServerEnumerator();
        /// <summary>
        /// 订阅器
        /// </summary>
        private static SubscriptionState mMonitoringGroup = new SubscriptionState();
        /// <summary>
        /// 订阅属性
        /// </summary>
        /// private static Subscription mMonitoringSubscription = null;
        private static ConcurrentDictionary<string, ClientModel> ServerDic = new ConcurrentDictionary<string, ClientModel>();
        private static Dictionary<string, Item> ItemsDic = new Dictionary<string, Item>();
        /// <summary>
        /// 获取OPC服务器列表,返回list集合
        /// </summary>
        /// <param name="DA_Version">DA 版本</param>
        /// <param name="host">主机名称</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Respostory GetOPCServerList(OPCDto dto)
        {
            List<string> list = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(dto.HostName)) return new Respostory(false, "主机名是空");
                switch (dto.DA_Version)
                {
                    case "DA10":
                        Opc.Server[] serverList10 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_10, dto.HostName, null);
                        foreach (Opc.Server server in serverList10)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                    case "DA20":
                        Opc.Server[] serverList20 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_20, dto.HostName, null);
                        foreach (Opc.Server server in serverList20)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                    case "DA30":
                        Opc.Server[] serverList30 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_30, dto.HostName, null);
                        foreach (Opc.Server server in serverList30)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                    default:
                        Opc.Server[] serverList201 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_20, dto.HostName, null);
                        foreach (Opc.Server server in serverList201)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                }
            }
            catch (Exception ex)
            {
                return new Respostory(false, ex.Message);
            }
        }
        /// <summary>
        /// 连接服务器/返回当前连接对象/ 如果已经连接则返回字典中已连接对象
        /// </summary>
        /// <param name="ProgID">服务器列表所选则的服务器名称</param>
        /// <param name="host">主机名称</param>
        /// <returns></returns>
        public Respostory ConnectServer(OPCDto dto)
        {
            try
            {
                string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
                foreach (string item in ServerDic.Keys)
                {
                    return new Respostory(true, ServerDic[url].Server);
                }
                //如果不存在该服务器,则添加一个新项,并实力化一个ClientModel对象,
                ServerDic.TryAdd(url, new ClientModel());
                // ClientModel tempClient = new ClientModel();
                //使用新加的对象,实例化服务器对象
                ServerDic[url].Server = new Opc.Da.Server(Factory, null);
                ServerDic[url].Server.Url = new Opc.URL(url);
                ServerDic[url].Server.Connect();
                if (ServerDic[url].Server.IsConnected)
                {
                    GetAllItemInServer(url);

                    //连接成功后实例化订阅组,并设置组参数
                    ServerDic[url].SubscriptionState = new SubscriptionState();
                    ServerDic[url].SubscriptionState.Name = url + Guid.NewGuid().ToString();
                    // The handle assigned by the server to the group.
                    ServerDic[url].SubscriptionState.ServerHandle = null;
                    // The handle assigned by the client to the group.
                    ServerDic[url].SubscriptionState.ClientHandle = Guid.NewGuid().ToString();
                    ServerDic[url].SubscriptionState.Active = true;
                    // The refresh rate is 1 second. -> 1000/
                    ServerDic[url].SubscriptionState.UpdateRate = dto.Rate;
                    // When the dead zone value is set to 0, the server will notify the group of any data changes in the group.
                    ServerDic[url].SubscriptionState.Deadband = 0;
                    ServerDic[url].SubscriptionState.Locale = null;
                    ServerDic[url].ItemsDic = new ConcurrentDictionary<string, ItemModel>();
                    ServerDic[url].Subscription = (Subscription)ServerDic[url].Server.CreateSubscription(ServerDic[url].SubscriptionState);

                    ///展开变量列表,待完成
                    ///

                     
                    //返回连接的对象,和对象所有的Tag列表
                    return new Respostory(true, ServerDic[url].Server, ServerDic[url].mItempNameList);
                }
                return new Respostory(false, "连接失败,未知原因");
            }
            catch (Exception ex)
            {
                return new Respostory(false, ex.Message);
            }
        }
        /// <summary>
        /// 添加变量到更新列表
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Respostory AddItem(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            try
            {
                if (string.IsNullOrEmpty(dto.ItemName)) return new Respostory(false, "变量名为空不能添加");
                if (ServerDic[url].ItemsDic.ContainsKey(dto.ItemName))
                    return new Respostory(false, "变量已经存在,不允许重复添加");

                ServerDic[url].ItemsDic.TryAdd(dto.ItemName, new ItemModel());
                ServerDic[url].ItemsDic[dto.ItemName].Item = new Item();
                ServerDic[url].ItemsDic[dto.ItemName].Item.Active = true;
                ServerDic[url].ItemsDic[dto.ItemName].Item.ClientHandle = new Guid().ToString();
                ServerDic[url].ItemsDic[dto.ItemName].Item.ItemPath = null;
                ServerDic[url].ItemsDic[dto.ItemName].Item.ItemName = dto.ItemName;
                //ServerDic[url].ItemsDic[itemName].Item.ReqType = VT_I2;
                //重新注册到订阅..
                List<Item> temlist = new List<Item>();
                foreach (var item in ServerDic[url].ItemsDic.Values)
                {
                    temlist.Add(item.Item);
                }
                ServerDic[url].Subscription.AddItems(temlist.ToArray());
                ServerDic[url].Subscription.DataChanged += OnDataChange;
                return new Respostory(true, "变量:" + dto.ItemName + ",添加成功");
            }
            catch (Exception ex)
            {
                return new Respostory(false, "添加失败,错误信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 从字典删除变量
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Respostory SubItem(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            if (string.IsNullOrEmpty(dto.ItemName)) return new Respostory(false, "变量名为空不能添加");
            if (!ServerDic[url].ItemsDic.ContainsKey(dto.ItemName)) return new Respostory(false, "不包含该变量");
            ServerDic[url].ItemsDic.TryRemove(dto.ItemName, out ItemModel value);
            return new Respostory(true, "变量:" + dto.ItemName + ",删除成功");
        }
        /// <summary>
        /// 启动停止更新
        /// </summary>
        /// <param name="ProgID"></param>
        /// <param name="host"></param>
        /// <param name="start"></param>
        public void UpdateAsync(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return;// new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;

            if (!dto.Start)
            {
                if (ServerDic[url].MyThread != null)
                {
                    ServerDic[url].MyThread.Interrupt();
                    //return new Respostory(true, "更新停止");
                }
            }
            else
            {
                if (ServerDic[url].MyThread != null && ServerDic[url].MyThread.IsAlive)
                {
                    return;
                }

                if (ServerDic[url].MyThread != null && !ServerDic[url].MyThread.IsAlive)
                {
                    ServerDic[url].MyThread.Start();
                    return;
                    //return new Respostory(true, "更新开始");
                }
                ParameterizedThreadStart parameterized = new ParameterizedThreadStart(ReadAsync);
                ServerDic[url].MyThread = new Thread(parameterized);
                ServerDic[url].MyThread.Name = url;
                ServerDic[url].MyThread.IsBackground = true;
                ServerDic[url].MyThread.Start(url);
                //return new Respostory(true, "更新开始");
            }
        }
        private void ReadAsync(object obj)
        {
            string str = obj as string;
            while (true)
            {
                if (ServerDic[str].Subscription != null)
                {
                    Opc.Da.ItemValueResult[] values = ServerDic[str].Subscription.Read(ServerDic[str].Subscription.Items);
                    //MessageBox.Show(values.ToString());
                    foreach (var item in values)
                    {
                        //MessageBox.Show(item.Value.ToString());
                        if (!string.IsNullOrEmpty(item.ItemName) && ServerDic[str].ItemsDic.ContainsKey(item.ItemName))
                            ServerDic[str].ItemsDic[item.ItemName].Value = item;
                    }
                }
            }
        }

        public object GetValue(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return null; // new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            if (ServerDic[url].ItemsDic.ContainsKey(dto.ItemName))
                return ServerDic[url].ItemsDic[dto.ItemName].Value;
            else
                return null;
        }
        /// <summary>
        ///  断开指定服务器
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public Respostory Disconnect(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            try
            {
                foreach (string item in ServerDic.Keys)
                {
                    if (item == url)
                    {
                        ServerDic[item].Subscription.DataChanged -= OnDataChange;
                        ServerDic[item].Subscription.RemoveItems(ServerDic[item].Subscription.Items);
                        ServerDic[item].Server.CancelSubscription(ServerDic[item].Subscription);
                        ServerDic[item].Subscription.Dispose();
                        ServerDic[item].Server.Disconnect();
                        ServerDic.TryRemove(item, out ClientModel value1);
                        ServerDic.TryRemove(item, out ClientModel value2);
                        return new Respostory(true, "断开成功");
                    }
                }
                return new Respostory(true, "已断开");
            }
            catch (Exception ex)
            {

                return new Respostory(false, "断开失败,错误信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 断开所有的连接
        /// </summary>
        /// <returns></returns>
        public Respostory Disconnect()
        {
            try
            {
                foreach (string item in ServerDic.Keys)
                {
                    ServerDic[item].Subscription.DataChanged -= OnDataChange;
                    ServerDic[item].Subscription.RemoveItems(ServerDic[item].Subscription.Items);
                    ServerDic[item].Server.CancelSubscription(ServerDic[item].Subscription);
                    ServerDic[item].Subscription.Dispose();
                    ServerDic[item].Server.Disconnect();
                    ServerDic.TryRemove(item, out ClientModel value);
                    return new Respostory(true, "断开成功");
                }
                return new Respostory(true, "已断开");
            }
            catch (Exception ex)
            {
                return new Respostory(false, "断开失败,错误信息:" + ex.Message);
            }
        }

        public void OnDataChange(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            //对于每一个结果进行遍历
            foreach (ItemValueResult item in values)
            {
                if(item.ClientHandle == null){//服务器发过来的无用信息。
                    continue;
                }
                foreach (var dicNode in ServerDic)
                {
                    //如果句柄相同,找到对应的key值.
                    if (dicNode.Value.Subscription.ClientHandle == subscriptionHandle)
                    {
                        ///对于该服务器下的所有Item遍历出来挨个赋值.
                        
                        foreach (var obj in ServerDic[dicNode.Key].ItemsDic.Values)
                        {
                            if(obj.Item.ItemName== item.ItemName)
                            {
                                ServerDic[dicNode.Key].ItemsDic[obj.Item.ItemName].Value = item;
                                if (ServerDic[dicNode.Key].ItemsDic[obj.Item.ItemName].Value != null)
                                    ServerDic[dicNode.Key].ItemsDic[obj.Item.ItemName].DataType = item.Value.GetType().ToString();
                                else
                                    ServerDic[dicNode.Key].ItemsDic[obj.Item.ItemName].DataType = null;

                            }
                        }
                      
                    }
                }
            }
        }

        public void OnReadComplete(object requestHandle, Opc.Da.ItemValueResult[] values)
        {

        }
        List<ItemModel> tempList = new List<ItemModel>();
        /// <summary>
        /// 返回对应服务器中注册的Item的值集合
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Respostory GetValues(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            tempList.Clear();
            foreach (var item in ServerDic[url].ItemsDic)
            {
                    tempList.Add(new ItemModel() { DataType = item.Value.Value.Value.GetType().ToString(), Value = item.Value.Value, Item = item.Value.Item });
            }
            return new Respostory(true, tempList);
        }

        private string[] MonitoringItemNames;

        private void GetItemsInChildren(BrowseElement[] tParent,string url)
        {
            for (int i = 0; i < tParent.Length; i++)
            {
                if (tParent[i].IsItem == true)
                {
                    ServerDic[url].mItempNameList.Add(tParent[i].ItemName);
                }
                else
                {
                    BrowsePosition position;
                    ItemIdentifier tItemChild = new ItemIdentifier();
                    BrowseFilters tFilterChild = new BrowseFilters();
                    tFilterChild.BrowseFilter = Opc.Da.browseFilter.all;
                    tItemChild.ItemName = tParent[i].ItemName;
                    //tItemChild.ItemPath = tChildren[i].ItemPath;
                    BrowseElement[] tChildren = ServerDic[url].Server.Browse(tItemChild, tFilterChild, out position);
                    if (tChildren != null) 
                       GetItemsInChildren(tChildren,url);
                }
            }
        }

        private void GetAllItemInServer(string url)
        {
            ServerDic[url].mItempNameList = new List<string>();

            BrowsePosition position;
            ItemIdentifier tItem = new ItemIdentifier();
            BrowseFilters tFilter = new BrowseFilters();
            tFilter.BrowseFilter = Opc.Da.browseFilter.all;
            BrowseElement[] children = ServerDic[url].Server.Browse(tItem, tFilter, out position);
            GetItemsInChildren(children,url);
            MonitoringItemNames = new string[ServerDic[url].mItempNameList.Count];
            for (int i = 0; i < ServerDic[url].mItempNameList.Count; i++)
            {
                MonitoringItemNames[i] = ServerDic[url].mItempNameList[i];
            }
        }


      



    }
    }
