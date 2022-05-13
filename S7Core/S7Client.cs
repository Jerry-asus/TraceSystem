using Common;
using S7Core.Models;
using Sharp7;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S7Core
{
    public class S7Client:Is7Client
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static ConcurrentDictionary<string, S7ClientModel> S7dic = new ConcurrentDictionary<string, S7ClientModel>();
        readonly static object _lock = new object();
        public static bool IsReading { get; set; }
        public Responsitory ConnectServer(S7Dto dto)
        {
            string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
            if (S7dic.ContainsKey(url))
            {
                Logger.Error("连接已存在, 不能重复连接");
                return new Responsitory(false, "连接已存在,不能重复连接");
            }
            try
            {
                if (S7dic.TryAdd(url, new S7ClientModel()))
                {
                    var res = S7dic[url].PLC.ConnectTo(dto.HostIP, dto.Rock, dto.Slot);
                    if (res != 0)
                    {
                        Logger.Fatal("连接失败,错误代码:" + S7dic[url].PLC.ErrorText(res).ToString());
                        return new Responsitory(false, "连接失败,错误代码:" + S7dic[url].PLC.ErrorText(res).ToString());
                    }
                    return new Responsitory(true, S7dic[url].PLC);
                }
                //}
                S7dic.TryRemove(url, out _);
                {
                    Logger.Error("创建连接失败");
                    return new Responsitory(false, "创建连接失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("创建连接失败"+ ex.Message);
                return new Responsitory(false, "连接失败:" + ex.Message);
            }
        }


        private string CreateKey()
        {
            string keystring = null;


            return keystring;
        }
        /// <summary>
        /// 注册变量
        /// </summary>
        /// <param name="listItems"></param>
        /// <returns></returns>
        public Responsitory RegistorItem(ref List<S7ItemModel> listItems)
        {
            if(listItems.Count>0)
            {

            }




            return null;
        }

        /// <summary>
        /// 添加和删除Item集合
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Responsitory UpDateItemList(S7Dto dto)
        {
            Logger.Info(dto.HostIP + "_" + dto.HostIP + "_" + dto.ItemData.ItemName + "_" + dto.Mehtod.ToString());
            switch (dto.Mehtod)
            {
                case Method.Add:
                    string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                    lock (_lock)
                    {
                        if (S7dic.ContainsKey(url) && S7dic[url].PLC != null && S7dic[url].PLC.Connected && dto.ItemData!=null)
                        {
                            byte[] dbBuffer = new byte[dto.ItemData.Length];
                            int obj = S7dic[url].PLC.DBRead(dto.ItemData.DBNumber, dto.ItemData.StartAddr, dto.ItemData.Length, dbBuffer);
                            if (obj != 0)
                            {
                                Logger.Error(dto.ItemData.ItemName+ "Item不存在");
                                return new Responsitory(false, "Item不存在");
                            }
                            if (S7dic[url].ItemData.Contains(dto.ItemData))
                            {
                                Logger.Warn(dto.ItemData.ItemName + "已存在,不能重复添加");
                                return new Responsitory(false, "Item已存在,不能重复添加");
                            }
                            S7dic[url].ItemData.Add(dto.ItemData);
                            if (IsReading)
                                IsReading = false;
                            ReadValue();
                            return new Responsitory(true, dto.ItemData);
                        }
                        else
                        {
                            Logger.Warn("请先连接PLC,然后再添加tag地址");
                            return new Responsitory(false, "请先连接PLC,然后再添加tag地址");
                        }
                    }
                case Method.Delete:
                    string url1 = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                    if (dto.ItemData.ItemName == null) return new Responsitory(false, "Tag地址为空");
                    lock (_lock)
                    {
                        if (S7dic.ContainsKey(url1) && S7dic[url1].ItemData.Contains(dto.ItemData))
                        {
                            S7dic[url1].ItemData.Remove(dto.ItemData);
                            int i = 0;
                            foreach (var item in S7dic)
                            {
                                for (int n = 0; n < item.Value.ItemData.Count; i++)
                                {
                                    i++;
                                }
                            }
                            if (i == 0)
                                IsReading = false;
                            Logger.Info(dto.ItemData.ItemName + "删除成功");
                            return new Responsitory(true, dto.ItemData);
                        }
                        else
                        {
                            Logger.Error(dto.ItemData.ItemName + "删除失败");
                            return new Responsitory(false, "删除Item失败");
                        }
                    }
                default:
                    Logger.Error("执行" + dto.Mehtod + "失败");
                    return new Responsitory(false, "执行" + dto.Mehtod + "失败");
            }
        }

        /// <summary>
        /// 循环读取Value
        /// </summary>
        private void ReadValue()
        {
            if (IsReading) return;

            IsReading = true;
            Logger.Info(IsReading + "=true,+启动循环");
            Action readFromTread = () =>
            {
                while (IsReading)
                {
                    
                    if (S7dic.Count > 0)
                    {
                        foreach (var item in S7dic)
                        {
                            if (item.Value!=null && item.Value.PLC != null)
                            {
                                if (item.Value.PLC.Connected)
                                     // item.Value.PLC.Connect();
                                {
                                    if (item.Value != null && item.Value.ItemData.Count > 0)
                                    {
                                        for (int i = 0; i < item.Value.ItemData.Count; i++)
                                        {
                                            if (item.Value.ItemData[i] != null)
                                            {
                                                Thread.Sleep(20);
                                                switch (item.Value.ItemData[i].Datatype)
                                                {
                                                    case S7DataType.Word:
                                                        byte[] db1Buffer = new byte[item.Value.ItemData[i].Length];
                                                        int res1 = (ushort)item.Value.PLC.DBRead(item.Value.ItemData[i].DBNumber, item.Value.ItemData[i].StartAddr, item.Value.ItemData[i].Length, db1Buffer);
                                                        if (res1 == 0)
                                                            item.Value.ItemData[i].Value =S7.GetLWordAt(db1Buffer, 0);
                                                        break;
                                                    case S7DataType.Int:
                                                        byte[] db2Buffer = new byte[item.Value.ItemData[i].Length];
                                                        int res2 = ((ushort)item.Value.PLC.DBRead(item.Value.ItemData[i].DBNumber, item.Value.ItemData[i].StartAddr, item.Value.ItemData[i].Length, db2Buffer));
                                                        if (res2 == 0)
                                                        {
                                                            item.Value.ItemData[i].Value = S7.GetIntAt(db2Buffer, 0);
                                                        }
                                                        break;
                                                    case S7DataType.DWord:
                                                        byte[] db3Buffer = new byte[item.Value.ItemData[i].Length];
                                                        int res3 = item.Value.PLC.DBRead(item.Value.ItemData[i].DBNumber, item.Value.ItemData[i].StartAddr, item.Value.ItemData[i].Length, db3Buffer);
                                                        item.Value.ItemData[i].Value = S7.GetDWordAt(db3Buffer, 0);
                                                        break;
                                                    case S7DataType.DInt:
                                                        byte[] db4Buffer = new byte[item.Value.ItemData[i].Length];
                                                        int res4 = (item.Value.PLC.DBRead(item.Value.ItemData[i].DBNumber, item.Value.ItemData[i].StartAddr, item.Value.ItemData[i].Length, db4Buffer));
                                                        if (res4 == 0)
                                                            item.Value.ItemData[i].Value = S7.GetDIntAt(db4Buffer, 0);
                                                        break;
                                                    case S7DataType.Real:
                                                        byte[] db5Buffer = new byte[item.Value.ItemData[i].Length];
                                                        int res5 = item.Value.PLC.DBRead(item.Value.ItemData[i].DBNumber, item.Value.ItemData[i].StartAddr, item.Value.ItemData[i].Length, db5Buffer);
                                                        if (res5 == 0)
                                                        {
                                                            item.Value.ItemData[i].Value = S7.GetDIntAt(db5Buffer, 0);
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Task.Factory.StartNew(readFromTread);
        }
        /// <summary>
        /// UI读取values,返回结构为S7ItemModel
        /// </summary>
        /// <returns></returns>
        public List<S7ItemModel> GetValues()
        {
            Logger.Info("开始读取变量值");
            List<S7ItemModel> items = new List<S7ItemModel>();

            foreach (var item in S7dic)
            {
                Logger.Info("变量列表存在数据,开始取数据");
                if (item.Value!=null && item.Value.ItemData.Count > 0)
                {
                    items.AddRange(item.Value.ItemData);
                }
            }
            return items;
        }
        /// <summary>
        ///返回简单变量名+值+数据类型组成的List
        /// </summary>
        /// <returns></returns>
        public List<S7SimpleItemModel> GetSimpleValues()
        {
            Logger.Info("开始读取变量值");
            List<S7SimpleItemModel> items = new List<S7SimpleItemModel>();

            foreach (var item in S7dic)
            {
                if (item.Value != null && item.Value.ItemData.Count > 0)
                {
                    Logger.Info("变量列表存在数据,开始取数据");
                    for (int i = 0; i < item.Value.ItemData.Count; i++)
                    {
                        items.Add(new S7SimpleItemModel() { ItemName = item.Value.ItemData[i].ItemName,Value= item.Value.ItemData[i].Value,Datatype = item.Value.ItemData[i].Datatype});
                    }
                }
            }
            return items;
        }
        public void Disconnect()
        {
            try
            {
                foreach (var item in S7dic)
                {
                    if (item.Value != null)
                    {
                        if (item.Value.PLC != null)
                        {
                            item.Value.PLC.Disconnect();
                        }
                           
                        S7dic.Remove(item.Key, out _);
                        Logger.Info(item.Key + "断开成功!");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("断开失败"+ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Responsitory Disconnect(S7Dto dto)
        {
            try
            {
                string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                if (string.IsNullOrWhiteSpace(url))
                {
                    Logger.Warn("服务器地址是空");
                    return new Responsitory(false, "服务器地址是空");
                }
                if (S7dic.ContainsKey(url))
                {
                    if (S7dic[url].PLC != null)
                        S7dic[url].PLC.Disconnect();
                    S7dic.Remove(url, out _);
                    Logger.Info(url + "断开成功");
                    return new Responsitory(true, dto);
                }
                Logger.Error(url + "需要断开的服务器尚未连接");
                return new Responsitory(false, "需要断开的服务器尚未连接");
            }
            catch (Exception ex)
            {
                Logger.Error("断开失败,错误信息:" + ex.Message);
                return new Responsitory(false, "断开失败,错误信息:" + ex.Message);
            }
        }


        /// <summary>
        /// 注册变量后对变量进行分组,分组结束后开始循环读取变量值
        /// </summary>
        private void GroupVars()
        {

        }


    }
}
