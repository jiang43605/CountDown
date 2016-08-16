using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WorkRemind
{
    public static class TimerExtension
    {
        public static void ForceStop(this Timer timer, string flag)
        {
            AppDomain.CurrentDomain.SetData(flag, false);
            timer.Stop();
        }
        public static bool Flag(this Timer timer, string flag)
        {
            return (bool)AppDomain.CurrentDomain.GetData(flag);
        }
        public static void ForceStart(this Timer timer, string flag)
        {
            AppDomain.CurrentDomain.SetData(flag, true);
            timer.Start();
        }
    }
}
