//using OPCAutomation;
using Opc.Da;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Industry.Extenal;

namespace TraceSystem.CommCore.OPCDa
{
    public interface IOPCClient
    {

        Respostory GetOPCServerList(OPCDto dto);

        Respostory ConnectServer(OPCDto dto);

        Respostory AddItem(OPCDto dto);

        Respostory SubItem(OPCDto dto);

        void UpdateAsync(OPCDto dto);

        Respostory Disconnect(OPCDto dto);

        Respostory Disconnect();

        object GetValue(OPCDto dto);

        Respostory GetValues(OPCDto dto);
    }

        
}
