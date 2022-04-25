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
    public enum ByteBoxMode
    {
        Decimal = 0, Hexadecimal = 1
    }

    public partial class ByteBox : UserControl
    {
        public event RoutedEventHandler ValueChanged;



        private string Text
        {
            get { return Body.Text; }
            set { Body.Text = value; }
        }

        private ByteBoxMode _mode;
        public ByteBoxMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;

                if (value == ByteBoxMode.Decimal)
                    ConvertTextToDecimal();
                else if (value == ByteBoxMode.Hexadecimal)
                    ConvertTextToHexadecimal();
            }
        }

        public byte Value
        {
            get 
            {
                if (TryGetValue(out byte res)) return res;
                return 0;
            }
            set
            {
                SetValue(value);
            }
        }

        public bool InputIsValid { get; private set; }



        public ByteBox()
        {
            InitializeComponent();
        }

        private void Body_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Mode == ByteBoxMode.Decimal)
            {
                if (byte.TryParse(Text, out byte value))
                    InputIsValid = true;
                else InputIsValid = false;
            }
            else
            {
                if (byte.TryParse(Text, System.Globalization.NumberStyles.HexNumber, null, out byte value))
                    InputIsValid = true;
                else InputIsValid = false;
            }

            if(InputIsValid)
            {
                Body.Foreground = new SolidColorBrush(Colors.Black);
                OnValueChanged();
            }
            else Body.Foreground = new SolidColorBrush(Colors.Red);
        }

        public bool TryGetValue(out byte value)
        {
            value = 0;

            if (Mode == ByteBoxMode.Decimal)
                return byte.TryParse(Text, out value);
            else if (Mode == ByteBoxMode.Hexadecimal)
                return byte.TryParse(Text, System.Globalization.NumberStyles.HexNumber, null, out value);
            else return false;
        }

        public void SetValue(byte value)
        {
            Text = value.ToString();

            if (Mode == ByteBoxMode.Decimal)
                ConvertTextToDecimal();
            else if (Mode == ByteBoxMode.Hexadecimal)
                ConvertTextToHexadecimal();
        }

        private void ConvertTextToDecimal()
        {
            if (byte.TryParse(Text, System.Globalization.NumberStyles.HexNumber, null, out byte value))
                Text = value.ToString();
        }

        private void ConvertTextToHexadecimal()
        {
            if (byte.TryParse(Text, out byte value))
                Text = value.ToString("X");
        }



        public void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new RoutedEventArgs());
        }
    }
}
