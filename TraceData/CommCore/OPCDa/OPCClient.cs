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
using TraceData.Extension;

namespace TraceData.CommCore.OPCDa
{
    public class OpcClient : IOPCClient
    {
        public OpcClient()
        {
            TaskList = new List<Task>();
        }

        /// <summary>
        /// 任务列表
        /// </summary>
         List<Task> TaskList;
        /// <summary>
        /// OPC 工厂类
        /// </summary>
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
                if (string.IsNullOrEmpty(dto.HostName)) return new Respostory(false, "主机名不能是空");
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
            if (string.IsNullOrEmpty(dto.HostName)) return new Respostory(false, "主机名不能是空");
            if (string.IsNullOrEmpty(dto.ProgID)) return new Respostory(false, "OPCProgID不能为空");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            foreach (string item in ServerDic.Keys)
            {
                if(item.Contains(url))
                   return new Respostory(true, ServerDic[url].Server);
            }
            ServerDic.TryAdd(url, new ClientModel());
            try
            {
               if(ServerDic[url].MyTask==null)
                {
                    ServerDic[url].MyTask = Task<Respostory>.Factory.StartNew(() =>
                    {
                       return ConnServer(url);
                    },TaskCreationOptions.LongRunning);
                    return ServerDic[url].MyTask.Result;
                }
               else
                {
                    return new Respostory(false, "启动连接任务失败");
                }
            }
            catch (Exception ex)
            {
                return new Respostory(false, ex.Message);
            }
        }


        private Respostory ConnServer(string url)
        {
            //如果不存在该服务器,则添加一个新项,并实力化一个ClientModel对象,
            ServerDic.TryAdd(url, new ClientModel());
            // ClientModel tempClient = new ClientModel();
            //使用新加的对象,实例化服务器对象
            ServerDic[url].Server = new Opc.Da.Server(Factory, null);
            ServerDic[url].Server.Url = new Opc.URL(url);
            ServerDic[url].Server.Connect();

            if (ServerDic[url].Server.IsConnected)
            {

                ServerDic[url].Server.ServerShutdown += Server_ServerShutdown;
                //展开所有Item
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
                ServerDic[url].SubscriptionState.UpdateRate = 500;
                // When the dead zone value is set to 0, the server will notify the group of any data changes in the group.
                ServerDic[url].SubscriptionState.Deadband = 0;
                ServerDic[url].SubscriptionState.Locale = null;
                ServerDic[url].ItemsDic = new ConcurrentDictionary<string, ItemModel>();
                ServerDic[url].Subscription = (Subscription)ServerDic[url].Server.CreateSubscription(ServerDic[url].SubscriptionState);
                //返回连接的对象,和对象所有的Tag列表
                return new Respostory(true, ServerDic[url].Server, ServerDic[url].mItempNameList);
            }
            return new Respostory(false, "连接失败,未知原因");
        }
        private void Server_ServerShutdown(string reason)
        {
         
        }


        /// <summary>
        /// 添加/删除变量到更新列表
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Respostory UpdateItem(OPCDto dto)
        {
            if (string.IsNullOrEmpty(dto.HostName) || string.IsNullOrEmpty(dto.ProgID) || !dto.IsConnect)
                return new Respostory(false, "要添加的对象信息为空或者没有连接服务器");
            string url = "opcda://" + dto.HostName + "/" + dto.ProgID;
            if (string.IsNullOrEmpty(dto.ItemName)) return new Respostory(false, "变量名为空不能添加");
            if (ServerDic[url].ItemsDic.ContainsKey(dto.ItemName))
                return new Respostory(false, "变量已经存在,不允许重复添加");
            try
            {
                switch (dto.ItemMethod)
                {
                    case Method.Add:
                        ServerDic[url].Subscription.DataChanged -= OnDataChange;
                        bool res= ServerDic[url].ItemsDic.TryAdd(dto.ItemName, new ItemModel());
                        if(res)
                        {
                            ServerDic[url].ItemsDic[dto.ItemName].Item = new Item();
                            ServerDic[url].ItemsDic[dto.ItemName].Item.Active = true;
                            ServerDic[url].ItemsDic[dto.ItemName].Item.ClientHandle = new Guid().ToString();
                            ServerDic[url].ItemsDic[dto.ItemName].Item.ItemPath = null;
                            ServerDic[url].ItemsDic[dto.ItemName].Item.ItemName = dto.ItemName;
                            //ServerDic[url].ItemsDic[itemName].Item.ReqType = VT_I2;
                            //重新注册到订阅..
                            List<Item> tempAddlist = new List<Item>();
                            List<string> reslist = new List<string>();
                            foreach (var item in ServerDic[url].ItemsDic.Values)
                            {
                                tempAddlist.Add(item.Item);
                                reslist.Add(item.Item.ItemName);
                            }
                            ServerDic[url].Subscription.AddItems(tempAddlist.ToArray());
                            ServerDic[url].Subscription.DataChanged += OnDataChange;
                            return new Respostory(true, "变量:" + dto.ItemName + ",添加成功", reslist);
                        }
                        else
                        {
                            return new Respostory(false, "变量:" + dto.ItemName + ",添加失败");
                        }
                       
                    case Method.Delete:
                        ServerDic[url].Subscription.DataChanged -= OnDataChange;
                        ServerDic[url].ItemsDic.TryRemove(dto.ItemName,out _);
                        
                        //ServerDic[url].ItemsDic[itemName].Item.ReqType = VT_I2;
                        //重新注册到订阅..
                        List<Item> tempDellist = new List<Item>();
                        foreach (var item in ServerDic[url].ItemsDic.Values)
                        {
                            tempDellist.Add(item.Item);
                        }
                        if(tempDellist.Count>0)
                        {
                            ServerDic[url].Subscription.AddItems(tempDellist.ToArray());
                            ServerDic[url].Subscription.DataChanged += OnDataChange;
                        }
                       
                        return new Respostory(true, "变量:" + dto.ItemName + ", 删除成功");

                    default:
                        return new Respostory(false, "变量:" + dto.ItemName + ", 操作参数错误");
                }
            }
            catch (Exception ex)
            {
                return new Respostory(false, "操作失败,错误信息:" + ex.Message);
            }
        }

        private void UpdateAsync1(string url)
        {
            if(ServerDic[url].MyTask != null)
            {

            }

        }


      /// <summary>
      ///  返回字典中所有有效的Values对象.
      /// </summary>
      /// <returns></returns>
        public List<ItemValue> GetValue()
        {
            List<ItemValue> list = new List<ItemValue>();
            foreach (var item in ServerDic)
            {
                foreach (var item1 in item.Value.ItemsDic)
                {
                    list.Add(item1.Value.Value);
                }
            }

            return list;
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
        public List<ItemValue> GetValues()
        {
            List<ItemValue> list = new List<ItemValue>();
            foreach (var item in ServerDic)
            {
                foreach (var item1 in item.Value.ItemsDic)
                {
                    list.Add(item1.Value.Value);
                }
            }

            return list;
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
