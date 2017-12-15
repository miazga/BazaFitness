using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace bazafitness.DAL
{
    public class ProductAdapter : BaseAdapter<Products>
    {
        Activity activity;
        int layoutResourceId;
        List<Products> items = new List<Products>();

        public ProductAdapter(Activity activity, int layoutResourceId)
        {
            this.activity = activity;
            this.layoutResourceId = layoutResourceId;
        }

        //Returns the view for a specific item on the list
        public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var row = convertView;
            var currentItem = this[position];
            CheckBox checkBox;

            if (row == null)
            {
                var inflater = activity.LayoutInflater;
                row = inflater.Inflate(layoutResourceId, parent, false);

                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkProducts);

                checkBox.CheckedChange += async (sender, e) => {
                    var cbSender = sender as CheckBox;
                    if (cbSender != null && cbSender.Tag is ProductWrapper && cbSender.Checked)
                    {
                        cbSender.Enabled = false;
                        if (activity is MainActivity)
                            await ((MainActivity)activity).CheckItem((cbSender.Tag as ProductWrapper).Products);
                    }
                };
            }
            else
                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkProducts);

            checkBox.Text = currentItem.Name;
            checkBox.Checked = false;
            checkBox.Enabled = true;
            checkBox.Tag = new ProductWrapper(currentItem);

            return row;
        }

        public void Add(Products item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            items.Clear();
            NotifyDataSetChanged();
        }

        public void Remove(Products item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }

        #region implemented abstract members of BaseAdapter

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override Products this[int position]
        {
            get
            {
                return items[position];
            }
        }

        #endregion
    }
}
