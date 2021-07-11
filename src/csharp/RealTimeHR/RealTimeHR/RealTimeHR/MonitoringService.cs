using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

using RealTimeHR.Helper;

namespace RealTimeHR
{
    [Service]
    public class MonitoringService : Service
    {
        private HeartRateHelper HRHelper => HeartRateHelper.Instance;
        private Timer measureTimer;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (measureTimer == null)
            {
                measureTimer = new Timer();
                //measureTimer = new Timer(MeasureHR, new AutoResetEvent(false), Timeout.Infinite, Timeout.Infinite);
                measureTimer.Elapsed += MeasureHR;
                measureTimer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
                measureTimer.Stop();
            }

            ChangeInterval(10);

            measureTimer.Start();

            return StartCommandResult.Sticky;
        }

        private void MeasureHR(object sender, EventArgs e)
        {
            measureTimer.Stop();

            Vibrator vibrator = ApplicationContext.GetSystemService(VibratorService) as Vibrator;

            vibrator.Vibrate(VibrationEffect.CreateOneShot(300, VibrationEffect.DefaultAmplitude));

            HRHelper.HRDataChanged += FinalizeMeasure;

            HRHelper.StartSensor();
        }

        private void FinalizeMeasure(object sender, int data)
        {
            HRHelper.StopSensor();
            HRHelper.HRDataChanged -= FinalizeMeasure;

            DateTime now = DateTime.Now;

            RecordHelper.WriteData($"{now:yyyyMMddHHmmss} {data}");

            measureTimer.Start();
        }

        public void ChangeInterval(int min)
        {
            //measureTimer.Change(TimeSpan.FromSeconds(min), TimeSpan.FromSeconds(min));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            //measureTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            measureTimer?.Stop();
            measureTimer?.Dispose();
        }
    }
}