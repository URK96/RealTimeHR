using Android.App;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Widget;

using Com.Airbnb.Lottie;

using System;

namespace RealTimeHR
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity, ISensorEventListener
    {
        private const float FACTOR = 0.146467f;

        private LinearLayout rootLayout;
        private LottieAnimationView lottieAnimationView;
        private TextView hrMonitoringDataTextView;

        private SensorManager sensorManager;
        private Sensor hrSensor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainMonitorLayout);

            rootLayout = FindViewById<LinearLayout>(Resource.Id.MainRootLayout);
            lottieAnimationView = FindViewById<LottieAnimationView>(Resource.Id.HeartbeatAnimationView);
            hrMonitoringDataTextView = FindViewById<TextView>(Resource.Id.RealTimeHRData);

            SetAmbientEnabled();

            RequestPermissions(new string[] { "android.permission.BODY_SENSORS" }, 0);

            AdjustInset();

            sensorManager = GetSystemService(SensorService) as SensorManager;
            hrSensor = sensorManager.GetDefaultSensor(SensorType.HeartRate);
        }

        private void AdjustInset()
        {
            if (ApplicationContext.Resources.Configuration.IsScreenRound)
            {
                var inset = Convert.ToInt32(FACTOR * Resources.DisplayMetrics.WidthPixels);

                rootLayout.SetPadding(inset, inset, inset, inset);
            }
        }

        private void RegisterHRSensor()
        {
            if (hrSensor != null)
            {
                sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Fastest);
                lottieAnimationView.PlayAnimation();
            }
            else
            {
                hrMonitoringDataTextView.Text = "No HR Sensor :(";
            }
        }

        private void UnregisterHRSensor()
        {
            sensorManager.UnregisterListener(this);
            lottieAnimationView.PauseAnimation();
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

            RunOnUiThread(() => { hrMonitoringDataTextView.Text = hrData.ToString(); });
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


