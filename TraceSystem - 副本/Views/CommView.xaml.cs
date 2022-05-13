using SciChart.Charting.Model.DataSeries;
using SciChart.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TraceSystem.ViewModels;

namespace TraceSystem.Views
{
    /// <summary>
    /// CommView.xaml 的交互逻辑
    /// </summary>
    public partial class CommView : UserControl
    {

        // Data Sample Rate (sec) - 20 Hz
        private readonly double dt = 0.02;

        // FIFO Size is 200 samples, meaning after 200 samples have been appended, each new sample appended
        // results in one sample being discarded
        private readonly int FifoSize = 50000;

        // Timer to process updates
        private readonly Timer _timerNewDataUpdate;

        // The current time
        private double t;

        readonly Random _random = new Random();

        // The dataseries to fill
        private IXyDataSeries<double, double> series0;
        private IXyDataSeries<double, double> series1;
        private IXyDataSeries<double, double> series2;



        private TimedMethod _startDelegate;

        public CommView()
        {
            InitializeComponent();
            var DataPrivde = new CommViewModel();
            _timerNewDataUpdate = new Timer(dt * 1000) { AutoReset = true };
            _timerNewDataUpdate.Elapsed += OnNewData;

            CreateDataSetAndSeries();




        }

        private void CreateDataSetAndSeries()
        {
            // Create new Dataseries of type X=double, Y=double
            series0 = new XyDataSeries<double, double>();
            series1 = new XyDataSeries<double, double>();
            series2 = new XyDataSeries<double, double>();

            if (true)
            {
                // Add three FIFO series to fill with data.                 
                // setting the FIFO capacity will denote this series as a FIFO series. New data is appended until the size is met, at which point
                //  old data is discarded. Internally the FIFO series is implemented as a circular buffer so that old data is pushed out of the buffer
                //  once the capacity has been reached
                // Note: Once a FIFO series has been added to a dataset, all subsequent series must be FIFO series. In addition, the FifoSize must be the
                //  same for all FIFO series in a dataset. 
                series0.FifoCapacity = FifoSize;
                series1.FifoCapacity = FifoSize;
                series2.FifoCapacity = FifoSize;
            }

            // Set the dataseries on the chart's RenderableSeries
            //RenderableSeries0.DataSeries = series0;
            //RenderableSeries1.DataSeries = series1;
            //RenderableSeries2.DataSeries = series2;
        }

        private void ClearDataSeries()
        {
            using (sciChart.SuspendUpdates())
            {
                series0?.Clear();
                series1?.Clear();
                series2?.Clear();
            }
        }


        private void OnNewData(object sender, EventArgs e)
        {
            // Compute our three series values
            double y1 = 3.0 * Math.Sin(2 * Math.PI * 1.4 * t) + _random.NextDouble() * 0.5;
            double y2 = 2.0 * Math.Cos(2 * Math.PI * 0.8 * t) + _random.NextDouble() * 0.5;
            double y3 = 1.0 * Math.Sin(2 * Math.PI * 2.2 * t) + _random.NextDouble() * 0.5;

            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (sciChart.SuspendUpdates())
            {
                // Append x,y data to previously created series
                series0.Append(t, y1);
                series1.Append(t, y2);
                series2.Append(t, y3);
            }

            // Increment current time
            t += dt;
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsChecked = true;
            PauseButton.IsChecked = false;
            ResetButton.IsChecked = false;

            //IsFifoCheckBox.IsEnabled = false;

            // Start a timer to create new data and append on each tick
            _timerNewDataUpdate.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _timerNewDataUpdate?.Stop();

            StartButton.IsChecked = false;
            PauseButton.IsChecked = true;
            ResetButton.IsChecked = false;

            //IsFifoCheckBox.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton_Click(this, null);

            StartButton.IsChecked = false;
            PauseButton.IsChecked = false;
            ResetButton.IsChecked = true;

            //IsFifoCheckBox.IsEnabled = true;

            t = -5.0;

            ClearDataSeries();
        }
    }
        
}
