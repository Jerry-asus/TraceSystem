using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TraceSystem.ViewModels;

namespace TraceSystem.Views
{
    /// <summary>
    /// Mainview.xaml 的交互逻辑
    /// </summary>
    public partial class MainView2 : Window
    {
        public MainView2()
        {
            //this.DataContext = new MainViewModel();
            InitializeComponent();

            this.MainBorder.MouseDown += (e, s) =>
              {
                  if (s.MouseDevice.LeftButton == MouseButtonState.Pressed)
                  {
                      this.DragMove();
                  }
              };


        }
    }
}
