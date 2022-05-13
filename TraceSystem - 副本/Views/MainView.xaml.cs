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

namespace TraceSystem.Views
{
    /// <summary>
    /// MainView1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.MinBtn.Click += (e, s) => { this.WindowState = WindowState.Minimized; };
            this.CloseBtn.Click += (e, s) => { this.Close(); };
            this.MaxBtn.Click += (e, s) => 
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };

            this.MainViewPannel.MouseMove += (e, s) =>
              {
                  if (s.LeftButton == MouseButtonState.Pressed)
                  {
                      this.DragMove();
                  }
              };
            this.MainViewPannel.MouseDoubleClick += (e, s) =>
              {
                  if (this.WindowState == WindowState.Maximized)
                      this.WindowState = WindowState.Normal;
                  else
                      this.WindowState = WindowState.Maximized;
              };
        }
    }
}
