using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.RecyclerView.Widget;
using AndroidX.Wear.Widget;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RealTimeHR
{
    [Activity(Label = "HRRecordListActivity")]
    public class HRRecordListActivity : Activity
    {
        private WearableRecyclerView recyclerView;

        private RecordListAdapter adapter;

        private List<HeartRateData> datas = new List<HeartRateData>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HRRecordListLayout);

            recyclerView = FindViewById<WearableRecyclerView>(Resource.Id.HRRecordRecyclerView);

            InitControl();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await LoadDatas();
        }

        private void InitControl()
        {
            adapter = new RecordListAdapter(datas);

            recyclerView.CircularScrollingGestureEnabled = true;
            recyclerView.SetLayoutManager(new WearableLinearLayoutManager(this));
            recyclerView.SetAdapter(adapter);
        }

        private async Task LoadDatas()
        {
            datas.Clear();

            string recordPath = Path.Combine(Android.OS.Environment.StorageDirectory.AbsolutePath, "self", "primary", "Record");
            string recordFile = Path.Combine(recordPath, "HRDatas.txt");

            if (File.Exists(recordFile))
            {
                using StreamReader reader = new StreamReader(recordFile);

                string[] list = (await reader.ReadToEndAsync()).Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                foreach (string data in list)
                {
                    string[] sData = data.Split(" ");

                    if (sData.Length == 3)
                    {
                        datas.Add(new HeartRateData()
                        {
                            DateTimeData = $"{sData[0]}{sData[1]}",
                            HeartRateDate = sData[2]
                        });
                    }
                    else
                    {
                        datas.Add(new HeartRateData()
                        {
                            DateTimeData = sData[0],
                            HeartRateDate = sData[1]
                        });
                    }
                }
            }

            adapter?.NotifyDataSetChanged();
        }


        #region INNER_CLASS

        public class RecordListViewHolder : RecyclerView.ViewHolder
        {
            public TextView DateTimeText { get; private set; }
            public TextView HeartRateText { get; private set; }

            public RecordListViewHolder(View view) : base(view)
            {
                DateTimeText = view.FindViewById<TextView>(Resource.Id.HRRecordDateTimeTextView);
                HeartRateText = view.FindViewById<TextView>(Resource.Id.HRRecordDataTextView);
            }
        }

        public class ListHeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView HeaderText { get; private set; }

            public ListHeaderViewHolder(View view) : base(view)
            {
                HeaderText = view.FindViewById<TextView>(Resource.Id.ListHeaderTextView);
            }
        }

        public class RecordListAdapter : RecyclerView.Adapter
        {
            enum ViewType { Header = 0, Item }

            private List<HeartRateData> list;

            public override int ItemCount => list.Count + 1;

            public RecordListAdapter(List<HeartRateData> list)
            {
                this.list = list;
            }

            public override int GetItemViewType(int position)
            {
                ViewType type = position switch
                {
                    0 => ViewType.Header,
                    _ => ViewType.Item
                };

                return (int)type;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (position == 0)
                {
                    ListHeaderViewHolder vh = holder as ListHeaderViewHolder;

                    vh.HeaderText.Text = Application.Context.Resources.GetString(Resource.String.recordlist_title);
                }
                else
                {
                    RecordListViewHolder vh = holder as RecordListViewHolder;
                    HeartRateData item = list[position - 1];

                    vh.DateTimeText.Text = item.DateTimeData;
                    vh.HeartRateText.Text = item.HeartRateDate;
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view;
                RecyclerView.ViewHolder vh;

                switch ((ViewType)viewType)
                {
                    case ViewType.Header:
                        view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListHeaderLayout, parent, false);
                        vh = new ListHeaderViewHolder(view);
                        break;
                    default:
                        view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.HRRecordItemLayout, parent, false);
                        vh = new RecordListViewHolder(view);
                        break;
                }

                return vh;
            }
        }

        public class HeartRateData
        {
            public string DateTimeData;
            public string HeartRateDate;
        }

        #endregion
    }
}