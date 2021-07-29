using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using RealTimeHR.Helper;

using System;

namespace RealTimeHR
{
    public class GoogleAPITestFragment : AndroidX.Fragment.App.Fragment, ISensorEventListener
    {
        private const string LOG_TAG = "RealTimeHR_GoogleAPITest";

        private SensorManager sensorManager;
        private Sensor hrSensor;

        private Button testButton;
        private TextView statusTextView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.GoogleFitAPITestLayout, container);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            testButton = view.FindViewById<Button>(Resource.Id.GoogleAPITestButton);
            statusTextView = view.FindViewById<TextView>(Resource.Id.APIStatusTextView);

            testButton.Click += delegate { MeasureHR(Context); };
        }

        private void MeasureHR(Context context)
        {
            try
            {
                sensorManager = context.GetSystemService(Context.SensorService) as SensorManager;
                hrSensor = sensorManager?.GetDefaultSensor(SensorType.HeartRate);

                sensorManager?.RegisterListener(this, hrSensor, SensorDelay.Normal);

                statusTextView.Text = "Measuring...";

                Log.Info(LOG_TAG, "Start Sensor");
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.ToString());

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

                Log.Info(LOG_TAG, "Stop Sensor");

                statusTextView.Text = "Measured";

                if (GoogleFitHelper.Instance.HasPermissions)
                {
                    statusTextView.Text = "Inserting...";

                    GoogleFitHelper.Instance.InsertHeartRateData(hrData);
                }
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.ToString());
            }
        }
    }
}