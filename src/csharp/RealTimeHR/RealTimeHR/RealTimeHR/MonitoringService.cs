using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;

using RealTimeHR.Helper;

using System;
using System.Timers;

using static Android.OS.PowerManager;

namespace RealTimeHR
{
    [Service(Name = "com.urk.realtimehr.monitoringservice", Process = ":monitoring_service", Exported = true)]
    public class MonitoringService : Service, ISensorEventListener
    {
        private HeartRateHelper HRHelper => HeartRateHelper.Instance;

        private Timer measureTimer;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        private Vibrator vibrator;
        private WakeLock wakeLock;

        public static bool isRunning = false;

        public override void OnCreate()
        {
            base.OnCreate();

            NotificationChannel channel = new NotificationChannel("RealTimeHRService", "Monitoring Service", NotificationImportance.Low);

            (ApplicationContext.GetSystemService(NotificationService) as NotificationManager).CreateNotificationChannel(channel);

            Notification notification = new Notification.Builder(this, "RealTimeHRService")
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
                measureTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
                measureTimer.Stop();

                vibrator = ApplicationContext.GetSystemService(VibratorService) as Vibrator;
            }

            PowerManager powerManager = ApplicationContext.GetSystemService(Context.PowerService) as PowerManager;
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "RealTimeHR::MonitoringServiceLockTag");

            wakeLock.Acquire();

            ChangeInterval(30);

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

                Log.Info("RealTimeHRService", "Start Sensor");
            }
            catch (Exception ex)
            {
                RecordHelper.WriteText(ex.ToString());
            }
        }

        private void FinalizeMeasure(object sender, int data)
        {
            try
            {
                HRHelper.StopSensor();
                HRHelper.HRDataChanged -= FinalizeMeasure;

                DateTime now = DateTime.Now;

                RecordHelper.WriteData($"{now:yyyyMMddHHmmss} {data}");

                measureTimer?.Start();

                Log.Info("RealTimeHRService", "Stop Sensor");
            }
            catch (Exception ex)
            {
                RecordHelper.WriteText(ex.ToString());
            }
        }

        public void ChangeInterval(int min)
        {
            //measureTimer.Change(TimeSpan.FromSeconds(min), TimeSpan.FromSeconds(min));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            measureTimer?.Stop();
            measureTimer?.Dispose();

            //wakeLock?.Release();

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

            RecordHelper.WriteData($"{now:yyyyMMddHHmmss} {hrData}");

            Log.Info("RealTimeHRService", "Stop Sensor");

            measureTimer?.Start();
        }
    }
}