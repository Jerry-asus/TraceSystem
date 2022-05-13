using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Da;

namespace TraceSystem.CommCore.OPCDa
{
    [Serializable]
    public class ItemModel
    {
        public ItemValue Value { get; set; }

        public Opc.Da.Item Item { get; set; }

        public string DataType { get; set; }

        /// <summary>
	    /// 深度拷贝: 针对.net 5版本以前，新版本过时提示
    	/// </summary>
	    /// <returns></returns>
    	public ItemModel DeepClone()
        {
            using (System.IO.Stream os = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(os, this);
                os.Seek(0, System.IO.SeekOrigin.Begin);
                return formatter.Deserialize(os) as ItemModel;
            }
        }
    }
}
