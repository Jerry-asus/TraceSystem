using Common;
using Opc.Da;
using System.Collections.Generic;

namespace OPCDaMutithreadCore.OPCDa
{
    public interface IOPCClient
    {
        /// <summary>
        /// 返回目标主机服务器列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Responsitory GetOPCServerList(OPCDto dto);
        /// <summary>
        /// 根据选择的主机连接选择的服务器
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Responsitory ConnectServer(OPCDto dto);
        /// <summary>
        /// 对已经连接服务器注册Item或者删除Item
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Responsitory UpdateItem(OPCDto dto);

        /// <summary>
        /// 断开连接指定服务器连接
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Responsitory Disconnect(OPCDto dto);
        /// <summary>
        /// 关闭系统,全部服务器断开连接
        /// </summary>
        /// <returns></returns>
        Responsitory Disconnect();
        /// <summary>
        /// 获取所有更新值,
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Responsitory GetValues();
    }

        
}
