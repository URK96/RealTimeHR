﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace RealTimeHR
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/SplashTheme")]
    public class SplashActivity : AndroidX.Fragment.App.FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestPermissions(new string[] { "android.permission.BODY_SENSORS", "android.permission.READ_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE", "android.permission.VIBRATE" }, 0);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            bool isPass = true;

            foreach (Permission result in grantResults)
            {
                if (result == Permission.Denied)
                {
                    isPass = false;
                }
            }

            if (isPass)
            {
                Intent intent = new Intent(this, typeof(MainViewPagerActivity));

                StartActivity(typeof(MainViewPagerActivity));
            }

            Finish();
        }
    }
}