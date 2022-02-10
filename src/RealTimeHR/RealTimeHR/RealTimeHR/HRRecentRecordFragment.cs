using Android.OS;
using Android.Views;
using Android.Widget;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeHR
{
    public class HRRecentRecordFragment : AndroidX.Fragment.App.Fragment
    {
        private TextView recordTimeText;
        private TextView recordValueText;
        private Button recordListButton;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HRRecentRecordLayout, container);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            recordTimeText = view.FindViewById<TextView>(Resource.Id.HRRecentRecordTimeTextView);
            recordValueText = view.FindViewById<TextView>(Resource.Id.HRRecentRecordValueTextView);
            recordListButton = view.FindViewById<Button>(Resource.Id.HRRecordListButton);

            InitControl();
        }

        private void InitControl()
        {
            recordListButton.Click += delegate { Activity.StartActivity(typeof(HRRecordListActivity)); };
        }

        public override async void OnResume()
        {
            base.OnResume();

            await LoadValue();
        }

        private async Task LoadValue()
        {
            string recordPath = Path.Combine(Android.OS.Environment.StorageDirectory.AbsolutePath, "self", "primary", "Record");
            string recordFile = Path.Combine(recordPath, "HRDatas.txt");

            if (File.Exists(recordFile))
            {
                using StreamReader reader = new StreamReader(recordFile);

                string[] list = (await reader.ReadToEndAsync()).Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                string[] sData = list.Last().Split(" ");
                string dt;
                string value;

                if (sData.Length == 3)
                {
                    dt = $"{sData[0]}{sData[1]}";
                    value = sData[2];
                }
                else
                {
                    dt = sData[0];
                    value = sData[1];
                }

                recordValueText.Text = $"{value} bpm";
                recordTimeText.Text = $"{dt}";
            }
        }
    }
}