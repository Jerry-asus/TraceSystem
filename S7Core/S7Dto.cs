using Common;
using S7Core.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace S7Core
{
    public class S7Dto
    {
        public string HostIP { get; set; }

        public int Rock { get; set; }

        public int Slot { get; set; }

        public S7ItemModel ItemData { get; set; }

        public Method Mehtod { get; set; }

    }

   

   


}
