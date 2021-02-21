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
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// Zooming from: https://zerobytellc.com/2006/11/24/drawing-a-rubberband-in-wpf/
/// </summary>

namespace GraphControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count = 0;

        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000);
            dispatcherTimer.Start();

            graph.title = "Animated Graph Example with Zooming";

            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown; ;
            canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            canvas.MouseMove += Canvas_MouseMove;

        }

        private Shape rubberBand = null;
        Point mouseLeftDownPoint;

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(canvas);

                if (rubberBand == null)
                {
                    rubberBand = new Rectangle();
                    rubberBand.Stroke = new SolidColorBrush(Colors.Red);
                    rubberBand.StrokeThickness = 2.0;
                    canvas.Children.Add(rubberBand);
                }

                double width = Math.Abs(mouseLeftDownPoint.X - currentPoint.X);
                double height = Math.Abs(mouseLeftDownPoint.Y - currentPoint.Y);
                double left = Math.Min(mouseLeftDownPoint.X, currentPoint.X);
                double top = Math.Min(mouseLeftDownPoint.Y, currentPoint.Y);

                rubberBand.Width = width;
                rubberBand.Height = height;
                Canvas.SetLeft(rubberBand, left);
                Canvas.SetTop(rubberBand, top);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canvas.IsMouseCaptured && rubberBand != null)
            {
                canvas.Children.Remove(rubberBand);
                rubberBand = null;

                canvas.ReleaseMouseCapture();

                // sort the X axis
                Point currentPoint = e.GetPosition(canvas);
                double one = graph.AxisX.ConvertPixelsToUnits(currentPoint.X);
                double two = graph.AxisX.ConvertPixelsToUnits(mouseLeftDownPoint.X);

                if (one < two)
                {

                    graph.AxisX.ZoomIn(one, two);
                    graph.InvalidateVisual();
                }
                else
                {
                    graph.AxisX.ZoomIn(two, one);
                    graph.InvalidateVisual();
                }

                // sort the Y axis

                double three = graph.AxisY.ConvertPixelsToUnits(currentPoint.Y);
                double four = graph.AxisY.ConvertPixelsToUnits(mouseLeftDownPoint.Y);

                if (three < four)
                {

                    graph.AxisY.ZoomIn(three, four);
                    graph.InvalidateVisual();
                }
                else
                {
                    graph.AxisY.ZoomIn(four, three);
                    graph.InvalidateVisual();
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canvas.IsMouseCaptured)
            {
                mouseLeftDownPoint = e.GetPosition(canvas);
                canvas.CaptureMouse();
            };
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            count++;
            Console.WriteLine("tick: " + count);

            double[] todraw = new double[300];
            Random r = new Random(Guid.NewGuid().GetHashCode());

            for (int x = 0; x < 300; x++)
            {
                todraw[x] = 100.0 * r.NextDouble();
            }

            graph.SetData(todraw);
        }

        
        /// <summary>
        /// the mouse has moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(this);

            // display our current mouise coord
            Console.WriteLine(  graph.AxisX.ConvertPixelsToUnits(point.X) + " " + graph.AxisY.ConvertPixelsToUnits(point.Y));

            coords.Content = graph.AxisX.ConvertPixelsToUnits(point.X).ToString("F1") + " " + graph.AxisY.ConvertPixelsToUnits(point.Y).ToString( "F1");
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            graph.AxisY.ZoomOut();
            graph.AxisX.ZoomOut();

            graph.InvalidateVisual();
        }
    }
}
