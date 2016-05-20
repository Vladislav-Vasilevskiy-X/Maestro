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
using System.Windows.Ink;
using System.Runtime.InteropServices;
using System.IO;

namespace Maestro.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private enum Tools { Pencil, Eraser, Magnifier, ColorPicker, Fill, Text, Line, Rectangle, Ellipse }

        private Tools Tool { get; set; }
        private Point CurrentPoint { get; set; }
        private Point PreviousPoint { get; set; }
        private Point StartPoint { get; set; }
        private Color Color { get; set; }
        private double Thickness { get; set; }
        private Rectangle rectangle;
        private Ellipse ellipse;
        private Line line;
        double X2, Y2, X1, Y1;
        private Polyline eraser;
        private Polyline pencil;
        private const double ScaleRate = 1.1;
        private Polygon fillPolygon;
        private bool isDrawing = false;

        #region Tools buttons listeners

        private void pencil_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Pencil;
        }

        private void eraser_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Eraser;
        }

        private void color_picker_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.ColorPicker;
        }

        private void magnifier_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Magnifier;
        }

        private void fill_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Fill;
        }

        private void text_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Text;
        }

        private void line_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Line;
        }

        private void ellipse_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Ellipse;
        }

        private void rectangle_button_Click(object sender, RoutedEventArgs e)
        {
            this.Tool = Tools.Rectangle;
        }

        #endregion

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            rectangle = null;
            ellipse = null;
            line = null;
            eraser = null;
            pencil = null;

            //isDrawing = false;

            //if (fillPolygon != null)
            //{
            //    fillPolygon.Points.Add(fillPolygon.Points.First());
            //    fillPolygon.Fill = Brushes.Yellow;
            //}
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                //if (!isDrawing)
                //{
                //    isDrawing = true;

                //    fillPolygon = new Polygon()
                //    {
                //        Stroke = Brushes.Black,
                //        StrokeThickness = 1,
                //        StrokeMiterLimit = 1,
                //        StrokeLineJoin = PenLineJoin.Round,
                //        StrokeStartLineCap = PenLineCap.Round,
                //        StrokeEndLineCap = PenLineCap.Round
                //    };

                //    AddPoint(e.GetPosition(canvas));
                //    canvas.Children.Add(fillPolygon);
                //}

                switch (this.Tool)
                {
                    case Tools.Pencil:
                        {
                            Mouse.OverrideCursor = Cursors.Pen;
                            this.StartPoint = e.GetPosition(canvas);
                            this.pencil = new Polyline();
                            this.pencil.Stroke = new SolidColorBrush(this.Color);
                            this.pencil.StrokeThickness = this.Thickness;
                            canvas.Children.Add(this.pencil);
                        }
                        break;
                    case Tools.Eraser:
                        {
                            Mouse.OverrideCursor = Cursors.Hand;
                            this.StartPoint = e.GetPosition(canvas);
                            this.eraser = new Polyline();
                            this.eraser.Stroke = new SolidColorBrush(Colors.White);
                            this.eraser.StrokeThickness = 10;
                            canvas.Children.Add(this.eraser);
                        }
                        break;
                    case Tools.ColorPicker:
                        {
                            //canvas.Children.;
                            var point = e.GetPosition(canvas);
                            ColorPicker.SelectedColor = this.GetPixelColor(point);
                        }
                        break;
                    case Tools.Line:
                        {
                            Mouse.OverrideCursor = Cursors.Cross;
                            line = new Line();
                            line.X1 = e.GetPosition(canvas).X;
                            line.Y1 = e.GetPosition(canvas).Y;
                            line.X2 = line.X1 + 1;
                            line.Y2 = line.Y1 + 1;
                            line.StrokeThickness = this.Thickness;
                            line.Stroke = new SolidColorBrush(this.Color);
                            canvas.Children.Add(line);
                        }
                        break;

                    case Tools.Rectangle:
                        {
                            Mouse.OverrideCursor = Cursors.Cross;
                            rectangle = new Rectangle();
                            X1 = e.GetPosition(canvas).X;
                            Y1 = e.GetPosition(canvas).Y;
                            X2 = X1;
                            Y2 = Y1;
                            rectangle.Width = X2 - X1;
                            rectangle.Height = Y2 - Y1;
                            rectangle.StrokeThickness = this.Thickness;
                            rectangle.Stroke = new SolidColorBrush(this.Color);
                            //rectangle.Fill = new SolidColorBrush(FillColor);
                            canvas.Children.Add(rectangle);
                        }
                        break;
                    case Tools.Ellipse:
                        {
                            Mouse.OverrideCursor = Cursors.Cross;
                            ellipse = new Ellipse();
                            X1 = e.GetPosition(canvas).X;
                            Y1 = e.GetPosition(canvas).Y;
                            X2 = X1;
                            Y2 = Y1;
                            ellipse.Width = X2 - X1;
                            ellipse.Height = Y2 - Y1;
                            ellipse.StrokeThickness = this.Thickness;
                            ellipse.Stroke = new SolidColorBrush(this.Color);
                            //rectangle.Fill = new SolidColorBrush(FillColor);
                            canvas.Children.Add(ellipse);
                        }
                        break;
                }
                CurrentPoint = e.GetPosition(canvas);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isDrawing)
            //{
            //    AddPoint(e.GetPosition(canvas));
            //}

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (this.Tool)
                {
                    case Tools.Pencil:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                var point = e.GetPosition(canvas);
                                if (this.StartPoint != point && point != null && pencil != null)
                                {
                                    this.pencil.Points.Add(point);
                                }
                            }
                        }
                        break;
                    case Tools.Line:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                line.X2 = e.GetPosition(canvas).X;
                                line.Y2 = e.GetPosition(canvas).Y;
                            }
                        }
                        break;

                    case Tools.Rectangle:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                X2 = e.GetPosition(canvas).X;
                                Y2 = e.GetPosition(canvas).Y;
                                if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                    rectangle.Margin = new Thickness(X1, Y1, X2, Y2);
                                else if ((X2 - X1) < 0)
                                    rectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                                else if ((Y2 - Y1) < 0)
                                    rectangle.Margin = new Thickness(X1, Y2, X2, Y1);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                    rectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                                rectangle.Width = Math.Abs(X2 - X1);
                                rectangle.Height = Math.Abs(Y2 - Y1);
                            }
                        }
                        break;

                    case Tools.Ellipse:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                X2 = e.GetPosition(canvas).X;
                                Y2 = e.GetPosition(canvas).Y;
                                if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                    ellipse.Margin = new Thickness(X1, Y1, X2, Y2);
                                else if ((X2 - X1) < 0)
                                    ellipse.Margin = new Thickness(X2, Y1, X1, Y2);
                                else if ((Y2 - Y1) < 0)
                                    ellipse.Margin = new Thickness(X1, Y2, X2, Y1);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                    ellipse.Margin = new Thickness(X2, Y1, X1, Y2);
                                ellipse.Width = Math.Abs(X2 - X1);
                                ellipse.Height = Math.Abs(Y2 - Y1);
                            }
                        }
                        break;

                    case Tools.Eraser:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                var point = e.GetPosition(canvas);
                                if (this.StartPoint != point && point != null && this.eraser != null)
                                {
                                    this.eraser.Points.Add(point);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void canvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                canvas_scale.ScaleX *= ScaleRate;
                canvas_scale.ScaleY *= ScaleRate;
            }
            else
            {
                canvas_scale.ScaleX /= ScaleRate;
                canvas_scale.ScaleY /= ScaleRate;
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.Color = ColorPicker.SelectedColor.GetValueOrDefault();
        }

        private void ThicknessChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItemText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            try
            {
                this.Thickness = Double.Parse(selectedItemText.Substring(0, 1));
            }
            catch (Exception)
            {
                this.Thickness = 1;
            }
        }

        private double GetDistance(double X1, double Y1, double X2, double Y2)
        {
            double a = (double)(X2 - X1);
            double b = (double)(Y2 - Y1);

            return Math.Sqrt(a * a + b * b);
        }

        private void AddPoint(Point value)
        {
            if (value.X < (canvas.ActualWidth - 1)
            && value.Y < (canvas.ActualHeight - 1))
            {
                fillPolygon.Points.Add(value);
            }
        }

        public void MakeFloodFill(object sender, MouseEventArgs e)
        {
            // public void floodFill(BufferedImage image, Point node, Color targetColor, Color replacementColor) {
            System.Drawing.Bitmap image = this.GetBitmapFromCanvas();
            int width = image.Width;
            int height = image.Height;
            System.Drawing.Color replacementColor = this.GetPixelDrawingColor(e.GetPosition(canvas));
            Point node = e.GetPosition(canvas);
            System.Drawing.Color targetColor = System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B);
            int target = targetColor.ToArgb();
            if (targetColor != replacementColor)
            {
                Queue<Point> queue = new Queue<Point>();
                bool noMorePixelsLeft = false;
                do
                {
                    int x = (int)node.X;
                    int y = (int)node.Y;
                    while (x > 0 && image.GetPixel(x - 1, y).ToArgb() == target)
                    {
                        x--;
                    }
                    bool spanUp = false;
                    bool spanDown = false;
                    while (x < width && image.GetPixel(x, y).ToArgb() == target)
                    {
                        image.SetPixel(x, y, replacementColor);
                        if (!spanUp && y > 0 && image.GetPixel(x, y - 1).ToArgb() == target)
                        {
                            queue.Enqueue(new Point(x, y - 1));
                            spanUp = true;
                        }
                        else if (spanUp && y > 0 && image.GetPixel(x, y - 1).ToArgb() != target)
                        {
                            spanUp = false;
                        }
                        if (!spanDown && y < height - 1 && image.GetPixel(x, y + 1).ToArgb() == target)
                        {
                            queue.Enqueue(new Point(x, y + 1));
                            spanDown = true;
                        }
                        else if (spanDown && y < height - 1 && image.GetPixel(x, y + 1).ToArgb() != target)
                        {
                            spanDown = false;
                        }
                        x++;
                    }
                    noMorePixelsLeft = false;
                    if (queue.Count > 0)
                    {
                        node = queue.Dequeue();
                        noMorePixelsLeft = true;
                    }
                    else noMorePixelsLeft = false;
                } while (noMorePixelsLeft);
                canvas.UpdateLayout();
            }
        }

        public System.Windows.Media.Color GetPixelColor(Point point)
        {
            var color = this.GetBitmapFromCanvas().GetPixel((int)point.X, (int)point.Y);
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public System.Drawing.Color GetPixelDrawingColor(Point point)
        {
            return this.GetBitmapFromCanvas().GetPixel((int)point.X, (int)point.Y);
        }

        public System.Drawing.Bitmap GetBitmapFromCanvas()
        {
            Transform transform = canvas.LayoutTransform;
            canvas.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(canvas.Width, canvas.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            System.Drawing.Bitmap result;

            // Create a file stream for saving image
            using (MemoryStream outStream = new MemoryStream())
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);

                result = new System.Drawing.Bitmap(outStream);
            }

            // Restore previously saved layout
            canvas.LayoutTransform = transform;

            return result;
        }


        //public Color GetPixelColor(Visual visual, Point pt)
        //{
        //    Point ptDpi = getScreenDPI(visual);

        //    Size srcSize = VisualTreeHelper.GetDescendantBounds(visual).Size;

        //    //Viewbox uses values between 0 & 1 so normalize the Rect with respect to the visual's Height & Width
        //    Rect percentSrcRec = new Rect(pt.X / srcSize.Width, pt.Y / srcSize.Height,
        //                                  1 / srcSize.Width, 1 / srcSize.Height);

        //    //var bmpOut = new RenderTargetBitmap(1, 1, 96d, 96d, PixelFormats.Pbgra32); //assumes 96 dpi
        //    var bmpOut = new RenderTargetBitmap((int)(ptDpi.X / 96d),
        //                                        (int)(ptDpi.Y / 96d),
        //                                        ptDpi.X, ptDpi.Y, PixelFormats.Default); //generalized for monitors with different dpi

        //    DrawingVisual dv = new DrawingVisual();
        //    using (DrawingContext dc = dv.RenderOpen())
        //    {
        //        dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrcRec },
        //                         null, //no Pen
        //                         new Rect(0, 0, 1d, 1d));
        //    }
        //    bmpOut.Render(dv);

        //    var bytes = new byte[4];
        //    int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
        //    bmpOut.CopyPixels(bytes, iStride, 0);

        //    return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        //}

        //public static Point getScreenDPI(Visual v)
        //{
        //    //System.Windows.SystemParameters
        //    PresentationSource source = PresentationSource.FromVisual(v);
        //    Point ptDpi;
        //    if (source != null)
        //    {
        //        ptDpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11,
        //                           96.0 * source.CompositionTarget.TransformToDevice.M22);
        //    }
        //    else
        //        ptDpi = new Point(96d, 96d); //default value.

        //    return ptDpi;
        //}
    }
}
