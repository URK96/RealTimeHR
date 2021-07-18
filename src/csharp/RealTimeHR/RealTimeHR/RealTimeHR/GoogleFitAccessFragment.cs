using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeHR
{
    public class GoogleFitAccessFragment : AndroidX.Fragment.App.Fragment, IActivityResultCallback
    {
        private Button loginButton;
        private TextView loginResultText;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActivityResultLauncher launcher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
        }

        public void OnActivityResult(Java.Lang.Object obj)
        {
            ActivityResult result = obj as ActivityResult;

            if (result.ResultCode == (int)Result.Ok)
            {
                if (loginResultText != null)
                {
                    loginResultText.Text = "Login Success";
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.GoogleFitAccessLayout, container);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            loginButton = view.FindViewById<Button>(Resource.Id.GoogleLoginButton);
            loginResultText = view.FindViewById<TextView>(Resource.Id.LoginResultTextView);

            InitControl();
        }

        private void InitControl()
        {
            loginButton.Click += LoginButton_Click;
        }

        private void LoginGoogle()
        {
            var fitnessOptions = FitnessOptions.InvokeBuilder()
                .AddDataType(Android.Gms.Fitness.Data.DataType.TypeHeartRateBpm, FitnessOptions.AccessWrite)
                .Build();

            var account = GoogleSignIn.GetAccountForExtension(Context, fitnessOptions);

            if (!GoogleSignIn.HasPermissions(account, fitnessOptions))
            {
                GoogleSignIn.RequestPermissions(this, 1, account, fitnessOptions);
            }
            else
            {
                loginResultText.Text = "Already Login";
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginGoogle();
        }
    }
}