using System;
using System.Collections.Generic;
using System.Text;
using TraceSystem.Extension;

namespace TraceSystem.CommCore.S7Core
{
    public interface Is7Client
    {
        Respostory ConnectServer(S7Dto dto);

        Respostory UpDateItemList(S7Dto dto);


        List<S7ItemModel> GetValues();

        void Disconnect();

        Respostory Disconnect(S7Dto dto);

    }
}
