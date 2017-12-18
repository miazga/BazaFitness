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

namespace bazafitness.Model
{
    public class UserData
    {
        public string Sex { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public double Weight { get; set; }
        public string Activity { get; set; }
        public string Target { get; set; }
        public double Energy { get; set; }//kcal
    }
}