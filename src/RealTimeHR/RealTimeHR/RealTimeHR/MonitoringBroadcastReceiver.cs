using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;

using RealTimeHR.Helper;

using System;

using static Android.OS.PowerManager;

namespace RealTimeHR
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new string[] { MONITORING_INTENT })]
    public class MonitoringBroadcastReceiver : BroadcastReceiver, ISensorEventListener
    {
        private const string LOG_TAG = "RealTimeHR_MonitoringService";
        public const string MONITORING_INTENT = "com.urk.realtimehr.EXECUTE_MONITORING";

        private WakeLock wakeLock;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager powerManager = context.GetSystemService(Context.PowerService) as PowerManager;
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "RealTimeHR::MonitoringServiceLockTag");

            wakeLock.Acquire();

            MeasureHR(context);
        }

        private void MeasureHR(Context context)
        {
            try
            {
                sensorManager = context.GetSystemService(Context.SensorService) as SensorManager;
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

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            try
            {
                sensorManager?.UnregisterListener(this, hrSensor);

                int hrData = (int)Math.Round(e.Values[0]);

                DateTime now = DateTime.Now;

                RecordHelper.WriteData($"{now:yyyy/MM/dd/ HH:mm:ss} {hrData}");

                Log.Info(LOG_TAG, "Stop Sensor");

                if (GoogleFitHelper.Instance.HasPermissions)
                {
                    GoogleFitHelper.Instance.InsertHeartRateData(hrData);
                }

                wakeLock?.Release();
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.ToString());
            }
        }
    }
}