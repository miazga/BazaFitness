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
    class RecipeAdapter : BaseAdapter<Recipe>
    {
        Activity activity;
        int layoutResourceId;
        List<Recipe> items = new List<Recipe>();

        public RecipeAdapter(Activity activity, int layoutResourceId)
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
            TextView icon;
            TextView name;
            TextView content;

            if (row == null)
            {
                var inflater = activity.LayoutInflater;
                row = inflater.Inflate(layoutResourceId, parent, false);

                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkRecipe);
                icon = row.FindViewById<TextView>(Resource.Id.txtIcon);
                name = row.FindViewById<TextView>(Resource.Id.txtName);
                content = row.FindViewById<TextView>(Resource.Id.txtContent);
                checkBox.CheckedChange += async (sender, e) =>
                {
                    var cbSender = sender as CheckBox;
                    if (cbSender != null && cbSender.Tag is RecipeWrapper && cbSender.Checked)
                    {
                        cbSender.Enabled = false;
                        if (activity is RecipeActivity)
                        {
                            await ((RecipeActivity) activity).CheckItem((cbSender.Tag as RecipeWrapper).Recipe);
                        }

                    }
                };
            }
            else
            {
                icon = row.FindViewById<TextView>(Resource.Id.txtIcon);
                name = row.FindViewById<TextView>(Resource.Id.txtName);
                content = row.FindViewById<TextView>(Resource.Id.txtContent);
                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkRecipe);
            }
                

            //checkBox.Text = currentItem.Name;
            checkBox.Checked = false;
            checkBox.Enabled = true;
            checkBox.Tag = new RecipeWrapper(currentItem);
            icon.Text = currentItem.Icon;
            name.Text = currentItem.Name;
            content.Text = currentItem.Content;

            return row;
        }

        public void Add(Recipe item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            items.Clear();
            NotifyDataSetChanged();
        }

        public void Remove(Recipe item)
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

        public override Recipe this[int position]
        {
            get
            {
                return items[position];
            }
        }

        #endregion
    }
}