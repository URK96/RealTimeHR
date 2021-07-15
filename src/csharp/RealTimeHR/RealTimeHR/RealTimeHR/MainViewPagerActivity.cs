using Android.App;
using Android.OS;

using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using AndroidX.Wear.Ambient;

using RealTimeHR.Helper;

using System.Collections.Generic;

using Fragment = AndroidX.Fragment.App.Fragment;

namespace RealTimeHR
{
    [Activity(Label = "MainViewPagerActivity")]
    public class MainViewPagerActivity : FragmentActivity, AmbientModeSupport.IAmbientCallbackProvider
    {
        private ViewPager2 viewPager;

        public AmbientModeSupport.AmbientCallback AmbientCallback => new MainAmbientCallBack();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainViewPagerLayout);

            AmbientModeSupport.Attach(this);

            viewPager = FindViewById<ViewPager2>(Resource.Id.MainViewPager);

            InitAdapter();
        }

        private void InitAdapter()
        {
            var adapter = new MainPagerFragmentStateAdapter(this);
            adapter.AddFragment(new RealTimeMeasureFragment());

            if (HeartRateHelper.Instance.IsSensorExist)
            {
                adapter.AddFragment(new MonitoringToggleFragment());
                adapter.AddFragment(new HRRecordListFragment());
            }

            viewPager.Adapter = adapter;
        }

        private class MainAmbientCallBack : AmbientModeSupport.AmbientCallback
        {
            public override void OnEnterAmbient(Bundle ambientDetails)
            {
                base.OnEnterAmbient(ambientDetails);
            }

            public override void OnExitAmbient()
            {
                base.OnExitAmbient();
            }

            public override void OnUpdateAmbient()
            {
                base.OnUpdateAmbient();
            }
        }
    }

    class MainPagerFragmentStateAdapter : FragmentStateAdapter
    {
        private readonly List<Fragment> fragments;

        public override int ItemCount => fragments.Count;

        public MainPagerFragmentStateAdapter(FragmentActivity activity) : base(activity)
        {
            fragments = new List<Fragment>();
        }

        public void AddFragment(Fragment fragment)
        {
            fragments.Add(fragment);
        }

        public override Fragment CreateFragment(int position)
        {
            return fragments[position];
        }
    }
}