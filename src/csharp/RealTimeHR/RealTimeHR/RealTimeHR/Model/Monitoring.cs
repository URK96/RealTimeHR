using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using AndroidX.Work;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeHR.Model
{
    public static class Monitoring
    {
        public const string WORKER_TAG = "MonitoringMeasureTag";

        public static WorkRequest CreateWorkRequest(TimeSpan interval)
        {
            var request = new OneTimeWorkRequest.Builder(typeof(MonitoringWork));

            request.AddTag(WORKER_TAG)
                .SetInitialDelay(interval);

            return request.Build();
        }
    }
}