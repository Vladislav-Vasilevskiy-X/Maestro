using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private InfoPanel infoPanel = new InfoPanel();

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Utils.executor.CleanShapes();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                switch (Utils.Tool)
                {
                    case Utils.Tools.Pencil:
                        Utils.executor.DrawWithPencil(ref canvas, e.GetPosition(canvas));
                        break;

                    case Utils.Tools.Eraser:
                        Utils.executor.Erase(ref canvas, e.GetPosition(canvas));
                        break;

                    case Utils.Tools.ColorPicker:
                        ColorPicker.SelectedColor = Utils.GetPixelColor(e.GetPosition(canvas), ref canvas);
                        break;

                    case Utils.Tools.Line:
                        Utils.executor.DrawWithLine(ref canvas, e.GetPosition(canvas));
                        break;

                    case Utils.Tools.Rectangle:
                        Utils.executor.DrawWithRectangle(ref canvas, e.GetPosition(canvas));
                        break;

                    case Utils.Tools.Ellipse:
                        Utils.executor.DrawWithEllipse(ref canvas, e.GetPosition(canvas));
                        break;

                    case Utils.Tools.Fill:
                        Utils.executor.MakeFloodFill(ref canvas, e.GetPosition(canvas));
                        break;
                    case Utils.Tools.Text:
                        Utils.executor.DrawText(sender, e.GetPosition(canvas), ref canvas, ref textBox,
                            (FontPicker.SelectedItem as ComboBoxItem).Content as string,
                            (TextSizePicker.SelectedItem as ComboBoxItem).Content as string);
                        break;
                }
                Utils.executor.CurrentPoint = e.GetPosition(canvas);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (Utils.Tool)
                {
                    case Utils.Tools.Pencil:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                Utils.executor.HoldPressedPencilAndMove(e.GetPosition(canvas));
                            }
                        }
                        break;

                    case Utils.Tools.Line:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                Utils.executor.HoldPressedLineAndMove(e.GetPosition(canvas));
                            }
                        }
                        break;

                    case Utils.Tools.Rectangle:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                Utils.executor.HoldPressedRecatngleAndMove(e.GetPosition(canvas));
                            }
                        }
                        break;

                    case Utils.Tools.Ellipse:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                Utils.executor.HoldPressedEllipseAndMove(e.GetPosition(canvas));
                            }
                        }
                        break;

                    case Utils.Tools.Eraser:
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                Utils.executor.HoldPressedEraserAndMove(e.GetPosition(canvas));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            Info_panel.Position = (Utils.ComposePositionLabelContent(e.GetPosition(canvas)));
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Utils.executor.Color = ColorPicker.SelectedColor.GetValueOrDefault();
        }

        private void ThicknessChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItemText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            try
            {
                Utils.executor.Thickness = Double.Parse(selectedItemText.Substring(0, 1));
            }
            catch (Exception)
            {
                Utils.executor.Thickness = 1;
            }
        }

        private void text_bold_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.BooleanTrigger(ref Utils.executor.boldText);
        }

        private void text_italic_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.BooleanTrigger(ref Utils.executor.italicText);
        }

        private void text_underline_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.BooleanTrigger(ref Utils.executor.underlinedText);
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            canvas.Focus();
            Info_panel.Position = Utils.ComposePositionLabelContent(e.GetPosition(canvas));
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Info_panel.Position = "Позиция курсора: ";
        }

        private void Clear_Screen_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utils.executor.ClearCanvas(ref canvas, ref canvas_scale);
        }

        private void Exit_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utils.executor.ClearCanvas(ref canvas, ref canvas_scale);
            Application.Current.Shutdown();
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
                    Utils.GetBitmapFromCanvas(ref canvas).Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void Open_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Image";
            dlg.Filter = "Image files (*.jpg)|*.jpg|(*.png)|*.png|(*.bmp) | *.bmp";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(dlg.FileName);
                Utils.executor.ClearCanvas(ref canvas, ref canvas_scale);
                canvas.Width = image.Width;
                canvas.Height = image.Height;
                canvas.Children.Add(new Image() { Source = Utils.BitmapToImageSource(image) });
            }
        }

        private void Undo_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utils.executor.Undo(ref canvas);
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                Utils.executor.ZoomIn(ref canvas_scale, ref canvas);
            }
            if (e.Key == Key.Q)
            {
                Utils.executor.ZoomOut(ref canvas_scale, ref canvas);
            }
        }

        private void About_menu_item_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Vladislav Parkhutich, 2016",
                "About");
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Info_panel.CanvasSize = Utils.ComposeCanvaSizeLabelContent(canvas.Width, canvas.Height);
        }
    }
}
