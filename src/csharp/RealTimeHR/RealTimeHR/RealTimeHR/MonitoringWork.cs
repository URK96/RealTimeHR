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
using AndroidX.Work;
using RealTimeHR.Helper;
using Android.Util;
using RealTimeHR.Model;
using static Android.OS.PowerManager;
using Android.Hardware;

namespace RealTimeHR
{
    public class MonitoringWork : Worker, ISensorEventListener
    {
        private HeartRateHelper HRHelper => HeartRateHelper.Instance;
        private Context context;

        private WakeLock wakeLock;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        private bool isStop = false;

        public MonitoringWork(Context context, WorkerParameters parameters) : base(context, parameters)
        {
            this.context = context;
        }

        public override Result DoWork()
        {
            try
            {
                //Vibrator vibrator = ApplicationContext.GetSystemService(Context.VibratorService) as Vibrator;

                //vibrator.Vibrate(VibrationEffect.CreateOneShot(300, VibrationEffect.DefaultAmplitude));

                //HRHelper.HRDataChanged += FinalizeMeasure;
                //HRHelper.HRDataChangedOnce += FinalizeMeasure;

                //HRHelper.StartSensor();

                PowerManager powerManager = ApplicationContext.GetSystemService(Context.PowerService) as PowerManager;
                wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "RealTimeHR::MonitoringServiceLockTag");

                wakeLock.Acquire();

                sensorManager = ApplicationContext.GetSystemService(Context.SensorService) as SensorManager;
                hrSensor = sensorManager?.GetDefaultSensor(SensorType.Proximity);

                sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Normal);

                Log.Info("RealTimeHRService", "Start Sensor");
            }
            catch
            {
                return new Result.Failure();  
            }

            return new Result.Success();
        }

        private void FinalizeMeasure(object sender, int data)
        {
            try
            {
                //HRHelper.HRDataChanged -= FinalizeMeasure;
                HRHelper.StopSensor();
                
                DateTime now = DateTime.Now;

                RecordHelper.WriteData($"{now:yyyyMMddHHmmss} {data}");

                Log.Info("RealTimeHRService", "Stop Sensor");
            }
            catch (Exception ex)
            {
                RecordHelper.WriteText(ex.ToString());
            }
            finally
            {
                if (!isStop)
                {
                    WorkerHelper.Instance.EnqueueWork(Monitoring.CreateWorkRequest(TimeSpan.FromSeconds(10)));
                }
            }
        }

        public override void OnStopped()
        {
            base.OnStopped();

            Log.Info("RealTimeHRService", "Receive Stop Signal");

            isStop = true;
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

            wakeLock?.Release();
            wakeLock?.Dispose();

            if (!isStop)
            {
                WorkerHelper.Instance.EnqueueWork(Monitoring.CreateWorkRequest(TimeSpan.FromSeconds(10)));
            }
        }
    }
}