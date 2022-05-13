using System;
using System.Collections.Generic;
using System.Text;

namespace TraceSystem.CommCore.S7Core
{
    public class S7ItemModel
    {
        public string ItemName { get; set; }

        public S7DataType Datatype { get; set; }

        public int DBNumber { get; set; }

        public int StartAddr { get; set; }

        public int Length { get; set; }

        public object Value { get; set; }

    }


    public enum S7DataType
    {
        Int,
        DInt,
        Word,
        DWord,
        Real

    }
}
