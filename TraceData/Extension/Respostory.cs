using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Da;
using TraceData.CommCore.OPCDa;

namespace TraceData.Extension
{
    /// <summary>
    /// 返回信息类
    /// </summary>
    public class Respostory
    {
       
        public bool Status { get; set; }

        public string Message { get; set; }

        public object Obj { get; set; }

        /// <summary>
        /// /服务器列表
        /// </summary>
        public List<string> ServerList { get; set; }

        /// <summary>
        /// 更新数据返回新数据列表
        /// </summary>
        public List<ItemModel> ValueList { get; set; }

        /// <summary>
        /// Tag列表
        /// </summary>
        public List<string> ItemList { get; set; }

        public Respostory(bool status,Object obj)
        {
            this.Status = status;
            this.Obj = obj;
        }

        public Respostory()
        {
            ServerList = new List<string>();
            ItemList = new List<string>();
            ValueList = new List<ItemModel>();
        }

        /// <summary>
        /// 失败 +信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="Message"></param>
        public Respostory(bool status, string Message)
        {
            this.Status = status;
            this.Message = Message;
        }

        /// <summary>
        /// 连接成功,返回连接对象和对象的Tag表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="obj"></param>
        /// <param name="itemList"></param>
        public Respostory(bool status, Server obj, List<string> itemList)
        {
            this.Status = status;
            this.Obj = obj;
            this.ItemList = itemList;
        }

        public Respostory(bool status, string ItemName, List<string> itemList)
        {
            this.Status = status;
            this.Message = ItemName;
            this.ItemList = itemList;
        }

        /// <summary>
        /// 获取服务器成功,返回服务器列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="valueList"></param>
        public Respostory(bool status, List<string> serverList)
        {
            this.Status = status;
            this.ServerList = serverList;
        }
        /// <summary>
        /// 返货获取更新数据列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="valueList"></param>
        public Respostory(bool status, List<ItemModel> valueList)
        {
            this.Status = status;
            this.ValueList = valueList;
        }



    }



}
