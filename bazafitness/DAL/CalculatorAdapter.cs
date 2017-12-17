﻿using System;
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
    class CalculatorAdapter : BaseAdapter<Recipe>
    {
        Activity activity;
        int layoutResourceId;
        List<Recipe> items = new List<Recipe>();

        public CalculatorAdapter(Activity activity, int layoutResourceId)
        {
            this.activity = activity;
            this.layoutResourceId = layoutResourceId;
        }

        //Returns the view for a specific item on the list
        public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var row = convertView;
            var currentItem = this[position];
            //CheckBox checkBox;
            TextView icon;
            TextView name;
            TextView content;
            TextView meal;

            if (row == null)
            {
                var inflater = activity.LayoutInflater;
                row = inflater.Inflate(layoutResourceId, parent, false);
                
                icon = row.FindViewById<TextView>(Resource.Id.txtIcon);
                name = row.FindViewById<TextView>(Resource.Id.txtName);
                content = row.FindViewById<TextView>(Resource.Id.txtContent);
                meal = row.FindViewById<TextView>(Resource.Id.txtMeal);
            }
            else
            {
                icon = row.FindViewById<TextView>(Resource.Id.txtIcon);
                name = row.FindViewById<TextView>(Resource.Id.txtName);
                content = row.FindViewById<TextView>(Resource.Id.txtContent);
                meal = row.FindViewById<TextView>(Resource.Id.txtMeal);
            }
            icon.Text = currentItem.Icon;
            name.Text = currentItem.Name;
            content.Text = currentItem.Content;
            meal.Text = currentItem.Meal;

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