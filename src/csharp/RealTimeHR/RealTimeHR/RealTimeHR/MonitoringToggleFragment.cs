using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using RealTimeHR.Helper;

using System;

namespace RealTimeHR
{
    public class MonitoringToggleFragment : AndroidX.Fragment.App.Fragment
    {
        private LinearLayout rootLayout;
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

            rootLayout = view.FindViewById<LinearLayout>(Resource.Id.MonitoringRootLayout);
            monitoringSwitch = view.FindViewById<Switch>(Resource.Id.MonitoringSwitch);
            settingButton = view.FindViewById<Button>(Resource.Id.MonitoringTimeSettingButton);

            InitControl();
        }

        private void InitControl()
        {
            monitoringSwitch.CheckedChange += MonitoringSwitch_CheckedChange;
            monitoringSwitch.Checked = MonitoringService.isRunning;

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
                    //Activity.StartService(serviceIntent);

                    //WorkerHelper.Instance.EnqueueWork(Monitoring.CreateWorkRequest(TimeSpan.FromSeconds(10)));
                }
                else
                {
                    //WorkManager.GetInstance(Context).CancelAllWorkByTag("MonitoringMeasureTag");

                    Activity.StopService(serviceIntent);
                }
            }
            catch (Exception ex)
            {
                RecordHelper.WriteText(ex.ToString());
            }
        }
    }
}