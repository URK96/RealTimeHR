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

namespace RealTimeHR
{
    public class GoogleFitAccessFragment : AndroidX.Fragment.App.Fragment, IActivityResultCallback
    {
        private Button loginButton;
        private TextView loginResultText;

        private ActivityResultLauncher launcher;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            launcher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
        }

        public async void OnActivityResult(Java.Lang.Object obj)
        {
            ActivityResult result = obj as ActivityResult;

            if (result.ResultCode == (int)Result.Ok)
            {
                if (loginResultText != null)
                {
                    loginResultText.Text = "Login Success";

                    GoogleSignInAccount account = await GoogleSignIn.GetSignedInAccountFromIntentAsync(result.Data);

                    GrantPermission(account);
                }
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            //base.OnActivityResult(requestCode, resultCode, data);

            Log.Debug("RealTimeHR_GoogleSignIn", $"Request Code : {requestCode}");
            Log.Debug("RealTimeHR_GoogleSignIn", $"Result Code : {resultCode}");

            if (requestCode == 1)
            {

            }
            else if (requestCode == 2)
            {
                if (resultCode == (int)Result.Ok)
                {
                    //GrantPermission();
                }
            }

            // resultCode 0 is cancel, -1 is success
            // requestCode 1 is fix
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

        public override void OnResume()
        {
            base.OnResume();

            if (GoogleSignIn.GetLastSignedInAccount(Context) != null)
            {
                loginResultText.Text = "Already Login";
            }
        }

        private void InitControl()
        {
            loginButton.Click += delegate { LoginGoogle(); };
        }

        private void LoginGoogle()
        {
            GoogleSignInOptions options = new GoogleSignInOptions.Builder()
                .RequestEmail()
                .RequestId()
                .RequestProfile()
                .Build();
            GoogleSignInClient client = GoogleSignIn.GetClient(Activity, options);

            //Activity.StartActivityForResult(client.SignInIntent, 2);

            launcher.Launch(client.SignInIntent);
        }

        private void GrantPermission(GoogleSignInAccount account)
        {
            GoogleFitHelper helper = GoogleFitHelper.Instance;

            if (!helper.HasPermissions)
            {
                GoogleSignIn.RequestPermissions(this, 1, account, helper.Options);
            }
            else
            {
                loginResultText.Text = "Already Login";
            }
        }
    }
}