//using OPCAutomation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Industry.Extenal;

namespace TraceSystem.CommCore.OPCDa
{
    public interface IOPCClient
    {

        Respostory GetOPCServerList(string DA_Version, string host);

        Respostory ConnectServer(string ProgID, string host, int updateRate);

        Respostory AddItem(string itemName, string url);

        Respostory SubItem(string itemName, string url);

        void UpdateAsync(string ProgID, string host, bool start);

        Respostory Disconnect(string ProgID, string host);
    }

        
}
