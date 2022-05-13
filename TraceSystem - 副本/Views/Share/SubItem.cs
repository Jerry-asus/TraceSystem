using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace TraceSystem.Views.Share
{
    public class SubItem
    {
        public SubItem(string name, PackIconKind icon,UserControl screen = null)
        {
            Name = name;
            Screen = screen;
            Icon= icon;
        }
        public string Name { get; private set; }

        public PackIconKind Icon { get; private set; }
        public UserControl Screen { get; private set; }
    }
}
