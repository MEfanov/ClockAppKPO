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
    }
}
