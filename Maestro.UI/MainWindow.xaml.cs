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
		private Executor executor;

		public MainWindow()
		{
			executor = new Executor();
			InitializeComponent();
		}

		//private enum Tools { Pencil, Eraser, Magnifier, ColorPicker, Fill, Text, Line, Rectangle, Ellipse }
		//private Tools Tool { get; set; }

		#region Tools buttons listeners

		//private void pencil_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.Pencil;
		//}

		//private void eraser_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.Eraser;
		//}

		//private void color_picker_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.ColorPicker;
		//}

		//private void magnifier_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.Magnifier;
		//	MessageBox.Show("Press 'W' for zoom in and 'Q' for zoom out.",
		//		"instruction");
		//}

		//private void fill_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.Fill;
		//}

		//private void text_button_Click(object sender, RoutedEventArgs e)
		//{
		//	this.Tool = Tools.Text;
		//}

		//private void line_button_Click(object sender, RoutedEventArgs e)
		//{
		//	Utils.Tool = Utils.Tools.Line;
		//}

		//private void ellipse_button_Click(object sender, RoutedEventArgs e)
		//{
  //          Utils.Tool = Utils.Tools.Ellipse;
		//}

		//private void rectangle_button_Click(object sender, RoutedEventArgs e)
		//{
  //          Utils.Tool = Utils.Tools.Rectangle;
		//}

		#endregion

		private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			executor.CleanShapes();
		}

		private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Pressed)
			{
				switch (Utils.Tool)
				{
					case Utils.Tools.Pencil:
						executor.DrawWithPencil(ref canvas, e.GetPosition(canvas));
						break;

					case Utils.Tools.Eraser:
						executor.Erase(ref canvas, e.GetPosition(canvas));
						break;

					case Utils.Tools.ColorPicker:
						ColorPicker.SelectedColor = Utils.GetPixelColor(e.GetPosition(canvas), ref canvas);
						break;

					case Utils.Tools.Line:
						executor.DrawWithLine(ref canvas, e.GetPosition(canvas));
						break;

					case Utils.Tools.Rectangle:
						executor.DrawWithRectangle(ref canvas, e.GetPosition(canvas));
						break;

					case Utils.Tools.Ellipse:
						executor.DrawWithEllipse(ref canvas, e.GetPosition(canvas));
						break;

					case Utils.Tools.Fill:
						executor.MakeFloodFill(ref canvas, e.GetPosition(canvas));
						break;
					case Utils.Tools.Text:
						executor.DrawText(sender, e.GetPosition(canvas), ref canvas, ref textBox,
							(FontPicker.SelectedItem as ComboBoxItem).Content as string,
							(TextSizePicker.SelectedItem as ComboBoxItem).Content as string);
						break;
				}
				executor.CurrentPoint = e.GetPosition(canvas);
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
								executor.HoldPressedPencilAndMove(e.GetPosition(canvas));
							}
						}
						break;

					case Utils.Tools.Line:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								executor.HoldPressedLineAndMove(e.GetPosition(canvas));
							}
						}
						break;

					case Utils.Tools.Rectangle:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								executor.HoldPressedRecatngleAndMove(e.GetPosition(canvas));
							}
						}
						break;

					case Utils.Tools.Ellipse:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								executor.HoldPressedEllipseAndMove(e.GetPosition(canvas));
							}
						}
						break;

					case Utils.Tools.Eraser:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								executor.HoldPressedEraserAndMove(e.GetPosition(canvas));
							}
						}
						break;

					default:
						break;
				}
			}
			Position_label.Content = Utils.ComposePositionLabelContent(e.GetPosition(canvas));
		}

		private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			executor.Color = ColorPicker.SelectedColor.GetValueOrDefault();
		}

		private void ThicknessChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selectedItemText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
			try
			{
				executor.Thickness = Double.Parse(selectedItemText.Substring(0, 1));
			}
			catch (Exception)
			{
				executor.Thickness = 1;
			}
		}

		private void text_bold_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref executor.boldText);
		}

		private void text_italic_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref executor.italicText);
		}

		private void text_underline_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref executor.underlinedText);
		}

		private void canvas_MouseEnter(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Cross;
			canvas.Focus();
			Position_label.Content = Utils.ComposePositionLabelContent(e.GetPosition(canvas));
		}

		private void canvas_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Arrow;
			Position_label.Content = "Позиция курсора: ";
		}

		private void Clear_Screen_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			executor.ClearCanvas(ref canvas, ref canvas_scale);
		}

		private void Exit_menu_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			executor.ClearCanvas(ref canvas, ref canvas_scale);
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
				executor.ClearCanvas(ref canvas, ref canvas_scale);
				canvas.Width = image.Width;
				canvas.Height = image.Height;
				canvas.Children.Add(new Image() { Source = Utils.BitmapToImageSource(image) });
			}
		}

		private void Undo_menu_item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			executor.Undo(ref canvas);
		}

		private void canvas_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.W)
			{
				executor.ZoomIn(ref canvas_scale, ref canvas);
			}
			if (e.Key == Key.Q)
			{
				executor.ZoomOut(ref canvas_scale, ref canvas);
			}
		}

		private void About_menu_item_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Created by Vladislav Parkhutich, 2016",
				"About");
		}
    }
}
