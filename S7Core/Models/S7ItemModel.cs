using System;
using System.Collections.Generic;
using System.Text;

namespace S7Core.Models
{
    public class S7ItemModel
    {
        public string ItemName { get; set; }

        public S7DataType Datatype { get; set; }

        public int DBNumber { get; set; }

        public int StartAddr { get; set; }

        public int Length { get; set; }

        /// <summary>
        /// 读取的数值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Item注册时标记是否有效
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// 注册时返回的状态信息 存在是显示正常,不存在时显示不存在
        /// </summary>
        public string ItemMessage { get; set; }

    }


    public class S7SimpleItemModel
    {
        public string ItemName { get; set; }

        public S7DataType Datatype { get; set; }

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
