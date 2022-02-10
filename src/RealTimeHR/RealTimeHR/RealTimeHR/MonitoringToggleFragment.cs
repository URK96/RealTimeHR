using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidX.Wear.Activity;

using System;

namespace RealTimeHR
{
    public class MonitoringToggleFragment : AndroidX.Fragment.App.Fragment
    {
        private Switch monitoringSwitch;
        private Button settingButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MonitoringToggleLayout, null);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            monitoringSwitch = view.FindViewById<Switch>(Resource.Id.MonitoringSwitch);
            settingButton = view.FindViewById<Button>(Resource.Id.MonitoringTimeSettingButton);

            InitControl();
        }

        private void InitControl()
        {
            monitoringSwitch.CheckedChange += MonitoringSwitch_CheckedChange;
            monitoringSwitch.Checked = MonitoringService.IsAlive();

            settingButton.Click += delegate { Activity.StartActivity(typeof(MonitoringSettingActivity)); };
        }

        private void MonitoringSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        { 
            Intent serviceIntent = new Intent(Activity, typeof(MonitoringService));

            try
            {
                if (e.IsChecked)
                {
                    Activity.StartForegroundService(serviceIntent);
                }
                else
                {
                    Activity.StopService(serviceIntent);

                    Intent intent = new Intent(Context, typeof(ConfirmationActivity))
                        .SetFlags(ActivityFlags.NewTask)
                        .PutExtra(ConfirmationActivity.ExtraAnimationType, ConfirmationActivity.SuccessAnimation)
                        .PutExtra(ConfirmationActivity.ExtraMessage, Resources.GetString(Resource.String.monitoring_service_stop_confirmation));

                    StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHR_MonitoringToggle", ex.ToString());
            }
        }
    }
}