using Android.App;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Widget;

using System;

namespace RealTimeHR
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity, ISensorEventListener
    {
        private TextView textView;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainMonitorLayout);

            textView = FindViewById<TextView>(Resource.Id.RealTimeHRData);

            SetAmbientEnabled();

            RequestPermissions(new string[] { "android.permission.BODY_SENSORS" }, 0);

            sensorManager = GetSystemService(SensorService) as SensorManager;
            hrSensor = sensorManager.GetDefaultSensor(SensorType.HeartRate);
            var list = sensorManager.GetSensorList(SensorType.HeartRate);
        }

        private void RegisterHRSensor()
        {
            sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Fastest);
        }

        private void UnregisterHRSensor()
        {
            sensorManager.UnregisterListener(this);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public override void OnEnterAmbient(Bundle ambientDetails)
        {
            base.OnEnterAmbient(ambientDetails);

            UnregisterHRSensor();
        }

        public override void OnExitAmbient()
        {
            base.OnExitAmbient();

            RegisterHRSensor();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            int hrData = (int)Math.Round(e.Values[0]);

            RunOnUiThread(() => { textView.Text = hrData.ToString(); });
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterHRSensor();
        }

        protected override void OnResume()
        {
            base.OnResume();

            RegisterHRSensor();
        }
    }
}


