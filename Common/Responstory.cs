using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Responsitory
    {
        public bool Status { get; set; }

        public string? Message { get; set; }

        public object? Obj { get; set; }

        public List<ItemModel>? Items { get; set; }

        /// <summary>
        /// Err+Err.Mesg
        /// </summary>
        /// <param name="status"></param>
        /// <param name="Message"></param>
        public Responsitory(bool status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        /// <summary>
        /// OK,Return Exceute Object
        /// </summary>
        /// <param name="status"></param>
        /// <param name="obj"></param>
        public Responsitory(bool status, object obj)
        {
            this.Status = status;
            this.Obj = obj;
        }


        public Responsitory(bool status, List<ItemModel> items)
        {
            this.Items = new List<ItemModel>();
            this.Status = status;
            this.Items = items;
        }
    }
}
