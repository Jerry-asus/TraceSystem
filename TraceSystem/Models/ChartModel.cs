using System;
using System.Windows.Media;

namespace TraceSystem.Models
{
    public class ChartModel
    {
        //public AxisX CreatexAxis()
        //{
        //    AxisX xAxis = new AxisX();
        //    xAxis.ValueType = AxisValueType.DateTime;
        //    xAxis.AutoFormatLabels = false;
        //    xAxis.LabelsAngle = 90;
        //    xAxis.AutoDivSpacing = false;
        //    xAxis.MajorDiv = 0.001; //1 ms
        //    xAxis.KeepDivCountOnRangeChange = false;
        //    xAxis.LabelsTimeFormat = "dd-MM-yyyy HH:mm.ss.fff";
        //    xAxis.Title.Visible = false;
        //    return xAxis;
        //}

        //public AxisY CreateyAxis()
        //{
        //    AxisY yAxis = new AxisY();
        //    yAxis.Title.Text = "EUR/USD";
        //    return yAxis;
        //}

        //public PointLineSeries CreatePointlineSeries(AxisX xAxis)
        //{

        //    DateTime startTime = DateTime.Now;
        //    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, startTime.Second);

        //    Random random = new Random();
        //    double y = 1.3;
        //    int pointCount = 100;
        //    SeriesPoint[] points = new SeriesPoint[pointCount];

        //    for (int i = 0; i < pointCount; i++)
        //    {
        //        y += (random.NextDouble() - 0.5) * 0.01;
        //        if (y > 1.35)
        //            y = 1.35;
        //        if (y < 1.25)
        //            y = 1.25;

        //        points[i].X = xAxis.DateTimeToAxisValue(startTime + TimeSpan.FromMilliseconds(i));
        //        points[i].Y = y;
        //    }

        //    PointLineSeries pointLineSeries = new PointLineSeries();
        //    pointLineSeries.AllowUserInteraction = false;
        //    pointLineSeries.LineStyle.Width = 1;
        //    pointLineSeries.Points = points;
        //    return pointLineSeries;
        //}

        //public AxisX CreateSecondaryxAxis()
        //{
        //    AxisX secondaryXAxis = new AxisX();
        //    secondaryXAxis.AllowUserInteraction = false;
        //    secondaryXAxis.AutoFormatLabels = false;
        //    secondaryXAxis.CustomTicksEnabled = true;
        //    secondaryXAxis.AxisColor = Colors.Transparent;
        //    secondaryXAxis.MajorGrid.Color = Colors.Red;
        //    secondaryXAxis.MajorGrid.Pattern = LinePattern.SmallDot;
        //    secondaryXAxis.LabelsColor = Colors.Red;
        //    secondaryXAxis.Title.Visible = false;
        //    secondaryXAxis.MajorDivTickStyle.Color = Colors.Red;
        //    secondaryXAxis.LabelsFont = new WpfFont("Segoe UI", 13, true, false);
        //    secondaryXAxis.AllowScaling = false;
        //    return secondaryXAxis;
        //}

        //public AxisY CreateSecondaryYAxis()
        //{
        //    AxisY secondaryYAxis = new AxisY();
        //    secondaryYAxis.AllowUserInteraction = false;
        //    secondaryYAxis.AutoFormatLabels = false;
        //    secondaryYAxis.CustomTicksEnabled = true;
        //    secondaryYAxis.AxisColor = Colors.Transparent;
        //    secondaryYAxis.MajorGrid.Color = Colors.Red;
        //    secondaryYAxis.MajorGrid.Pattern = LinePattern.SmallDot;
        //    secondaryYAxis.LabelsColor = Colors.Red;
        //    secondaryYAxis.Title.Visible = false;
        //    secondaryYAxis.MajorDivTickStyle.Color = Colors.Red;
        //    secondaryYAxis.LabelsFont = new WpfFont("Segoe UI", 13, true, false);
        //    secondaryYAxis.AllowScaling = false;
        //    return secondaryYAxis;
        //}
    }
}
