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
using System.Windows.Shapes;

namespace ColorPickerLib
{
    public partial class ColorPickWindow : Window
    {
        public Color Color
        {
            get 
            {
                return ColorPicker.Color; 
            } 
            set
            {
                ColorPicker.Color = value;
            } 
        }
        public bool Picked { get; private set; }

        public ColorPickWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Picked = false;
            Close();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ColorPicker.InputIsValid)
            {
                MessageBox.Show("Поля ввода могут содержать только целые " +
                    "положительные числа от 0 до 255 включительно", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                Picked = true;
                Close();
            }
        }
    }
}
