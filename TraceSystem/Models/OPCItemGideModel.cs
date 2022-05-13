using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace TraceSystem.Models
{
    public class OPCItemGideModel:BindableBase
    {
        private string tagName;

        public string TagName
        {
            get { return tagName; }
            set { tagName = value; RaisePropertyChanged(); }
        }


        private string value;

        public string Value
        {
            get { return value; }
            set { this.value = value; RaisePropertyChanged(); }
        }

        private string quality;

        public string Quality
        {
            get { return quality; }
            set { this.quality = value; RaisePropertyChanged(); }
        }

        private string dataType;

        public string DataType
        {
            get { return dataType; }
            set { this.dataType = value; RaisePropertyChanged(); }
        }

        private string timeStamp;

        public string TimeStamp
        {
            get { return timeStamp; }
            set { this.timeStamp = value; RaisePropertyChanged(); }
        }

    }
}
