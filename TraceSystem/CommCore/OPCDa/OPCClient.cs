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
using Trace.Industry.Extenal;

namespace TraceSystem.CommCore.OPCDa
{
    public class OpcClient:IOPCClient
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
        private static ConcurrentDictionary<string, ClientModel> ServerDic=new ConcurrentDictionary<string, ClientModel>();
        private static Dictionary<string, Item> ItemsDic = new Dictionary<string, Item>();
        /// <summary>
        /// 获取OPC服务器列表,返回list集合
        /// </summary>
        /// <param name="DA_Version">DA 版本</param>
        /// <param name="host">主机名称</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Respostory GetOPCServerList(string DA_Version= "DA20", string host = "127.0.0.1")
        {
            List<string> list = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(host)) return new Respostory(false,"主机名是空");
                switch (DA_Version)
                {
                    case "DA10":
                        Opc.Server[] serverList10 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_10, host, null);
                        foreach (Opc.Server server in serverList10)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true,list);
                    case "DA20":
                        Opc.Server[] serverList20 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_20, host, null);
                        foreach (Opc.Server server in serverList20)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                    case "DA30":
                        Opc.Server[] serverList30 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_30, host, null);
                        foreach (Opc.Server server in serverList30)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                    default:
                        Opc.Server[] serverList201 = ServerEnumerator.GetAvailableServers(Specification.COM_DA_20, host, null);
                        foreach (Opc.Server server in serverList201)
                        {
                            list.Add(server.Name);
                        };
                        return new Respostory(true, list);
                }
            }
            catch (Exception ex)
            {
                return new Respostory(false,ex.Message);
            }
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ProgID">服务器列表所选则的服务器名称</param>
        /// <param name="host">主机名称</param>
        /// <returns></returns>
        public Respostory ConnectServer(string ProgID, string host,int updateRate=1000)
        {
            try
            {
                string url = "opcda://" + host + "/" + ProgID;
                foreach (string item in ServerDic.Keys)
                {
                    if(item==url)
                        return new Respostory(false, "服务器已经连接,如果实际没有连接,需要重启软件并再次连接");
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
                    //连接成功后实例化订阅组,并设置组参数
                    ServerDic[url].SubscriptionState = new SubscriptionState();
                    ServerDic[url].SubscriptionState.Name = url + Guid.NewGuid().ToString();
                    // The handle assigned by the server to the group.
                    ServerDic[url].SubscriptionState.ServerHandle = null;          
                    // The handle assigned by the client to the group.
                    ServerDic[url].SubscriptionState.ClientHandle = Guid.NewGuid().ToString();     
                    ServerDic[url].SubscriptionState.Active = true;
                    // The refresh rate is 1 second. -> 1000/
                    ServerDic[url].SubscriptionState.UpdateRate = updateRate;
                    // When the dead zone value is set to 0, the server will notify the group of any data changes in the group.
                    ServerDic[url].SubscriptionState.Deadband = 0;                               
                    ServerDic[url].SubscriptionState.Locale = null;
                    ServerDic[url].ItemsDic = new ConcurrentDictionary<string, ItemModel>();
                    ServerDic[url].Subscription =(Subscription)ServerDic[url].Server.CreateSubscription(ServerDic[url].SubscriptionState);
                    return new Respostory(true, "连接成功");
                }
                return new Respostory(false,"连接失败,未知原因");
            }
            catch (Exception ex)
            {
                return new Respostory(false,ex.Message);
            }
        }
        /// <summary>
        /// 添加变量到变量字典
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Respostory AddItem(string itemName,string url)
        {
            try
            {
                if (string.IsNullOrEmpty(itemName)) return new Respostory(false, "变量名为空不能添加");
                if (ServerDic[url].ItemsDic.ContainsKey(itemName))
                    return new Respostory(false, "变量已经存在,不允许重复添加");

                ServerDic[url].ItemsDic.TryAdd(itemName, new ItemModel());
                ServerDic[url].ItemsDic[itemName].Item.Active = true;
                ServerDic[url].ItemsDic[itemName].Item.ClientHandle = new Guid().ToString();
                ServerDic[url].ItemsDic[itemName].Item.ItemPath = null;
                ServerDic[url].ItemsDic[itemName].Item.ItemName = itemName;
                //ServerDic[url].ItemsDic[itemName].Item.ReqType = VT_I2;
                //重新注册到订阅..
                ServerDic[url].Subscription.AddItems(ItemsDic.Values.ToArray());
                ServerDic[url].Subscription.DataChanged += OnDataChange;
                return new Respostory(true, "变量:" + itemName + ",添加成功");
            }
            catch (Exception ex)
            {
                return new Respostory(false,"添加失败,错误信息:"+ex.Message);
            }
        }
        /// <summary>
        /// 从字典删除变量
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Respostory SubItem(string itemName, string url)
        {
            if (string.IsNullOrEmpty(itemName)) return new Respostory(false, "变量名为空不能添加");
            if (!ServerDic[url].ItemsDic.ContainsKey(itemName)) return new Respostory(false, "不包含该变量");
            ServerDic[url].ItemsDic.TryRemove(itemName,out ItemModel value);
            return new Respostory(true, "变量:" + itemName + ",删除成功");
        }
        /// <summary>
        /// 启动停止更新
        /// </summary>
        /// <param name="ProgID"></param>
        /// <param name="host"></param>
        /// <param name="start"></param>
        public void UpdateAsync(string ProgID, string host,bool start)
        {
            string url = "opcda://" + host + "/" + ProgID;

            if(!start)
            {
                if (ServerDic[url].MyThread != null)
                {
                    ServerDic[url].MyThread.Interrupt();
                    return;
                }
            }
            else
            {
                if (ServerDic[url].MyThread != null)
                {
                    ServerDic[url].MyThread.Start();
                    return;
                }
                ParameterizedThreadStart parameterized = new ParameterizedThreadStart(ReadAsync);
                ServerDic[url].MyThread = new System.Threading.Thread(parameterized);
                ServerDic[url].MyThread.Name = url;
                ServerDic[url].MyThread.IsBackground = true;
                ServerDic[url].MyThread.Start(url);
            }
        }
        private void ReadAsync(object obj)
        {
            //遍历出所有的Item,添加到列表  然后注册到订阅
            string str=obj as string;
            List<Item> tempItems = new List<Item>();

            foreach (ItemModel item in ServerDic[str].ItemsDic.Values.ToArray())
            {
                tempItems.Add(item.Item);
            }
            //(ItemModel)ServerDic[str].ItemsDic.Values
            //Opc.Da.ItemValueResult[]
            while (true)
            {
                Opc.Da.ItemValueResult[] values = ServerDic[str].Subscription.Read(tempItems.ToArray());

                foreach (var item in values)
                {
                    if (ServerDic[str].ItemsDic.ContainsKey(item.ItemName))
                        ServerDic[str].ItemsDic[item.ItemName].Value = item;
                }
            }
        }
        /// <summary>
        ///  断开指定服务器
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public Respostory Disconnect(string serverName)
        {
            try
            {
                foreach (string item in ServerDic.Keys)
                {
                    if(item==serverName)
                    {
                        ServerDic[item].Subscription.DataChanged -= OnDataChange;
                        ServerDic[item].Subscription.RemoveItems(ServerDic[item].Subscription.Items);
                        ServerDic[item].Server.CancelSubscription(ServerDic[item].Subscription);
                        ServerDic[item].Subscription.Dispose();
                        ServerDic[item].Server.Disconnect();
                        ServerDic.TryRemove(item,out ClientModel value1);
                        ServerDic.TryRemove(item, out ClientModel value2);
                        return new Respostory(true, "断开成功");
                    }
                }
                return new Respostory(true, "已断开");
            }
            catch (Exception ex)
            {

                return new Respostory(false, "断开失败,错误信息:"+ex.Message);
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
                    ServerDic[item].Subscription.DataChanged -=OnDataChange;
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

        }

        public void OnReadComplete(object requestHandle, Opc.Da.ItemValueResult[] values)
        {

        }


        public void BrowseItem(string ProgID, string host)
        {
            string url = "opcda://" + host + "/" + ProgID;

            if(ServerDic.ContainsKey(url))
            {
                if(ServerDic[url].Server != null)
                {
                    //foreach (string item in ServerDic[url].Server.Browse(OPCBROWSETYPE.OPC_FLAT))
                    //{
                    //    Console.WriteLine(item);
                    //}
                }
            }


            

        }
    }
}
