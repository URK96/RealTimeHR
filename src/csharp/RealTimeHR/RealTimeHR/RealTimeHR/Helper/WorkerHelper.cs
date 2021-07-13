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

namespace RealTimeHR.Helper
{
    internal class WorkerHelper
    {
        private Context WorkerContext => Application.Context;
        private WorkManager WorkManagerInstance => WorkManager.GetInstance(WorkerContext);

        public static WorkerHelper Instance { get { return instance.Value; } }

        private static readonly Lazy<WorkerHelper> instance = new Lazy<WorkerHelper>(() => new WorkerHelper());

        public void EnqueueWork(WorkRequest request)
        {
            WorkManagerInstance.Enqueue(request);
        }

        public void CancelRequest(string tag)
        {
            WorkManagerInstance.CancelAllWorkByTag(tag);
        }
    }
}