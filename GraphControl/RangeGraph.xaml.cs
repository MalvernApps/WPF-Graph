using ApplicationSDK;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace GraphControl
{
    /// <summary>
    /// Interaction logic for RangeGraph.xaml
    /// </summary>
    public partial class RangeGraph : UserControl
    {
        double[] todraw;
        public string title {get; set; }
        Pen greenPen;
        Pen grey;
        Pen RedPen;

        public Axis AxisX;
        public Axis AxisY;


        public RangeGraph()
        {
            InitializeComponent();

            CreatePens();

            todraw = new double[300];
            Random r = new Random(100);

            for (int x = 0; x < 300; x++)
            {
                todraw[x] = 100.0*r.NextDouble();
                    }

            this.Loaded += RangeGraph_Loaded;

            title = "Title Not Set";

           
        }

        private void CreatePens()
        {
            RedPen = new Pen(Brushes.Red, 1.0);
            RedPen.Freeze();

            greenPen = new Pen(Brushes.LightGreen, 1.0);
            greenPen.Freeze();

            grey = new Pen(Brushes.DarkGray, 1.0);
            grey.DashStyle = DashStyles.Dash;
            grey.Freeze();
        }

        private void RangeGraph_Loaded(object sender, RoutedEventArgs e)
        {
            // seytup the scales now the window has been sized
            //AxisX = new Axis(0.8 * this.ActualWidth, 0, 100 - 1, 0.1 * this.ActualWidth, false);
            //AxisY = new Axis(0.8 * this.ActualHeight, 0, 100, 0.1 * this.ActualHeight, false);

            AxisX = new Axis(1.0 * this.ActualWidth, 0, 100 - 1, 0, false);
            AxisY = new Axis(1.0 * this.ActualHeight, 0, 100, 0, false);

            // force a drawing
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // error check function order
            if (AxisX == null) return;

            // background
            drawingContext.DrawRectangle(Brushes.Black, RedPen, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            // need to draw some axis.
            DrawAxis(drawingContext, AxisX, AxisY);

            // draw the line
            for (int i=0;i<99;i++)
            {
                drawingContext.DrawLine(greenPen, 
                    new Point(AxisX.ConvertUnitsToPixels(i), AxisY.ConvertUnitsToPixels( todraw[i] )) , 
                    new Point(AxisX.ConvertUnitsToPixels(i+1), AxisY.ConvertUnitsToPixels(todraw[i+1]) ));
            }

            //Console.WriteLine("OnRender finished");
        }

        [Obsolete]
        private void DrawAxis(DrawingContext drawingContex, Axis AxisX, Axis AxisY)
        {
            // Draw a formatted text string into the DrawingContext.
            FormattedText ft = new FormattedText(title,
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,                  
                  new Typeface("Arial"),
                  12, System.Windows.Media.Brushes.White);  //));


            drawingContex.DrawText( ft, new System.Windows.Point(this.ActualWidth / 2 - ft.Width/2, this.ActualWidth * 0.01));

            for (double X=0;X<=100;X+=10)
            {
                drawingContex.DrawLine(grey,
                    new Point(AxisX.ConvertUnitsToPixels(0), AxisY.ConvertUnitsToPixels(X)),
                    new Point(AxisX.ConvertUnitsToPixels(100), AxisY.ConvertUnitsToPixels(X)));

                FormattedText fty = new FormattedText(X.ToString("F0"),
                      CultureInfo.GetCultureInfo("en-us"),
                      FlowDirection.LeftToRight,
                      new Typeface("Arial"),
                      10, System.Windows.Media.Brushes.LightGray);  //));

                drawingContex.DrawText(fty, new Point(10, AxisY.ConvertUnitsToPixels(X)-ft.Height/2)
                    );


                drawingContex.DrawLine(grey,
                  new Point(AxisX.ConvertUnitsToPixels(X), AxisY.ConvertUnitsToPixels(0)),
                  new Point(AxisX.ConvertUnitsToPixels(X), AxisY.ConvertUnitsToPixels(100)));

                FormattedText ftx = new FormattedText(X.ToString("F0"),
                     CultureInfo.GetCultureInfo("en-us"),
                     FlowDirection.LeftToRight,
                     new Typeface("Arial"),
                     10, System.Windows.Media.Brushes.LightGray);  //));

                drawingContex.DrawText(ftx, new Point(AxisX.ConvertUnitsToPixels(X) -10, this.ActualHeight - 20 )
                    );

                ;

               // Console.WriteLine( "Y val:" + x.GetScaledValue(100));

            }
        }

        public void SetData(double[] data)
        {
            todraw = data;
            InvalidateVisual();
        }
    }
}
