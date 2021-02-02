using System;
using System.Windows.Forms;

namespace brake
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BrakeAppApplicationContext());
        }
    }

    public class BrakeAppApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private bool red = false;

        private Timer mainTimer;
        private DateTime lastReset;
        private const int minutes = 20;
        private const int seconds = 60 * minutes;

        private void GoRed()
        {
            if (!red)
            {
                trayIcon.Icon = Properties.Resources.red;
                red = true;
            }
        }

        private void GoGreen()
        {
            if (red)
            {
                trayIcon.Icon = Properties.Resources.green;
                red = false;
            }
        }

        public BrakeAppApplicationContext()
        {
            lastReset = DateTime.Now;
            trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.green,
                Text = "This goes red every " + minutes + " minutes. Click to reset.", // TODO
                ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Exit", Exit) }),
                Visible = true,
            };
            trayIcon.Click += new EventHandler(notifyIcon_Click);
            trayIcon.MouseMove += new MouseEventHandler(notifyIcon_MouseMove);
            mainTimer = new Timer { Interval = 1000 * seconds };
            mainTimer.Enabled = true;
            mainTimer.Tick += new EventHandler(Timer_Tick);
        }

        private void notifyIcon_MouseMove(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now.Subtract(lastReset);
            int totalMinutes = (int)span.TotalMinutes;

            if (red)
            {
                string minutesString = "minute";
                if (totalMinutes > 1)
                {
                    minutesString = "minutes";
                }
                trayIcon.Text = totalMinutes + " " + minutesString + " since last reset. Click to reset";
            }
            else
            {
                int minutesLeft = 0;
                string minutesString = "minute";
                if (totalMinutes < minutes)
                {
                    minutesLeft = minutes - totalMinutes;
                }
                if (minutesLeft > 1)
                {
                    minutesString = "minutes";
                } 
                trayIcon.Text = minutesLeft + " " + minutesString +  " to timeout";
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            lastReset = DateTime.Now;
            var eventArgs = e as MouseEventArgs;
            switch (eventArgs.Button)
            {
                case MouseButtons.Left:
                    // Left click resets icon and timer
                    GoGreen();
                    mainTimer.Stop();
                    mainTimer.Interval = 1000 * seconds;
                    mainTimer.Start();
                    break;
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            GoRed();
        }

        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            mainTimer.Stop();
            mainTimer.Dispose();
            Application.Exit();
        }
    }
}
