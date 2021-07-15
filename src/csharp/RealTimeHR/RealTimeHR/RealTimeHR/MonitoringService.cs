using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;

using RealTimeHR.Helper;

using System;
using System.Timers;

using Xamarin.Essentials;

using static Android.OS.PowerManager;

namespace RealTimeHR
{
    [Service(Name = "com.urk.realtimehr.monitoringservice", Process = ":monitoring_service", Exported = true)]
    public class MonitoringService : Service, ISensorEventListener
    {
        private const string LOG_TAG = "RealTimeHR_MonitoringService";
        private const string NOTIFICATION_CHANNEL = "RealTimeHRMonitoringService";

        private int Interval => Preferences.Get(SettingConstants.MONITORING_INTERVAL, 10);

        private Timer measureTimer;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        private Vibrator vibrator;
        private WakeLock wakeLock;

        public static bool isRunning = false;

        public override void OnCreate()
        {
            base.OnCreate();

            NotificationChannel channel = new NotificationChannel(NOTIFICATION_CHANNEL, "Monitoring Service", NotificationImportance.Low);

            (ApplicationContext.GetSystemService(NotificationService) as NotificationManager).CreateNotificationChannel(channel);

            Notification notification = new Notification.Builder(this, NOTIFICATION_CHANNEL)
                .SetContentTitle("Real Time HR Service")
                .SetContentText("Monitoring Service")
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .Build();

            StartForeground(1512, notification);
        }

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
                measureTimer.Elapsed += MeasureHR;
                measureTimer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
                measureTimer.Stop();

                vibrator = ApplicationContext.GetSystemService(VibratorService) as Vibrator;
            }

            PowerManager powerManager = ApplicationContext.GetSystemService(PowerService) as PowerManager;
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "RealTimeHR::MonitoringServiceLockTag");

            wakeLock.Acquire();

            measureTimer.Start();

            isRunning = true;

            return StartCommandResult.Sticky;
        }

        private void MeasureHR(object sender, EventArgs e)
        {
            try
            {
                //vibrator.Vibrate(VibrationEffect.CreateOneShot(100, VibrationEffect.DefaultAmplitude));

                measureTimer?.Stop();

                sensorManager = ApplicationContext.GetSystemService(SensorService) as SensorManager;
                hrSensor = sensorManager?.GetDefaultSensor(SensorType.HeartRate);

                sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Normal);

                Log.Info(LOG_TAG, "Start Sensor");
            }
            catch (Exception ex)
            {
                RecordHelper.WriteText(ex.ToString());

                sensorManager?.UnregisterListener(this, hrSensor);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            measureTimer?.Stop();
            measureTimer?.Dispose();

            isRunning = false;

            wakeLock?.Release();

            StopForeground(StopForegroundFlags.Remove);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            sensorManager?.UnregisterListener(this, hrSensor);

            int hrData = (int)Math.Round(e.Values[0]);

            DateTime now = DateTime.Now;

            RecordHelper.WriteData($"{now:yyyy/MM/dd/ HH:mm:ss} {hrData}");

            Log.Info(LOG_TAG, "Stop Sensor");

            measureTimer.Interval = TimeSpan.FromMinutes(Interval).TotalMilliseconds;

            measureTimer?.Start();
        }
    }
}