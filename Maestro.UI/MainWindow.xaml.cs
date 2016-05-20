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
using Microsoft.Win32;

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
        private bool boldText = false;
        private bool italicText = false;
        private bool underlinedText = false;
        private Stack<UIElement> lastAddedUiEls = new Stack<UIElement>();
        //private Stack<UIElementCollection> repeatChildrenInstances = new Stack<UIElementCollection>();

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
            rectangle = null;
            ellipse = null;
            line = null;
            eraser = null;
            pencil = null;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                switch (this.Tool)
                {
                    case Tools.Pencil:
                        {
                            this.StartPoint = e.GetPosition(canvas);
                            this.pencil = new Polyline();
                            this.pencil.Stroke = new SolidColorBrush(this.Color);
                            this.pencil.StrokeThickness = this.Thickness;
                            canvas.Children.Add(this.pencil);
                            lastAddedUiEls.Push(this.pencil);
                        }
                        break;
                    case Tools.Eraser:
                        {
                            this.StartPoint = e.GetPosition(canvas);
                            this.eraser = new Polyline();
                            this.eraser.Stroke = new SolidColorBrush(Colors.White);
                            this.eraser.StrokeThickness = 10;
                            canvas.Children.Add(this.eraser);
                            lastAddedUiEls.Push(this.eraser);
                        }
                        break;
                    case Tools.ColorPicker:
                        {
                            ColorPicker.SelectedColor = this.GetPixelColor(e.GetPosition(canvas));
                        }
                        break;
                    case Tools.Line:
                        {
                            line = new Line();
                            line.X1 = e.GetPosition(canvas).X;
                            line.Y1 = e.GetPosition(canvas).Y;
                            line.X2 = line.X1 + 1;
                            line.Y2 = line.Y1 + 1;
                            line.StrokeThickness = this.Thickness;
                            line.Stroke = new SolidColorBrush(this.Color);
                            canvas.Children.Add(line);
                            lastAddedUiEls.Push(this.line);
                        }
                        break;

                    case Tools.Rectangle:
                        {
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
                            lastAddedUiEls.Push(rectangle);
                        }
                        break;
                    case Tools.Ellipse:
                        {
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
                            lastAddedUiEls.Push(ellipse);
                        }
                        break;
                    case Tools.Fill:
                        {
                            this.MakeFloodFill(e.GetPosition(canvas));
                        }
                        break;
                    case Tools.Text:
                        {
                            Text(sender, e.GetPosition(canvas));
                        }
                        break;
                }
                CurrentPoint = e.GetPosition(canvas);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
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
                            if (e.LeftButton == MouseButtonState.Pressed && line != null)
                            {
                                line.X2 = e.GetPosition(canvas).X;
                                line.Y2 = e.GetPosition(canvas).Y;
                            }
                        }
                        break;

                    case Tools.Rectangle:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed && rectangle != null)
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
                            if (e.LeftButton == MouseButtonState.Pressed && ellipse != null)
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
                            if (e.LeftButton == MouseButtonState.Pressed && eraser != null)
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

        public void MakeFloodFill(Point canvasMousePoint)
        {
            System.Drawing.Bitmap image = this.GetBitmapFromCanvas();
            int width = image.Width;
            int height = image.Height;
            System.Drawing.Color replacementColor = System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B);
            Point node = canvasMousePoint;
            System.Drawing.Color targetColor = this.GetPixelDrawingColor(canvasMousePoint);
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
                Image img = new Image();
                img.Source = this.BitmapToImageSource(image);
                canvas.Children.Add(img);
                lastAddedUiEls.Push(img);
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
            Size size = new Size(canvas.Width, canvas.Height);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);
            System.Drawing.Bitmap result;
            using (MemoryStream outStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
                result = new System.Drawing.Bitmap(outStream);
            }
            canvas.LayoutTransform = transform;
            return result;
        }

        private void text_bold_button_Click(object sender, RoutedEventArgs e)
        {
            BooleanTrigger(ref boldText);
        }

        private void text_italic_button_Click(object sender, RoutedEventArgs e)
        {
            BooleanTrigger(ref italicText);
        }

        private void text_underline_button_Click(object sender, RoutedEventArgs e)
        {
            BooleanTrigger(ref underlinedText);
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Clear_Screen_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ClearCanvas();
        }

        private void Exit_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ClearCanvas();
            Application.Current.Shutdown();
        }

        private void Text(object sender, Point point)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = textBox.Text;
            textBox.FontFamily = new FontFamily((FontPicker.SelectedItem as ComboBoxItem).Content as string);
            try
            {
                textBlock.FontSize = Double.Parse((TextSizePicker.SelectedItem as ComboBoxItem).Content as string);
            }
            catch (Exception)
            {
                textBlock.FontSize = 8;
            }
            if (boldText)
            {
                textBlock.FontWeight = FontWeights.Bold;
            }
            if (italicText)
            {
                textBlock.FontStyle = FontStyles.Italic;
            }
            if (underlinedText)
            {
                textBlock.TextDecorations = TextDecorations.Underline;
            }
            textBlock.Foreground = new SolidColorBrush(this.Color);
            Canvas.SetLeft(textBlock, point.X);
            Canvas.SetTop(textBlock, point.Y);
            canvas.Children.Add(textBlock);
            lastAddedUiEls.Push(textBlock);
        }

        private void Save_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Untitled";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image (.jpg)|*.jpg";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    this.GetBitmapFromCanvas().Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void Open_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Image";
            dlg.Filter = "Image files (*.bmp)|*.bmp|(*.jpg)|*.jpg|(*.png)|*.png";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(dlg.FileName);
                this.ClearCanvas();
                canvas.Children.Add(new Image() { Source = this.BitmapToImageSource(image) });
            }
        }

        private void Undo_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lastAddedUiEls.Count > 0)
            {
                canvas.Children.Remove(lastAddedUiEls.Pop());
            }
        }

        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void BooleanTrigger(ref bool variable)
        {
            variable = !variable;
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            canvas.UpdateLayout();
        }
    }
}
