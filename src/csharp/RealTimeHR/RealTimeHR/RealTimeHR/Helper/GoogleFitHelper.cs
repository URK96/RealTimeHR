using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Tasks;

using System;
using System.Threading.Tasks;

using Log = Android.Util.Log;

namespace RealTimeHR.Helper
{
    public class GoogleFitHelper : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {
        private Context AppContext => Application.Context;

        public static GoogleFitHelper Instance => instance.Value;
        public FitnessOptions Options { get; private set; }
        public GoogleSignInAccount Account => GoogleSignIn.GetAccountForExtension(AppContext, Options);
        public bool HasPermissions => GoogleSignIn.HasPermissions(Account, Options);

        private static readonly Lazy<GoogleFitHelper> instance = new Lazy<GoogleFitHelper>(() => new GoogleFitHelper());

        private GoogleFitHelper()
        {
            Options = FitnessOptions.InvokeBuilder()
                .AddDataType(DataType.TypeHeartRateBpm, FitnessOptions.AccessWrite)
                .AddDataType(DataType.TypeHeartRateBpm, FitnessOptions.AccessRead)
                .Build();
        }

        public async Task<bool> InsertHeartRateDataAsync(int data)
        {
            try
            {
                await FitnessClass.GetHistoryClient(AppContext, Account)
                    .InsertDataAsync(CreateHeartRateData(data));
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHR_GoogleFitHelper", ex.ToString());

                return false;
            }

            Log.Info("RealTimeHR_GoogleFitHelper", "Success to request insert heart rate data");

            return true;
        }

        public bool InsertHeartRateData(int data)
        {
            try
            {
                var account = GoogleSignIn.GetAccountForExtension(AppContext, Options);

                FitnessClass.GetHistoryClient(AppContext, account)
                    .InsertData(CreateHeartRateData(data))
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHR_GoogleFitHelper", ex.ToString());

                return false;
            }

            Log.Info("RealTimeHR_GoogleFitHelper", "Success to request insert heart rate data");

            return true;
        }

        private DataSet CreateHeartRateData(int data)
        {
            DataSource source = new DataSource.Builder()
                .SetAppPackageName(AppContext)
                .SetDataType(DataType.TypeHeartRateBpm)
                .SetDevice(Device.GetLocalDevice(AppContext))
                .SetType(DataSource.TypeRaw)
                .Build();
            DataPoint point = DataPoint.InvokeBuilder(source)
                .SetTimestamp(Java.Lang.JavaSystem.CurrentTimeMillis(), Java.Util.Concurrent.TimeUnit.Milliseconds)
                .SetField(Field.FieldBpm, (float)data)
                .Build();
            DataSet set = DataSet.InvokeBuilder(source)
                .Add(point)
                .Build();

            return set;
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Log.Info("RealTimeHR_GoogleFitHelper", "insert success");
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Log.Info("RealTimeHR_GoogleFitHelper", "insert fail");
            Log.Error("RealTimeHR_GoogleFitHelper", e.ToString());
        }
    }
}