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

namespace Maestro.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InkCanvas incCanvas = new InkCanvas();
        string DrawingTool;
        double X1, X2, Y1, Y2, StrokeThickness = 1;
        Line NewLine;
        Ellipse NewEllipse;
        Point StartPoint, PreviousContactPoint, CurrentContactPoint;
        Polyline Pencil;
        Rectangle NewRectangle;
        Color BorderColor = Colors.Black, FillColor;
        uint PenID, TouchID;

        public MainWindow()
        {
            InitializeComponent();
        }

		//void canvas_PointerExited(object sender, Event e)
		//{
		//    Mouse.OverrideCursor = Cursors.Arrow;
		//}

		//void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
		//{
		//    if (DrawingTool != "Eraser")
		//        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);
		//    else
		//        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);

		//    switch (DrawingTool)
		//    {
		//        case "Line":
		//            {
		//                NewLine = new Line();
		//                NewLine.X1 = e.GetCurrentPoint(canvas).Position.X;
		//                NewLine.Y1 = e.GetCurrentPoint(canvas).Position.Y;
		//                NewLine.X2 = NewLine.X1 + 1;
		//                NewLine.Y2 = NewLine.Y1 + 1;
		//                NewLine.StrokeThickness = StrokeThickness;
		//                NewLine.Stroke = new SolidColorBrush(BorderColor);
		//                canvas.Children.Add(NewLine);
		//            }
		//            break;

		//        case "Pencil":
		//            {
		//                /* Old Code
		//                StartPoint = e.GetCurrentPoint(canvas).Position;
		//                Pencil = new Polyline();
		//                Pencil.Stroke = new SolidColorBrush(BorderColor);
		//                Pencil.StrokeThickness = StrokeThickness;
		//                canvas.Children.Add(Pencil);
		//                */

		//                var MyDrawingAttributes = new InkDrawingAttributes();
		//                MyDrawingAttributes.Size = new Size(StrokeThickness, StrokeThickness);
		//                MyDrawingAttributes.Color = BorderColor;
		//                MyDrawingAttributes.FitToCurve = true;
		//                MyInkManager.SetDefaultDrawingAttributes(MyDrawingAttributes);

		//                PreviousContactPoint = e.GetCurrentPoint(canvas).Position;
		//                //PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;  to identify the pointer device
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
		//                {
		//                    // Pass the pointer information to the InkManager.
		//                    MyInkManager.ProcessPointerDown(e.GetCurrentPoint(canvas));
		//                    PenID = e.GetCurrentPoint(canvas).PointerId;
		//                    e.Handled = true;
		//                }
		//            }
		//            break;

		//        case "Rectagle":
		//            {
		//                NewRectangle = new Rectangle();
		//                X1 = e.GetCurrentPoint(canvas).Position.X;
		//                Y1 = e.GetCurrentPoint(canvas).Position.Y;
		//                X2 = X1;
		//                Y2 = Y1;
		//                NewRectangle.Width = X2 - X1;
		//                NewRectangle.Height = Y2 - Y1;
		//                NewRectangle.StrokeThickness = StrokeThickness;
		//                NewRectangle.Stroke = new SolidColorBrush(BorderColor);
		//                NewRectangle.Fill = new SolidColorBrush(FillColor);
		//                canvas.Children.Add(NewRectangle);
		//            }
		//            break;

		//        case "Ellipse":
		//            {
		//                NewEllipse = new Ellipse();
		//                X1 = e.GetCurrentPoint(canvas).Position.X;
		//                Y1 = e.GetCurrentPoint(canvas).Position.Y;
		//                X2 = X1;
		//                Y2 = Y1;
		//                NewEllipse.Width = X2 - X1;
		//                NewEllipse.Height = Y2 - Y1;
		//                NewEllipse.StrokeThickness = StrokeThickness;
		//                NewEllipse.Stroke = new SolidColorBrush(BorderColor);
		//                NewEllipse.Fill = new SolidColorBrush(FillColor);
		//                canvas.Children.Add(NewEllipse);
		//            }
		//            break;

		//        case "Eraser":
		//            {
		//                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);
		//                StartPoint = e.GetCurrentPoint(canvas).Position;
		//                Pencil = new Polyline();
		//                Pencil.Stroke = new SolidColorBrush(Colors.Wheat);
		//                Pencil.StrokeThickness = 10;
		//                canvas.Children.Add(Pencil);
		//            }
		//            break;

		//        default:
		//            break;
		//    }
		//}

		//void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
		//{
		//    if (DrawingTool != "Eraser")
		//        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);
		//    else
		//        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);

		//    switch (DrawingTool)
		//    {
		//        case "Pencil":
		//            {
		//                /* Old Code
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
		//                {
		//                    if (StartPoint != e.GetCurrentPoint(canvas).Position)
		//                    {
		//                        Pencil.Points.Add(e.GetCurrentPoint(canvas).Position);
		//                    }
		//                }
		//                */

		//                if (e.Pointer.PointerId == PenID || e.Pointer.PointerId == TouchID)
		//                {
		//                    // Distance() is an application-defined function that tests
		//                    // whether the pointer has moved far enough to justify 
		//                    // drawing a new line.
		//                    CurrentContactPoint = e.GetCurrentPoint(canvas).Position;
		//                    X1 = PreviousContactPoint.X;
		//                    Y1 = PreviousContactPoint.Y;
		//                    X2 = CurrentContactPoint.X;
		//                    Y2 = CurrentContactPoint.Y;

		//                    if (Distance(X1, Y1, X2, Y2) > 2.0)
		//                    {
		//                        Line line = new Line()
		//                        {
		//                            X1 = X1,
		//                            Y1 = Y1,
		//                            X2 = X2,
		//                            Y2 = Y2,
		//                            StrokeThickness = StrokeThickness,
		//                            Stroke = new SolidColorBrush(BorderColor)
		//                        };

		//                        PreviousContactPoint = CurrentContactPoint;
		//                        canvas.Children.Add(line);
		//                        MyInkManager.ProcessPointerUpdate(e.GetCurrentPoint(canvas));
		//                    }
		//                }
		//            }
		//            break;

		//        case "Line":
		//            {
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
		//                {
		//                    NewLine.X2 = e.GetCurrentPoint(canvas).Position.X;
		//                    NewLine.Y2 = e.GetCurrentPoint(canvas).Position.Y;
		//                }
		//            }
		//            break;

		//        case "Rectagle":
		//            {
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
		//                {
		//                    X2 = e.GetCurrentPoint(canvas).Position.X;
		//                    Y2 = e.GetCurrentPoint(canvas).Position.Y;
		//                    if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
		//                        NewRectangle.Margin = new Thickness(X1, Y1, X2, Y2);
		//                    else if ((X2 - X1) < 0)
		//                        NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
		//                    else if ((Y2 - Y1) < 0)
		//                        NewRectangle.Margin = new Thickness(X1, Y2, X2, Y1);
		//                    else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
		//                        NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
		//                    NewRectangle.Width = Math.Abs(X2 - X1);
		//                    NewRectangle.Height = Math.Abs(Y2 - Y1);
		//                }
		//            }
		//            break;

		//        case "Ellipse":
		//            {
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
		//                {
		//                    X2 = e.GetCurrentPoint(canvas).Position.X;
		//                    Y2 = e.GetCurrentPoint(canvas).Position.Y;
		//                    if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
		//                        NewEllipse.Margin = new Thickness(X1, Y1, X2, Y2);
		//                    else if ((X2 - X1) < 0)
		//                        NewEllipse.Margin = new Thickness(X2, Y1, X1, Y2);
		//                    else if ((Y2 - Y1) < 0)
		//                        NewEllipse.Margin = new Thickness(X1, Y2, X2, Y1);
		//                    else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
		//                        NewEllipse.Margin = new Thickness(X2, Y1, X1, Y2);
		//                    NewEllipse.Width = Math.Abs(X2 - X1);
		//                    NewEllipse.Height = Math.Abs(Y2 - Y1);
		//                }
		//            }
		//            break;

		//        case "Eraser":
		//            {
		//                if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
		//                {
		//                    if (StartPoint != e.GetCurrentPoint(canvas).Position)
		//                    {
		//                        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);
		//                        Pencil.Points.Add(e.GetCurrentPoint(canvas).Position);
		//                    }
		//                }
		//            }
		//            break;

		//        default:
		//            break;
		//    }
		//}
	}
}
