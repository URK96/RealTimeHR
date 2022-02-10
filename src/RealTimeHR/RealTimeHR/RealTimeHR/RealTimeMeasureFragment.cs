using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Com.Airbnb.Lottie;

using RealTimeHR.Helper;

namespace RealTimeHR
{
    [Activity(Label = "MainMornitoringFragment")]
    public class RealTimeMeasureFragment : AndroidX.Fragment.App.Fragment
    {
        private const float FACTOR = 0.146467f;

        private HeartRateHelper HRService => HeartRateHelper.Instance;

        private LinearLayout rootLayout;
        private LottieAnimationView lottieAnimationView;
        private TextView hrMonitoringDataTextView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.RealTimeMeasureLayout, null);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            rootLayout = view.FindViewById<LinearLayout>(Resource.Id.MainRootLayout);
            lottieAnimationView = view.FindViewById<LottieAnimationView>(Resource.Id.HeartbeatAnimationView);
            hrMonitoringDataTextView = view.FindViewById<TextView>(Resource.Id.RealTimeHRData);
        }

        private void RegisterHRSensor()
        {
            if (HRService.IsSensorExist)
            {
                HRService.StartSensor();
                HRService.HRDataChanged += UpdateHRData;

                lottieAnimationView.PlayAnimation();
            }
            else
            {
                hrMonitoringDataTextView.Text = Resources.GetString(Resource.String.text_sensor_notfound);
            }
        }

        private void UnregisterHRSensor()
        {
            HRService.StopSensor();
            HRService.HRDataChanged -= UpdateHRData;

            lottieAnimationView.PauseAnimation();
        }

        private void UpdateHRData(object sender, int data)
        {
            Activity.RunOnUiThread(() => { hrMonitoringDataTextView.Text = data.ToString(); });
        }

        public override void OnResume()
        {
            base.OnResume();

            Activity.RunOnUiThread(() => { hrMonitoringDataTextView.Text = Resources.GetString(Resource.String.realtime_ready); });

            RegisterHRSensor();
        }

        public override void OnPause()
        {
            base.OnPause();

            UnregisterHRSensor();
        }
    }
}


