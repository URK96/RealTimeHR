using Android.App;
using Android.OS;
using Android.Widget;

using RealTimeHR.Helper;

using System;

using Xamarin.Essentials;

namespace RealTimeHR
{
    [Activity(Label = "MonitoringSettingActivity")]
    public class MonitoringSettingActivity : Activity
    {
        const int DEFAULT_INTERVAL = 10;

        private NumberPicker intervalSelector;
        private Button applyButton;

        private readonly string[] intervalList =
        {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "90",
            "120"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MonitoringTimeSettingLayout);

            intervalSelector = FindViewById<NumberPicker>(Resource.Id.MonitoringIntervalSelector);
            applyButton = FindViewById<Button>(Resource.Id.MonitoringIntervalApplyButton);

            InitControl();
        }

        protected override void OnResume()
        {
            base.OnResume();

            InitIntervalValue();
        }

        private void InitControl()
        {
            intervalSelector.SetDisplayedValues(intervalList);

            intervalSelector.MinValue = 0;
            intervalSelector.MaxValue = intervalList.Length - 1;

            applyButton.Click += delegate
            {
                int value = int.Parse(intervalList[intervalSelector.Value]);

                Preferences.Set(SettingConstants.MONITORING_INTERVAL, value);

                if (MonitoringService.IsAlive())
                {
                    MonitoringHelper.Instance.UpdateMonitoringInterval(value);
                }

                Finish();
            };
        }

        private void InitIntervalValue()
        {
            int interval = Preferences.Get(SettingConstants.MONITORING_INTERVAL, DEFAULT_INTERVAL);

            intervalSelector.Value = Array.FindIndex(intervalList, x => x.Equals(interval.ToString()));
        }
    }
}