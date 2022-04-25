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

namespace ColorPickerLib
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private bool loaded = false;



        public Color Color
        {
            get
            {
                if (loaded && InputIsValid)
                    return Color.FromRgb(RedBox.Value, GreenBox.Value, BlueBox.Value);
                else return Colors.Black;
            }
            set
            {
                RedBox.Value = value.R;
                GreenBox.Value = value.G;
                BlueBox.Value = value.B;
            }
        }

        public bool InputIsValid
        {
            get { return RedBox.InputIsValid && GreenBox.InputIsValid && BlueBox.InputIsValid; }
        }



        public ColorPicker()
        {
            InitializeComponent();
        }

        private void DecimalButton_Checked(object sender, RoutedEventArgs e)
        {
            RedBox.Mode = ByteBoxMode.Decimal;
            GreenBox.Mode = ByteBoxMode.Decimal;
            BlueBox.Mode = ByteBoxMode.Decimal;
        }

        private void HexadecimalButton_Checked(object sender, RoutedEventArgs e)
        {
            RedBox.Mode = ByteBoxMode.Hexadecimal;
            GreenBox.Mode = ByteBoxMode.Hexadecimal;
            BlueBox.Mode = ByteBoxMode.Hexadecimal;
        }

        private void ByteBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (loaded && InputIsValid)
                ColorIndicator.Fill = new SolidColorBrush(Color);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            if (InputIsValid)
                ColorIndicator.Fill = new SolidColorBrush(Color);
            else ColorIndicator.Fill = new SolidColorBrush(Colors.Black);
        }
    }
}
