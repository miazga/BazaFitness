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

namespace bazafitness.DAL
{
    public class Recipe
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "REC_NAME")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "REC_CONTENT")]
        public string Content { get; set; }
        
    }

    public class RecipeWrapper : Java.Lang.Object
    {
        public RecipeWrapper(Recipe item)
        {
            Recipe = item;
        }
        public Recipe Recipe { get; private set; }
    }
}