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

namespace ClockLib
{
    /// <summary>
    /// Логика взаимодействия для Clock.xaml
    /// </summary>
    public partial class Clock : UserControl
    {
        #region Constants
        private static readonly double secondsLine_RelativeSize = 0.9;
        private static readonly double minutesLine_RelativeSize = 0.7;
        private static readonly double hoursLine_RelativeSize = 0.6;

        private static readonly double hourNums_RelativePosition = 0.85;

        private static readonly double hourMarksStart_RelativePosition = 0.95;
        private static readonly double hourMarksEnd_RelativePosition = 0.99;
        private static readonly double minMarksStart_RelativePosition = 0.96;
        private static readonly double minMarksEnd_RelativePosition = 0.99;
        private static readonly double secMarksStart_RelativePosition = 0.975;
        private static readonly double secMarksEnd_RelativePosition = 0.99;

        private static readonly int hourNumCount = 12;
        private static readonly int hourMarkCount = 4;
        private static readonly int minMarkCount = 12;
        private static readonly int secMarkCount = 60;
        #endregion

        private TextBlock[] hourNums = new TextBlock[hourNumCount];
        private Line[] hourMarks = new Line[hourMarkCount];
        private Line[] minMarks = new Line[minMarkCount];
        private Line[] secMarks = new Line[secMarkCount];

        private bool loaded = false;

        private TimeOnly time = new TimeOnly(0, 0, 0);

        #region Properties
        public TimeOnly Time
        {
            get { return time; }
            set 
            {
                time = value;
                OnTimeChanged();
            }
        }
        #endregion

        #region Events
        public event RoutedEventHandler? TimeChanged;
        public event RoutedEventHandler? HourFontSizeChanged;

        public void OnTimeChanged()
        {
            if (loaded)
            {
                UpdateHoursLine();
                UpdateMinutesLine();
                UpdateSecondsLine();
            }
            TimeChanged?.Invoke(this, new RoutedEventArgs() { Source=this});
        }

        public void OnHourFontSizeChanged()
        {
            HourFontSizeChanged?.Invoke(this, new RoutedEventArgs() { Source=this });
        }
        #endregion

        public Clock()
        {
            InitializeComponent();
            InitializeMarks();
        }

