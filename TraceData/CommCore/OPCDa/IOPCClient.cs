//using OPCAutomation;
using Opc.Da;
using System.Collections.Generic;
using System.Threading.Tasks;
using TraceData.Extension;

namespace TraceData.CommCore.OPCDa
{
    public interface IOPCClient
    {
        /// <summary>
        /// 返回服务器列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        Respostory GetOPCServerList(OPCDto dto);

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Respostory ConnectServer(OPCDto dto);

        /// <summary>
        /// 注册Item
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Respostory UpdateItem(OPCDto dto);

        //Respostory SubItem(OPCDto dto);

       // void UpdateAsync(OPCDto dto);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Respostory Disconnect(OPCDto dto);

        /// <summary>
        /// 关闭系统
        /// </summary>
        /// <returns></returns>
        Respostory Disconnect();

        //object GetValue(OPCDto dto);

        /// <summary>
        /// 获取所有更新值
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        List<ItemValue> GetValues();
    }

        
}
