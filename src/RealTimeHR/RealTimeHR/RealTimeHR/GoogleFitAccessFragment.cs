using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;

using RealTimeHR.Helper;

using System.Text;

namespace RealTimeHR
{
    public class GoogleFitAccessFragment : AndroidX.Fragment.App.Fragment, IActivityResultCallback
    {
        private const string LOG_TAG = "RealTimeHR_GoogleFitAccess";
        private const int REQUEST_PERMISSION_CODE = 1;

        private Button loginButton;
        private Button logoutButton;
        private TextView accountInfoTitleText;
        private TextView accountInfoText;

        private ActivityResultLauncher launcher;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            launcher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
        }

        // For sign in google
        public async void OnActivityResult(Java.Lang.Object obj)
        {
            ActivityResult result = obj as ActivityResult;

            Log.Info(LOG_TAG, $"Google Login Result Code : {result.ResultCode}");

            if (result.ResultCode == (int)Result.Ok)
            {
                if (accountInfoText != null)
                {
                    GoogleSignInAccount account = await GoogleSignIn.GetSignedInAccountFromIntentAsync(result.Data);

                    GrantPermission(account);
                }
            }
        }

        // For request google cloud permissions
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            Log.Info(LOG_TAG, $"Request Permission Result Code : {resultCode}");

            if (requestCode == REQUEST_PERMISSION_CODE)
            {
                UpdateLoginStatus();
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
            logoutButton = view.FindViewById<Button>(Resource.Id.GoogleLogoutButton);
            accountInfoTitleText = view.FindViewById<TextView>(Resource.Id.AccountInfoTitleTextView);
            accountInfoText = view.FindViewById<TextView>(Resource.Id.AccountInfoTextView);

            InitControl();
        }

        public override void OnResume()
        {
            base.OnResume();

            UpdateLoginStatus();
        }

        private void UpdateLoginStatus()
        {
            GoogleSignInAccount account = GoogleSignIn.GetLastSignedInAccount(Context);

            if (account != null)
            {
                loginButton.Visibility = ViewStates.Gone;
                logoutButton.Visibility = ViewStates.Visible;
                accountInfoTitleText.Visibility = ViewStates.Visible;
                accountInfoText.Visibility = ViewStates.Visible;

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(account.DisplayName);
                sb.Append($"({account.Email})");

                accountInfoText.Text = sb.ToString();
            }
            else
            {
                loginButton.Visibility = ViewStates.Visible;
                logoutButton.Visibility = ViewStates.Gone;
                accountInfoTitleText.Visibility = ViewStates.Gone;
                accountInfoText.Visibility = ViewStates.Gone;
            }
        }

        private void InitControl()
        {
            loginButton.Click += delegate { LoginGoogle(); };
            logoutButton.Click += delegate { LogoutGoogle(); };
        }

        private void LoginGoogle()
        {
            //GoogleSignInOptions options = new GoogleSignInOptions.Builder()
            //    .RequestEmail()
            //    .RequestId()
            //    .RequestProfile()
            //    .Build();
            //GoogleSignInClient client = GoogleSignIn.GetClient(Activity, CreateSignInOptions());

            launcher.Launch(GoogleSignHelper.Instance.SignInClient.SignInIntent);
        }

        private void LogoutGoogle()
        {
            var listener = new GoogleSignHelper.GoogleSignOutListener();

            GoogleSignHelper.Instance.SignInClient
                .SignOut()
                .AddOnSuccessListener(listener)
                .AddOnFailureListener(listener);

            UpdateLoginStatus();
        }

        private void GrantPermission(GoogleSignInAccount account)
        {
            GoogleFitHelper helper = GoogleFitHelper.Instance;

            if (!helper.HasPermissions)
            {
                GoogleSignIn.RequestPermissions(this, REQUEST_PERMISSION_CODE, account, helper.FitOptions);
            }
        }
    }
}