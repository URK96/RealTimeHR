using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Util;

using System;

namespace RealTimeHR.Helper
{
    internal class HeartRateHelper : Java.Lang.Object, ISensorEventListener
    {
        public event EventHandler<int> HRDataChanged;

        public static HeartRateHelper Instance { get { return instance.Value; } }

        public bool IsSensorExist => hrSensor != null;

        private static readonly Lazy<HeartRateHelper> instance = new Lazy<HeartRateHelper>(() => new HeartRateHelper());

        private Context Context => Application.Context;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        private HeartRateHelper()
        {
            LoadSensor();
        }

        public void LoadSensor()
        {
            sensorManager = Context.GetSystemService(Context.SensorService) as SensorManager;
            hrSensor = sensorManager?.GetDefaultSensor(SensorType.HeartRate);
        }

        public void StartSensor()
        {
            sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Fastest);
        }

        public void StopSensor()
        {
            sensorManager?.UnregisterListener(this, hrSensor);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
#if DEBUG
            Log.Info("RealTimeHR", accuracy.ToString());
#endif
        }

        public void OnSensorChanged(SensorEvent e)
        {
            int hrData = (int)Math.Round(e.Values[0]);

            HRDataChanged?.Invoke(this, hrData);
        }
    }
}