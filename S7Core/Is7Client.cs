using Common;
using S7Core.Models;
using System.Collections.Generic;


namespace S7Core
{
    public interface Is7Client
    {
        Responsitory ConnectServer(S7Dto dto);

        Responsitory UpDateItemList(S7Dto dto);


        List<S7ItemModel> GetValues();

        void Disconnect();

        Responsitory Disconnect(S7Dto dto);

    }
}
