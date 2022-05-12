using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Industry.Extenal
{
    /// <summary>
    /// 返回信息类
    /// </summary>
    public class Respostory
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public object Obj { get; set; }

        public Respostory(bool status,Object obj)
        {
            this.Status = status;
            this.Obj = obj;
        }

        public Respostory(bool status, string Message)
        {
            this.Status = status;
            this.Message = Message;
        }

    }



}
