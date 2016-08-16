using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkRemind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon notifyicon = new System.Windows.Forms.NotifyIcon();
        Timer _timer = new Timer(1000);
        Timer _remindtimer = new Timer();
        Timer _autominitimer = new Timer(1000 * 5);
        TimeSpan _countdown;
        DateTime _startime;
        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (o, e) => this.DragMove();

            this.NotifyIcoInit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._timer.Elapsed += (a, b) =>
            {
                if (this._countdown.TotalSeconds == 0) { this._timer.Stop(); return; }
                this._countdown = this._countdown.Add(TimeSpan.FromSeconds(-1));
                var datetime = DateTime.Parse($"{this._countdown.Hours}:{this._countdown.Minutes}:{this._countdown.Seconds}");
                SetTxtTime(datetime);
            };

            this._remindtimer.Elapsed += (a, b) =>
            {
                this._remindtimer.Stop();
                // TODO 可能出现时间差导致timer刚刚运行，更改的内容在clear()之后
                this._timer.Stop();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.btnstart.Content = Properties.Resources.BtnstartBegin;
                    this.texth.Clear();
                    this.textm.Clear();
                    this.texts.Clear();
                }));
                this.notifyicon.ShowBalloonTip(1, Properties.Resources.BalloonTipTitle, Properties.Resources.BalloonTipText, System.Windows.Forms.ToolTipIcon.None);
                MessageBox.Show(Properties.Resources.MessageBoxShow);
            };

            this._autominitimer.Elapsed += (a, b) =>
            {
                this._autominitimer.Stop();
                if (!this._autominitimer.Flag(nameof(_autominitimer))) return;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.lbworkstart.Content = this._startime.ToString("HH:mm:ss");
                    this.lbworkend.Content = this._startime.AddHours(9).ToString("HH:mm:ss");
                    var st = this.Resources["TransMiniModel"] as Storyboard;
                    st.Begin();
                }));
            };
        }
        private void NotifyIcoInit()
        {
            this.notifyicon.Icon = Properties.Resources.ico;
            this.notifyicon.ContextMenu = new System.Windows.Forms.ContextMenu();
            Action<object, EventArgs> timeevent = (a, b) =>
             {
                 grid_MouseRightButtonUp(null, null);
                 var st = this.Resources["Normal"] as Storyboard;
                 this.Visibility = Visibility.Visible;
                 st.Completed += (n, m) =>
                 {
                     if (string.IsNullOrEmpty(this.texth.Text) || string.IsNullOrEmpty(this.textm.Text) || string.IsNullOrEmpty(this.texts.Text)) return;
                     this.btnstart.Content = Properties.Resources.BtnstartMini;
                     this._countdown = this._startime.AddHours(9) - DateTime.Now;
                     this._timer.Start();
                 };
                 st.Begin();
             };
            this.notifyicon.ContextMenu.MenuItems.Add("time", new EventHandler(timeevent));
            this.notifyicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(timeevent);
            this.notifyicon.ContextMenu.MenuItems.Add("exit", (o, e) => Window_Closed(null, null));
            this.notifyicon.Text = Properties.Resources.NotifyiconText;
            this.notifyicon.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.notifyicon.Dispose();
            Environment.Exit(0);
        }
        private DateTime? GetTxtTime()
        {
            try
            {
                var h = this.texth.Text;
                var m = this.textm.Text;
                var s = this.texts.Text;

                if (string.IsNullOrEmpty(h) && string.IsNullOrEmpty(m) && string.IsNullOrEmpty(s)) return null;

                return DateTime.Parse($"{h}:{m}:{s}");
            }
            catch
            {
                throw new Exception("time format are error, please check!");
            }


        }
        private void SetTxtTime(DateTime time)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
             {
                 this.texth.Text = time.ToString("HH");
                 this.textm.Text = time.ToString("mm");
                 this.texts.Text = time.ToString("ss");
             }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.btnstart.Content as string == Properties.Resources.BtnstartMini)
                {
                    this._autominitimer.ForceStop(nameof(this._autominitimer));
                    var sb = this.Resources["Mini"] as Storyboard;
                    sb.Completed += (a, b) =>
                    {
                        //this.btnstart.Content = Properties.Resources.BtnstartBegin;
                        this._timer.Stop();
                        this.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();
                    return;
                }

                var time = GetTxtTime();
                if (time == null)
                {
                    time = DateTime.Now;
                    SetTxtTime(time.Value);
                }

                if (time.Value.AddHours(9) <= DateTime.Now)
                {
                    MessageBox.Show(Properties.Resources.MessageBoxShowAlreadyOff);
                    return;
                }

                this._startime = time.Value;
                this._countdown = this._startime.AddHours(9) - DateTime.Now;
                this._remindtimer.Interval = this._countdown.TotalMilliseconds;
                this._timer.Start();
                this._remindtimer.Start();

                this.btnstart.Content = Properties.Resources.BtnstartMini;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.grid3.Visibility == Visibility.Visible) return;
            if (string.IsNullOrEmpty(this.texth.Text) || string.IsNullOrEmpty(this.textm.Text) || string.IsNullOrEmpty(this.texts.Text)) return;
            if (this.grid.IsMouseOver) return;
            this._autominitimer.ForceStart(nameof(this._autominitimer));
            this._timer.Start();
            this.SetReadOnly(true);
        }

        private void grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var st = this.Resources["TransMiniModel"] as Storyboard;
            st.Seek(TimeSpan.FromSeconds(0)); st.Stop();
            this._autominitimer.ForceStop(nameof(this._autominitimer));
            this.SetReadOnly(false);
        }

        private void SetReadOnly(bool status)
        {
            this.texth.IsReadOnly = status;
            this.textm.IsReadOnly = status;
            this.texts.IsReadOnly = status;
        }
    }
}
