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
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.btnstart.Content = "开始";
                    this.Visibility = Visibility.Visible;
                    this.texth.Clear();
                    this.textm.Clear();
                    this.texts.Clear();
                }));
                this.notifyicon.ShowBalloonTip(1, "提醒", "下班啦！下班啦！下班啦！下班啦！下班啦！下班啦！下班啦！", System.Windows.Forms.ToolTipIcon.None);
                MessageBox.Show("请注意！你现在可以下班了！");
            };
        }
        private void NotifyIcoInit()
        {
            this.notifyicon.Icon = Properties.Resources.ico;
            this.notifyicon.ContextMenu = new System.Windows.Forms.ContextMenu();
            Action<object, EventArgs> timeevent = (a, b) =>
             {
                 var st = this.Resources["Normal"] as Storyboard;
                 this.Visibility = Visibility.Visible;
                 st.Begin();
                 st.Completed += (n, m) =>
                 {
                     this.btnstart.Content = "最小化";
                     this._countdown = this._startime.AddHours(9) - DateTime.Now;
                     this._timer.Start();
                 };
             };
            this.notifyicon.ContextMenu.MenuItems.Add("time", new EventHandler(timeevent));
            this.notifyicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(timeevent);
            this.notifyicon.ContextMenu.MenuItems.Add("exit", (o, e) => Window_Closed(null, null));
            this.notifyicon.Text = "下班提醒服务正在运行...";
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
                if (this.btnstart.Content as string == "最小化")
                {
                    var sb = this.Resources["Mini"] as Storyboard;
                    sb.Begin();
                    sb.Completed += (a, b) =>
                    {
                        this.btnstart.Content = "开始";
                        this._timer.Stop();
                        this.Visibility = Visibility.Hidden;
                        //this.notifyicon.ShowBalloonTip(1, "提醒", "提醒服务最小化在任务栏运行", System.Windows.Forms.ToolTipIcon.Info);
                    };
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
                    MessageBox.Show("你已经可以下班啦！");
                    return;
                }

                this._startime = time.Value;
                this._countdown = this._startime.AddHours(9) - DateTime.Now;
                this._remindtimer.Interval = this._countdown.TotalMilliseconds;
                this._timer.Start();
                this._remindtimer.Start();

                this.btnstart.Content = "最小化";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
