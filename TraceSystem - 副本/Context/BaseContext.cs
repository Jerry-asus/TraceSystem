using System;
using System.Collections.Generic;
using System.Text;

namespace TraceSystem.Context
{
    public class BaseContext
    {
        public int ID { get; set; }

        public DateTime CrateTime { get; set; } = DateTime.Now;

    }
}
