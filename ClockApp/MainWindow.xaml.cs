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
using ClockLib;
using ColorPickerLib;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TimeZoneExpander.Header = TimeZoneInfo.Local;

            foreach(TimeZoneInfo? info in TimeZoneInfo.GetSystemTimeZones())
            {
                Button btn = new Button();
                btn.Content = info;
                btn.Click += TimeZoneButton_Click;
                TimeZonePanel.Children.Add(btn);
            }
        }

        private void TimeZoneButton_Click(object sender, RoutedEventArgs e)
        {
            TimeZoneInfo? info = (TimeZoneInfo)(((Button)sender).Content);
            AppClock.TimeZoneId = info.Id;
            TimeZoneExpander.Header = info;
        }

        private void ClockContextMenu_ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorPickWindow colorPickWindow = new ColorPickWindow();
            colorPickWindow.ShowDialog();
            
            if(colorPickWindow.Picked)
                AppClock.Background = new SolidColorBrush(colorPickWindow.Color);
        }

        private void ClockContextMenu_ChangeFill_Click(object sender, RoutedEventArgs e)
        {
            ColorPickWindow colorPickWindow = new ColorPickWindow();
            colorPickWindow.ShowDialog();

            if (colorPickWindow.Picked)
                AppClock.Fill = new SolidColorBrush(colorPickWindow.Color);
        }

        private void ClockContextMenu_ChangeArrowBrush_Click(object sender, RoutedEventArgs e)
        {
            ColorPickWindow colorPickWindow = new ColorPickWindow();
            colorPickWindow.ShowDialog();

            if (colorPickWindow.Picked)
                AppClock.ArrowBrush = new SolidColorBrush(colorPickWindow.Color);
        }

        private void ClockContextMenu_ChangeMarkBrush_Click(object sender, RoutedEventArgs e)
        {
            ColorPickWindow colorPickWindow = new ColorPickWindow();
            colorPickWindow.ShowDialog();

            if (colorPickWindow.Picked)
                AppClock.MarkBrush = new SolidColorBrush(colorPickWindow.Color);
        }
    }
}
