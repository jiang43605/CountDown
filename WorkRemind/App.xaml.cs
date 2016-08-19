using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WorkRemind
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void App_Startup(object sender, StartupEventArgs e)
        {
            var win = new MainWindow();

            if (e.Args.Length > 0)
            {
                string file = e.Args[0];
                if (file == "i")
                {
                    //win.Show();
                    win.Show();
                }
                else
                {
                    Console.Write("das");
                    MessageBox.Show("aa");
                    win.Close();
                }
            }
            else
            {
                win.Show();
            }
        }
    }
}
