using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Transitions;
using Android.Util;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeHR
{
    public class BackgroundSettingFragment : AndroidX.Fragment.App.Fragment
    {
        private LinearLayout rootLayout;
        private Switch monitoringSwitch;
        private LinearLayout intervalSettingLayout;
        private NumberPicker intervalNP;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.BackgroundSettingLayout, null);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            rootLayout = view.FindViewById<LinearLayout>(Resource.Id.MonitoringRootLayout);
            monitoringSwitch = view.FindViewById<Switch>(Resource.Id.MonitoringSwitch);
            intervalSettingLayout = view.FindViewById<LinearLayout>(Resource.Id.IntervalSettingLayout);
            intervalNP = view.FindViewById<NumberPicker>(Resource.Id.IntervalNumberPicker);

            InitControl();
        }

        private void InitControl()
        {
            //TransitionManager.BeginDelayedTransition(rootLayout);
            
            monitoringSwitch.CheckedChange += MonitoringSwitch_CheckedChange;
            monitoringSwitch.Checked = false;

            intervalNP.MinValue = 1;
            intervalNP.MaxValue = 10;
        }

        private void MonitoringSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Intent serviceIntent = new Intent(Activity, typeof(MonitoringService));

            if (e.IsChecked)
            {
                intervalSettingLayout.Visibility = ViewStates.Visible;

                Activity.StartService(serviceIntent);
            }
            else
            {
                intervalSettingLayout.Visibility = ViewStates.Gone;

                Activity.StopService(serviceIntent);
            }
        }
    }
}