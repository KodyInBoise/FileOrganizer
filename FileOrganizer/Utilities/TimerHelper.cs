using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FileOrganizer
{
    class TimerHelper
    {
        public static DispatcherTimer HourlyTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(1, 0, 0);

            return timer;
        }

        public static DispatcherTimer DailyTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(24, 0, 0);

            return timer;
        }

        public static DispatcherTimer CustomTimer(TimeSpan t)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = t;

            return timer;
        }
    }
}
