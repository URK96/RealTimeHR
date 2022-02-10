using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Tasks;
using Android.Util;

using System;

namespace RealTimeHR.Helper
{
    public class GoogleSignHelper
    {
        private const string LOG_TAG = "RealTimeHR_GoogleSignHelper";

        private Context AppContext => Application.Context;

        public static GoogleSignHelper Instance => instance.Value;
        public GoogleSignInOptions SignInOptions { get; private set; }
        public GoogleSignInClient SignInClient { get; private set; }

        private static readonly Lazy<GoogleSignHelper> instance = new Lazy<GoogleSignHelper>(() => new GoogleSignHelper());

        private GoogleSignHelper()
        {
            SignInOptions = new GoogleSignInOptions.Builder()
                .RequestEmail()
                .RequestId()
                .RequestProfile()
                .Build();
            SignInClient = GoogleSignIn.GetClient(AppContext, SignInOptions);
        }


        #region Listener Part

        public class GoogleSignOutListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(LOG_TAG, "Fail to sign out");
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                Log.Info(LOG_TAG, "Success to sign out");
            }
        }

        #endregion
    }
}