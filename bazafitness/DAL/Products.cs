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
using Newtonsoft.Json;

namespace bazafitness
{
    public class Products
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "PRO_NAME")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "PRO_CALORIES")]
        public int Calories { get; set; }
        [JsonProperty(PropertyName = "PRO_PROTEINS")]
        public int Proteins { get; set; }
        [JsonProperty(PropertyName = "PRO_CARBOHYDRATE")]
        public int Carbohydrate { get; set; }
        [JsonProperty(PropertyName = "PRO_FATS")]
        public int Fats { get; set; }
        [JsonProperty(PropertyName = "Checked")]
        public bool Deleted { get; set; }

    }

    public class ProductWrapper : Java.Lang.Object
    {
        public ProductWrapper(Products item)
        {
            Products = item;
        }
        public Products Products { get; private set; }
    }
}