        private void InitializeMarks()
        {
            for(int i = 0; i < hourNumCount; i++)
            {
                hourNums[i] = new TextBlock()
                {
                    Text = (i + 1).ToString(),
                    TextAlignment = TextAlignment.Center
                };
                Background.Children.Add(hourNums[i]);
            }

            for(int i = 0; i < hourMarkCount; i++)
            {
                hourMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                Background.Children.Add(hourMarks[i]);
            }

            for (int i = 0; i < minMarkCount; i++)
            {
                minMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                Background.Children.Add(minMarks[i]);
            }

            for (int i = 0; i < secMarkCount; i++)
            {
                secMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                Background.Children.Add(secMarks[i]);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Background.Height = Background.Width = Math.Min(ActualWidth, ActualHeight);

            UpdateCenterPoint();
            UpdateSecondsLine();
            UpdateMinutesLine();
            UpdateHoursLine();
            UpdateHourNums();
            UpdateHourMarks();
            UpdateMinMarks();
            UpdateSecMarks();
        }

        private void UpdateCenterPoint()
        {
            CenterPoint.Width = Background.Width / 45;
            CenterPoint.Height = Background.Width / 45;
            Canvas.SetLeft(CenterPoint, Background.Width / 2 - CenterPoint.Width / 2);
            Canvas.SetTop(CenterPoint, Background.Height / 2 - CenterPoint.Height / 2);
        }

        private void UpdateSecondsLine()
        {
            SecondsLine.StrokeThickness = Background.Width / 250;
            SecondsLine.X1 = Background.Width / 2;
            SecondsLine.Y1 = Background.Height / 2;
            SecondsLine.X2 = Background.Width * secondsLine_RelativeSize / 2 * Math.Cos((Time.Second - 15) * Math.PI / 30) + SecondsLine.X1;
            SecondsLine.Y2 = Background.Height * secondsLine_RelativeSize / 2 * Math.Sin((Time.Second - 15) * Math.PI / 30) + SecondsLine.Y1;
        }

        private void UpdateMinutesLine()
        {
            MinutesLine.StrokeThickness = Background.Width / 150;
            MinutesLine.X1 = Background.Width / 2;
            MinutesLine.Y1 = Background.Height / 2;
            MinutesLine.X2 = Background.Width * minutesLine_RelativeSize / 2 * Math.Cos((Time.Minute - 15) * Math.PI / 30) + MinutesLine.X1;
            MinutesLine.Y2 = Background.Height * minutesLine_RelativeSize / 2 * Math.Sin((Time.Minute - 15) * Math.PI / 30) + MinutesLine.Y1;
        }

        private void UpdateHoursLine()
        {
            HoursLine.StrokeThickness = Background.Width / 75;
            HoursLine.X1 = Background.Width / 2;
            HoursLine.Y1 = Background.Height / 2;
            HoursLine.X2 = Background.Width * hoursLine_RelativeSize / 2 * Math.Cos((time.Hour - 3) * Math.PI / 6) + HoursLine.X1;
            HoursLine.Y2 = Background.Height * hoursLine_RelativeSize / 2 * Math.Sin((time.Hour - 3) * Math.PI / 6) + HoursLine.Y1;
        }

        private void UpdateHourNums()
        {
            for (int i = 0; i < hourNumCount; i++)
            {
                hourNums[i].FontSize = Background.Height / 13;
                hourNums[i].Height = hourNums[i].FontSize * 1.4;
                hourNums[i].Width = hourNums[i].FontSize * 1.1;
                Canvas.SetLeft(hourNums[i], Background.Width / 2 +
                    Math.Cos((i - 2) * Math.PI / hourNumCount * 2) * Background.Width / 2 * hourNums_RelativePosition - hourNums[i].Width / 2);
                Canvas.SetTop(hourNums[i], Background.Height / 2 +
                    Math.Sin((i - 2) * Math.PI / hourNumCount * 2) * Background.Height / 2 * hourNums_RelativePosition - hourNums[i].Height / 2);
            }
        }

        private void UpdateHourMarks()
        {
            for (int i = 0; i < hourMarkCount; i++)
            {
                hourMarks[i].StrokeThickness = Background.Width / 150;
                hourMarks[i].X1 = Background.Width * hourMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / hourMarkCount * 2) +
                    Background.Width / 2;
                hourMarks[i].Y1 = Background.Height * hourMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / hourMarkCount * 2) +
                    Background.Width / 2;
                hourMarks[i].X2 = Background.Width * hourMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / hourMarkCount * 2) +
                    Background.Width / 2;
                hourMarks[i].Y2 = Background.Height * hourMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / hourMarkCount * 2) +
                    Background.Width / 2;
            }
        }

        private void UpdateMinMarks()
        {
            for (int i = 0; i < minMarkCount; i++)
            {
                minMarks[i].StrokeThickness = Background.Width / 250;
                minMarks[i].X1 = Background.Width * minMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / minMarkCount * 2) +
                    Background.Width / 2;
                minMarks[i].Y1 = Background.Height * minMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / minMarkCount * 2) +
                    Background.Width / 2;
                minMarks[i].X2 = Background.Width * minMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / minMarkCount * 2) +
                    Background.Width / 2;
                minMarks[i].Y2 = Background.Height * minMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / minMarkCount * 2) +
                    Background.Width / 2;
            }
        }

        private void UpdateSecMarks()
        {
            for (int i = 0; i < secMarkCount; i++)
            {
                secMarks[i].StrokeThickness = Background.Width / 350;
                secMarks[i].X1 = Background.Width * secMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / secMarkCount * 2) +
                    Background.Width / 2;
                secMarks[i].Y1 = Background.Height * secMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / secMarkCount * 2) +
                    Background.Width / 2;
                secMarks[i].X2 = Background.Width * secMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / secMarkCount * 2) +
                    Background.Width / 2;
                secMarks[i].Y2 = Background.Height * secMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / secMarkCount * 2) +
                    Background.Width / 2;
            }
        }

        private void Background_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
