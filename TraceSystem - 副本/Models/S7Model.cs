using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using TraceSystem.CommCore.S7Core;

namespace TraceSystem.Models
{
    public class S7Model:BindableBase
    {
        public S7Model()
        {
            Rock = new List<string>();
            Slot = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                Rock.Add(i.ToString());
                Slot.Add(i.ToString());
            }
            ValidHostCollect = new ObservableCollection<string>();
        }
        private string hostIP;

        public string HostIP
        {
            get { return hostIP; }
            set { hostIP = value; RaisePropertyChanged(); }
        }


        private List<string> rock;

        public List<string> Rock
        {           
            get { return rock; }
            set { rock = value; RaisePropertyChanged(); }
        }

        private string selectRock;

        public string  SelectRock
        {
            get { return selectRock; }
            set { selectRock = value;RaisePropertyChanged(); }
        }


        private List<string> slot;

        public List<string> Slot
        {
            get { return slot; }
            set { slot = value;RaisePropertyChanged(); }
        }

        private string selectSlot;

        public string SeletcSlot
        {
            get { return selectSlot; }
            set { selectSlot = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<string> validHostCollect;

        public ObservableCollection<string> ValidHostCollect
        {
            get { return validHostCollect; }
            set { validHostCollect = value; RaisePropertyChanged(); }
        }

        private string selectHostName;

        public string SelectHostName
        {
            get { return selectHostName; }
            set { selectHostName = value; RaisePropertyChanged(); }
        }






        private string dBNum;

        public string DBNum
        {
            get { return dBNum; }
            set { dBNum = value; RaisePropertyChanged(); }
        }

        private string startAddress;

        public string StartAddress
        {
            get { return startAddress; }
            set { startAddress = value; RaisePropertyChanged(); }
        }
        private string length;

        public string Length
        {
            get { return length; }
            set { length = value; RaisePropertyChanged(); }
        }


        private List<string> dataType;

        public List<string> DataType
        {
            get
            {
                List<string> templist = new List<string>();
                foreach (var item in Enum.GetValues(typeof(S7DataType)))
                {
                    templist.Add(item.ToString());
                }
                return templist;
            }
        }


        private string selectDatatype;

        public string SelectDatatype
        {
            get { return selectDatatype; }
            set { selectDatatype = value; RaisePropertyChanged(); }
        }




    }



}
