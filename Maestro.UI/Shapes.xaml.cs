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

namespace Maestro.UI
{
    /// <summary>
    /// Interaction logic for Shapes.xaml
    /// </summary>
    public partial class Shapes : UserControl
    {
        public Shapes()
        {
            InitializeComponent();
        }

        private void line_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Line;
        }

        private void ellipse_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Ellipse;
        }

        private void rectangle_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Rectangle;
        }
    }
}
