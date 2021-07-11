using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            if (!File.Exists(recordFile))
            {
                File.CreateText(recordFile);
            }
        }

        internal static void WriteData(string data)
        {
            CheckFile();

            try
            {
                File.AppendAllLines(recordFile, new string[] { data });
            }
            catch (Exception ex)
            {
                Log.Error("RealTimeHR", ex.ToString());
            }
        }
    }
}