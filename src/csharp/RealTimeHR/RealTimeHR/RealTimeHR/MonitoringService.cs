using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;

using RealTimeHR.Helper;

using System;

using Xamarin.Essentials;

namespace RealTimeHR
{
    [Service(Name = "com.urk.realtimehr.monitoringservice", Process = ":monitoring_service", Exported = true)]
    public class MonitoringService : Service
    {
        private const string LOG_TAG = "RealTimeHR_MonitoringService";
        private const string NOTIFICATION_CHANNEL = "RealTimeHRMonitoringService";
        private const int NOTIFICATION_ID = 1512;

        private int Interval => Preferences.Get(SettingConstants.MONITORING_INTERVAL, 10);

        public static bool isRunning = false;

        public override void OnCreate()
        {
            base.OnCreate();

            NotificationChannel channel = new NotificationChannel(NOTIFICATION_CHANNEL, "Monitoring Service", NotificationImportance.Low);

            (ApplicationContext.GetSystemService(NotificationService) as NotificationManager).CreateNotificationChannel(channel);

            Notification notification = new Notification.Builder(this, NOTIFICATION_CHANNEL)
                .SetContentTitle("Real Time HR Service")
                .SetContentText("Monitoring Service")
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .Build();

            StartForeground(NOTIFICATION_ID, notification);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Intent alarmIntent = new Intent(ApplicationContext, typeof(MonitoringBroadcastReceiver));
            //alarmIntent.SetAction(MonitoringBroadcastReceiver.MONITORING_INTENT);
            //PendingIntent pendingIntent = PendingIntent.GetBroadcast(ApplicationContext, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

            //AlarmManager alarmManager = ApplicationContext.GetSystemService(AlarmService) as AlarmManager;

            //alarmManager.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 10 * 1000, (long)TimeSpan.FromMinutes(Interval).TotalMilliseconds, pendingIntent);

            MonitoringHelper.Instance.RegisterMonitoring(Interval);

            Preferences.Set(SettingConstants.MONITORING_SERVICE_RUNNING, true);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                Log.Info(LOG_TAG, "Destory Service");

                //Intent alarmIntent = new Intent(ApplicationContext, typeof(MonitoringBroadcastReceiver));
                //alarmIntent.SetAction(MonitoringBroadcastReceiver.MONITORING_INTENT);
                //PendingIntent pendingIntent = PendingIntent.GetBroadcast(ApplicationContext, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

                //AlarmManager alarmManager = ApplicationContext.GetSystemService(AlarmService) as AlarmManager;

                //alarmManager.Cancel(pendingIntent); 

                MonitoringHelper.Instance.UnregisterMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.ToString());
            }
            finally
            {
                StopForeground(StopForegroundFlags.Remove);
                StopSelf();

                Preferences.Set(SettingConstants.MONITORING_SERVICE_RUNNING, false);
            }
        }
    }
}