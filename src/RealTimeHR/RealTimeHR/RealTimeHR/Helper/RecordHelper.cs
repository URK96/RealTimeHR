using Android.Util;

using System;
using System.IO;

namespace RealTimeHR.Helper
{
    internal static class RecordHelper
    {
        private static string recordPath = Path.Combine(Android.OS.Environment.StorageDirectory.AbsolutePath, "self", "primary", "Record");
        private static string recordFile = Path.Combine(recordPath, "HRDatas.txt");
        
        private static void CheckFile()
        {
            if (!Directory.Exists(recordPath))
            {
                Directory.CreateDirectory(recordPath);
            }
        }

        internal static void WriteData(string data)
        {
            CheckFile();

            try
            {
                //using StreamWriter writer = new StreamWriter(new FileStream(recordFile, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite))

                File.AppendAllLines(recordFile, new string[] { data });
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHRService", ex.ToString());
            }
        }

        internal static void WriteText(string data)
        {
            CheckFile();

            try
            {
                File.WriteAllText(Path.Combine(recordPath, $"{DateTime.Now:yyyyMMddHHmmss}.txt"), data);
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHR", ex.ToString());
            }
        }
    }
}