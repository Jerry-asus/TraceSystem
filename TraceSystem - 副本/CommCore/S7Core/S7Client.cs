using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TraceSystem.Extension;
using Sharp7;
using System.Threading.Tasks;

namespace TraceSystem.CommCore.S7Core
{
    public class S7Client:Is7Client
    {
        private static ConcurrentDictionary<string, S7ClientModel> S7dic = new ConcurrentDictionary<string, S7ClientModel>();
        readonly static object _lock = new object();
        public static bool IsReading { get; set; }
        public Respostory ConnectServer(S7Dto dto)
        {
            string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
            if (S7dic.ContainsKey(url)) return new Respostory(false, "连接已存在,不能重复连接");
            try
            {
                if (S7dic.TryAdd(url, new S7ClientModel()))
                {
                    var res = S7dic[url].PLC.ConnectTo(dto.HostIP, dto.Rock, dto.Slot);
                    if (res != 0) return new Respostory(false, "连接失败,错误代码:" + S7dic[url].PLC.ErrorText(res).ToString());

                    return new Respostory(true, S7dic[url].PLC);
                }
                //}
                S7dic.TryRemove(url, out _);
                return new Respostory(false, "创建连接失败");
            }
            catch (Exception ex)
            {
                return new Respostory(false, "连接失败:" + ex.Message);
            }
        }
        /// <summary>
        /// 添加和删除Item集合
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Respostory UpDateItemList(S7Dto dto)
        {
            switch (dto.Mehtod)
            {
                case UpdateMethod.Add:
                    string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                    // if (dto.ItemData.ItemName == null) return new S7Respostory(false, "Tag地址为空");
                    lock (_lock)
                    {
                        if (S7dic.ContainsKey(url) && S7dic[url].PLC != null && S7dic[url].PLC.Connected)
                        {
                            byte[] dbBuffer = new byte[dto.ItemData.Length];
                            int obj = S7dic[url].PLC.DBRead(dto.ItemData.DBNumber, dto.ItemData.StartAddr, dto.ItemData.Length, dbBuffer);
                            if (obj != 0)
                            {
                                return new Respostory(false, "Item不存在");
                            }
                            if (S7dic[url].ItemData.Contains(dto.ItemData))
                            {
                                return new Respostory(false, "Item已存在,不能重复添加");
                            }
                            S7dic[url].ItemData.Add(dto.ItemData);
                            if (IsReading)
                                IsReading = false;
                            ReadValue();
                            return new Respostory(true, dto.ItemData);
                        }
                        else
                        {
                            return new Respostory(false, "请先连接PLC,然后再添加tag地址");
                        }
                    }
                case UpdateMethod.Delete:
                    string url1 = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                    if (dto.ItemData.ItemName == null) return new Respostory(false, "Tag地址为空");
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
                            return new Respostory(true, dto.ItemData);
                        }
                        else
                        {
                            return new Respostory(false, "删除Item失败");
                        }
                    }
                default:
                    return new Respostory(false, "执行" + dto.Mehtod + "失败");
            }
        }

        /// <summary>
        /// 循环读取Value
        /// </summary>
        private void ReadValue()
        {

            if (IsReading) return;

            IsReading = true;

            Action readFromTread = () =>
            {
                while (IsReading)
                {
                    if (S7dic.Count > 0)
                    {
                        foreach (var item in S7dic)
                        {
                            if (item.Value.PLC != null)
                            {
                                if (!item.Value.PLC.Connected)
                                    return;
                                if (item.Value.PLC.Connected)
                                {
                                    if (item.Value == null || item.Value.ItemData.Count > 0)
                                    {
                                        //foreach (var itemData in item.Value.ItemData)
                                        for (int i = 0; i < item.Value.ItemData.Count; i++)
                                        {
                                            if (item.Value.ItemData[i] != null)
                                            {

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
                                        //{


                                    }
                                    // }

                                }
                            }
                        }
                    }

                }
            };
            Task.Factory.StartNew(readFromTread);

        }



        /// <summary>
        /// UI读取values
        /// </summary>
        /// <returns></returns>
        public List<S7ItemModel> GetValues()
        {
            List<S7ItemModel> items = new List<S7ItemModel>();

            foreach (var item in S7dic)
            {
                if (item.Value.ItemData.Count > 0)
                {
                    items.AddRange(item.Value.ItemData);
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
                            item.Value.PLC.Disconnect();
                        S7dic.Remove(item.Key, out _);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public Respostory Disconnect(S7Dto dto)
        {
            try
            {
                string url = dto.HostIP + dto.Rock.ToString() + dto.Slot.ToString();
                if (string.IsNullOrWhiteSpace(url)) return new Respostory(false, "服务器地址是空");
                if (S7dic.ContainsKey(url))
                {
                    if (S7dic[url].PLC != null)
                        S7dic[url].PLC.Disconnect();
                    S7dic.Remove(url, out _);
                    return new Respostory(true, dto);
                }
                return new Respostory(false, "需要断开的服务器尚未连接");
            }
            catch (Exception ex)
            {

                return new Respostory(false, "断开失败,错误信息:" + ex.Message);
            }


        }


    }
}
