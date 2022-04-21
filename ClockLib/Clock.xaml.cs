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
        private double secondsLineStart_RelativePosition = 0.2;
        private double secondsLineEnd_RelativePosition = 0.9;
        private double minutesLineStart_RelativePosition = 0;
        private double minutesLineEnd_RelativePosition = 0.7;
        private double hoursLineStart_RelativePosition = 0;
        private double hoursLineEnd_RelativePosition = 0.6;

        private double secondsLine_RelativeThickness = 1d / 250;
        private double minutesLine_RelativeThickness = 1d / 150;
        private double hoursLine_RelativeThickness = 1d / 75;

        private double hourNums_RelativePosition = 0.85;
        private double hourNums_FontToHeight = 1.4;
        private double hourNums_FontToWidth = 1.1;
        private double hourNums_RelativeFontSize = 1d / 13;

        private double hourMarksStart_RelativePosition = 0.95;
        private double hourMarksEnd_RelativePosition = 0.99;
        private double hourMarks_RelativeThickness = 1d / 150;
        private double minMarksStart_RelativePosition = 0.96;
        private double minMarksEnd_RelativePosition = 0.99;
        private double minMarks_RelativeThickness = 1d / 250;
        private double secMarksStart_RelativePosition = 0.975;
        private double secMarksEnd_RelativePosition = 0.99;
        private double secMarks_RelativeThickness = 1d / 350;

        private int hourNumCount = 12;
        private int hourMarkCount = 4;
        private int minMarkCount = 12;
        private int secMarkCount = 60;

        private TextBlock[] hourNums;
        private Line[] hourMarks;
        private Line[] minMarks;
        private Line[] secMarks;

        private bool loaded = false;

        private System.Windows.Threading.DispatcherTimer timer;

        private TimeSpan time = new TimeSpan(0, 0, 0, 0, 0);
        private TimeSpan interval = new TimeSpan(0, 0, 0, 0, 15);
        private TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
        private bool snap = true;
        private bool isRunning = false;

        private double debug_SpeedUp = 1;

        #region Properties
        public TimeSpan Time
        {
            get { return time; }
            set 
            {
                time = value;
                OnTimeChanged();
            }
        }
        public TimeSpan Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                timer.Interval = interval;
            }
        }
        public string TimeZoneId
        {
            get { return timeZoneInfo.Id; }
            set
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(value);
                UpdateTime();
            }
        }
        public bool Snap
        {
            get { return snap; }
            set
            {
                snap = value;
                if(loaded)
                {
                    UpdateSecondsLine();
                    UpdateMinutesLine();
                    UpdateHoursLine();
                }
            }
        }
        public bool IsRunning
        {
            get { return isRunning; }
            set 
            {
                if(isRunning != value)
                {
                    isRunning = value;
                    if (isRunning) Run();
                    else Stop();
                }
            }
        }


        public Brush Stroke
        {
            get { return Body.Stroke; }
            set { Body.Stroke = value; }
        }

        public Brush Fill
        {
            get { return Body.Fill; }
            set { Body.Fill = value; }
        }

        public Brush ArrowBrush
        {
            get { return SecondsLine.Stroke; }
            set
            {
                SecondsLine.Stroke = value;
                MinutesLine.Stroke = value;
                HoursLine.Stroke = value;
                CenterPoint.Fill = value;
            }
        }

        public Brush? MarksBrush
        {
            get
            {
                if(hourMarkCount == 0)
                    return null;
                return hourMarks[0].Stroke;
            }
            set
            {
                foreach(var hourMark in hourMarks)
                    hourMark.Stroke = value;

                foreach(var minMark in minMarks)
                    minMark.Stroke = value;

                foreach(var secMark in secMarks)
                    secMark.Stroke = value;

                foreach (var hourNum in hourNums)
                    hourNum.Foreground = value;
            }
        }
        #endregion

        #region Events
        public event RoutedEventHandler? TimeChanged;
        public event RoutedEventHandler? Tick;

        public void OnTimeChanged()
        {
            if (loaded)
            {
                UpdateHoursLine();
                UpdateMinutesLine();
                UpdateSecondsLine();
            }

            TimeChanged?.Invoke(this, new RoutedEventArgs());
        }

        public void OnTick()
        {
            Tick?.Invoke(this, new RoutedEventArgs());
        }
        #endregion

        public Clock()
        {
            InitializeComponent();
            InitializeClock();

            UpdateTime();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += UpdateTime;
            timer.Interval = Interval;
        }

        private void InitializeClock()
        {
            hourNums = new TextBlock[hourNumCount];
            hourMarks = new Line[hourMarkCount];
            minMarks = new Line[minMarkCount];
            secMarks = new Line[secMarkCount];

            for(int i = 0; i < hourNumCount; i++)
            {
                hourNums[i] = new TextBlock()
                {
                    Text = (i + 1).ToString(),
                    TextAlignment = TextAlignment.Center
                };
                BackgroundArea.Children.Add(hourNums[i]);
            }

            for(int i = 0; i < hourMarkCount; i++)
            {
                hourMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                BackgroundArea.Children.Add(hourMarks[i]);
            }

            for (int i = 0; i < minMarkCount; i++)
            {
                minMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                BackgroundArea.Children.Add(minMarks[i]);
            }

            for (int i = 0; i < secMarkCount; i++)
            {
                secMarks[i] = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
                BackgroundArea.Children.Add(secMarks[i]);
            }
        }

        #region UpdateAppearance
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BackgroundArea.Height = BackgroundArea.Width = Math.Min(ActualWidth, ActualHeight);

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
            CenterPoint.Width = BackgroundArea.Width / 45;
            CenterPoint.Height = BackgroundArea.Width / 45;
            Canvas.SetLeft(CenterPoint, BackgroundArea.Width / 2 - CenterPoint.Width / 2);
            Canvas.SetTop(CenterPoint, BackgroundArea.Height / 2 - CenterPoint.Height / 2);
        }

        private void UpdateSecondsLine()
        {
            SecondsLine.StrokeThickness = BackgroundArea.Width * secondsLine_RelativeThickness;
            if (snap)
            {
                SecondsLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * secondsLineStart_RelativePosition * 
                    Math.Cos((Time.Seconds - 15) * Math.PI / 30);
                SecondsLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * secondsLineStart_RelativePosition * 
                    Math.Sin((Time.Seconds - 15) * Math.PI / 30);
                SecondsLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * secondsLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.Seconds - 15) * Math.PI / 30);
                SecondsLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * secondsLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.Seconds - 15) * Math.PI / 30);
            }
            else
            {
                SecondsLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * secondsLineStart_RelativePosition * 
                    Math.Cos((Time.TotalSeconds - 15) * Math.PI / 30);
                SecondsLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * secondsLineStart_RelativePosition * 
                    Math.Sin((Time.TotalSeconds - 15) * Math.PI / 30);
                SecondsLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * secondsLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.TotalSeconds - 15) * Math.PI / 30);
                SecondsLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * secondsLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.TotalSeconds - 15) * Math.PI / 30);
            }
        }

        private void UpdateMinutesLine()
        {
            if (snap)
            {
                MinutesLine.StrokeThickness = BackgroundArea.Width * minutesLine_RelativeThickness;
                MinutesLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * minutesLineStart_RelativePosition * 
                    Math.Cos((Time.Minutes - 15) * Math.PI / 30);
                MinutesLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * minutesLineStart_RelativePosition *
                    Math.Sin((Time.Minutes - 15) * Math.PI / 30);
                MinutesLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * minutesLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.Minutes - 15) * Math.PI / 30);
                MinutesLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * minutesLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.Minutes - 15) * Math.PI / 30);
            }
            else
            {
                MinutesLine.StrokeThickness = BackgroundArea.Width * minutesLine_RelativeThickness;
                MinutesLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * minutesLineStart_RelativePosition * 
                    Math.Cos((Time.TotalMinutes - 15) * Math.PI / 30);
                MinutesLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * minutesLineStart_RelativePosition * 
                    Math.Sin((Time.TotalMinutes - 15) * Math.PI / 30);
                MinutesLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * minutesLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.TotalMinutes - 15) * Math.PI / 30);
                MinutesLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * minutesLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.TotalMinutes - 15) * Math.PI / 30);
            }
        }

        private void UpdateHoursLine()
        {
            if (snap)
            {
                HoursLine.StrokeThickness = BackgroundArea.Width * hoursLine_RelativeThickness;
                HoursLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * hoursLineStart_RelativePosition * 
                    Math.Cos((Time.Hours - 3) * Math.PI / 6);
                HoursLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * hoursLineStart_RelativePosition * 
                    Math.Sin((Time.Hours - 3) * Math.PI / 6);
                HoursLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * hoursLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.Hours - 3) * Math.PI / 6);
                HoursLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * hoursLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.Hours - 3) * Math.PI / 6);
            }
            else
            {
                HoursLine.StrokeThickness = BackgroundArea.Width * hoursLine_RelativeThickness;
                HoursLine.X1 = BackgroundArea.Width / 2 - BackgroundArea.Width / 2 * hoursLineStart_RelativePosition * 
                    Math.Cos((Time.TotalHours - 3) * Math.PI / 6);
                HoursLine.Y1 = BackgroundArea.Height / 2 - BackgroundArea.Height / 2 * hoursLineStart_RelativePosition * 
                    Math.Sin((Time.TotalHours - 3) * Math.PI / 6);
                HoursLine.X2 = BackgroundArea.Width / 2 + BackgroundArea.Width * hoursLineEnd_RelativePosition / 2 * 
                    Math.Cos((Time.TotalHours - 3) * Math.PI / 6);
                HoursLine.Y2 = BackgroundArea.Height / 2 + BackgroundArea.Height * hoursLineEnd_RelativePosition / 2 * 
                    Math.Sin((Time.TotalHours - 3) * Math.PI / 6);
            }
        }

        private void UpdateHourNums()
        {
            for (int i = 0; i < hourNumCount; i++)
            {
                hourNums[i].FontSize = BackgroundArea.Height * hourNums_RelativeFontSize;
                hourNums[i].Height = hourNums[i].FontSize * hourNums_FontToHeight;
                hourNums[i].Width = hourNums[i].FontSize * hourNums_FontToWidth;
                Canvas.SetLeft(hourNums[i], BackgroundArea.Width / 2 +
                    Math.Cos((i - 2) * Math.PI / hourNumCount * 2) * BackgroundArea.Width / 2 * hourNums_RelativePosition - hourNums[i].Width / 2);
                Canvas.SetTop(hourNums[i], BackgroundArea.Height / 2 +
                    Math.Sin((i - 2) * Math.PI / hourNumCount * 2) * BackgroundArea.Height / 2 * hourNums_RelativePosition - hourNums[i].Height / 2);
            }
        }

        private void UpdateHourMarks()
        {
            for (int i = 0; i < hourMarkCount; i++)
            {
                hourMarks[i].StrokeThickness = BackgroundArea.Width * hourMarks_RelativeThickness;
                hourMarks[i].X1 = BackgroundArea.Width * hourMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / hourMarkCount * 2) +
                    BackgroundArea.Width / 2;
                hourMarks[i].Y1 = BackgroundArea.Height * hourMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / hourMarkCount * 2) +
                    BackgroundArea.Width / 2;
                hourMarks[i].X2 = BackgroundArea.Width * hourMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / hourMarkCount * 2) +
                    BackgroundArea.Width / 2;
                hourMarks[i].Y2 = BackgroundArea.Height * hourMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / hourMarkCount * 2) +
                    BackgroundArea.Width / 2;
            }
        }

        private void UpdateMinMarks()
        {
            for (int i = 0; i < minMarkCount; i++)
            {
                minMarks[i].StrokeThickness = BackgroundArea.Width * minMarks_RelativeThickness;
                minMarks[i].X1 = BackgroundArea.Width * minMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / minMarkCount * 2) +
                    BackgroundArea.Width / 2;
                minMarks[i].Y1 = BackgroundArea.Height * minMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / minMarkCount * 2) +
                    BackgroundArea.Width / 2;
                minMarks[i].X2 = BackgroundArea.Width * minMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / minMarkCount * 2) +
                    BackgroundArea.Width / 2;
                minMarks[i].Y2 = BackgroundArea.Height * minMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / minMarkCount * 2) +
                    BackgroundArea.Width / 2;
            }
        }

        private void UpdateSecMarks()
        {
            for (int i = 0; i < secMarkCount; i++)
            {
                secMarks[i].StrokeThickness = BackgroundArea.Width * secMarks_RelativeThickness;
                secMarks[i].X1 = BackgroundArea.Width * secMarksStart_RelativePosition / 2 * Math.Cos(i * Math.PI / secMarkCount * 2) +
                    BackgroundArea.Width / 2;
                secMarks[i].Y1 = BackgroundArea.Height * secMarksStart_RelativePosition / 2 * Math.Sin(i * Math.PI / secMarkCount * 2) +
                    BackgroundArea.Width / 2;
                secMarks[i].X2 = BackgroundArea.Width * secMarksEnd_RelativePosition / 2 * Math.Cos(i * Math.PI / secMarkCount * 2) +
                    BackgroundArea.Width / 2;
                secMarks[i].Y2 = BackgroundArea.Height * secMarksEnd_RelativePosition / 2 * Math.Sin(i * Math.PI / secMarkCount * 2) +
                    BackgroundArea.Width / 2;
            }
        }
        #endregion

        private void Background_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private void Run()
        {
            timer.Start();
        }

        private void Stop()
        {
            timer.Stop();
        }

        private void UpdateTime(object? sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).TimeOfDay;
        }
    }
}
