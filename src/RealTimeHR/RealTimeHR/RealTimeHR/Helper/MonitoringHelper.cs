using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

using System;

namespace RealTimeHR.Helper
{
    public class MonitoringHelper
    {
        private Context AppContext => Application.Context;
        private AlarmManager AppAlarmManager => AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
        public static MonitoringHelper Instance => instance.Value;

        private static readonly Lazy<MonitoringHelper> instance = new Lazy<MonitoringHelper>(() => new MonitoringHelper());

        private MonitoringHelper() { }

        public PendingIntent CreatePendingIntent()
        {
            Intent alarmIntent = new Intent(AppContext, typeof(MonitoringBroadcastReceiver));
            alarmIntent.SetAction(MonitoringBroadcastReceiver.MONITORING_INTENT);

            return PendingIntent.GetBroadcast(AppContext, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
        }

        public void RegisterMonitoring(int interval)
        {
            AppAlarmManager.SetRepeating(AlarmType.ElapsedRealtimeWakeup, 
                SystemClock.ElapsedRealtime() + 10 * 1000, 
                (long)TimeSpan.FromMinutes(interval).TotalMilliseconds, 
                CreatePendingIntent());
        }

        public void UnregisterMonitoring()
        {
            AppAlarmManager.Cancel(CreatePendingIntent());
        }

        public void UpdateMonitoringInterval(int interval)
        {
            UnregisterMonitoring();
            RegisterMonitoring(interval);

            Log.Info("RealTimeHR_MonitoringHelper", "Change monitoring interval success");
        }
    }
}