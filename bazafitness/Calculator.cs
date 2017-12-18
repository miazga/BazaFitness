using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using bazafitness.DAL;
using bazafitness.Model;
using Microsoft.WindowsAzure.MobileServices;

namespace bazafitness
{
    
    [Activity(MainLauncher = true,
        Icon = "@drawable/meal", Label = "@string/app_name",
        Theme = "@style/AppTheme")]
    public class Calculator : Activity
    {
        // Client reference.
        private MobileServiceClient client;

        private IMobileServiceTable<Products> productTable;
        private IMobileServiceTable<Recipe> recipeTable;

        // Adapter to map the items list to the view
        private CalculatorAdapter adapter;

        // EditText containing the "New Products" text
        
        private EditText txtAge;
        private EditText txtHeight;
        private EditText txtWeight;

        private Button btnAddProduct;
        private Button btnAddRecipe;
        private Button btnSubmit;

        private Spinner sexSpinner;
        private Spinner activitySpinner;
        private Spinner targetSpinner;

        private UserData userData;

        const string applicationURL = @"https://bazafitness.azurewebsites.net";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.calculator_layout);

            CurrentPlatform.Init();
            
            client = new MobileServiceClient(applicationURL);

            productTable = client.GetTable<Products>();
            recipeTable = client.GetTable<Recipe>();

            txtAge = FindViewById<EditText>(Resource.Id.age);
            txtHeight = FindViewById<EditText>(Resource.Id.height);
            txtWeight = FindViewById<EditText>(Resource.Id.weight);

            btnAddProduct = FindViewById<Button>(Resource.Id.btnAddProduct);
            btnAddRecipe = FindViewById<Button>(Resource.Id.btnAddRecipe);
            btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);

            adapter = new CalculatorAdapter(this, Resource.Layout.row_list_calculator);
            var listViewRecipe = FindViewById<ListView>(Resource.Id.listViewRecipes);
            listViewRecipe.Adapter = adapter;

            btnAddProduct.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
            btnAddRecipe.Click += delegate
            {
                StartActivity(typeof(RecipeActivity));
            };
            btnSubmit.Click += delegate
            {
                OnRefreshItemsSelected();
            };

            //spinnery
            sexSpinner = FindViewById<Spinner>(Resource.Id.sex);
            var sexSpinnerAdapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.sex_array, Android.Resource.Layout.SimpleSpinnerItem);
            sexSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sexSpinner.Adapter = sexSpinnerAdapter;
            activitySpinner = FindViewById<Spinner>(Resource.Id.activity);
            var activitySpinnerAdapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.activity_array, Android.Resource.Layout.SimpleSpinnerItem);
            activitySpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            activitySpinner.Adapter = activitySpinnerAdapter;
            targetSpinner = FindViewById<Spinner>(Resource.Id.target);
            var targetSpinnerAdapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.target_array, Android.Resource.Layout.SimpleSpinnerItem);
            targetSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            targetSpinner.Adapter = targetSpinnerAdapter;

        }

        //Initializes the activity menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_main, menu);
            return true;
        }

        //Select an option from the menu
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_refresh)
            {
                item.SetEnabled(false);

                OnRefreshItemsSelected();

                item.SetEnabled(true);
            }
            return true;
        }

        // Called when the refresh menu option is selected.
        private async void OnRefreshItemsSelected()
        {
            if (client == null || string.IsNullOrWhiteSpace(txtAge.Text) || string.IsNullOrWhiteSpace(txtHeight.Text) || string.IsNullOrWhiteSpace(txtWeight.Text))
            {
                return;
            }
            // refresh view using local store.
            await RefreshItemsFromTableAsync();
        }

        private void GetEnergy()
        {
            userData = new UserData()
            {
                Activity = activitySpinner.SelectedItem.ToString(),
                Age = Int32.Parse(txtAge.Text),
                Height = Int32.Parse(txtHeight.Text),
                Sex = sexSpinner.SelectedItem.ToString(),
                Weight = Double.Parse(txtWeight.Text, System.Globalization.CultureInfo.InvariantCulture),
                Target = targetSpinner.SelectedItem.ToString()
            };
            double addOn;
            double activityMultiplier;
            double targetMultiplier;
            switch (userData.Sex)
            {
                case "Mężczyzna":
                    addOn = 5;
                    break;
                default:
                    addOn = -161;
                    break;
            }
            switch (userData.Activity)
            {
                case "Raczej siedząca": activityMultiplier = 0.85;
                    break;
                case "Dużo ruchu": activityMultiplier = 1.15;
                    break;
                default: activityMultiplier = 1.0;
                    break;
            }
            switch (userData.Target)
            {
                case "Spadek wagi": targetMultiplier = 0.85;
                    break;
                case "Wzrost wagi": targetMultiplier = 1.15;
                    break;
                default: targetMultiplier = 1.0;
                    break;
            }
            userData.Energy = Math.Round((9.99 * userData.Weight + 6.25 * userData.Height + 4.92 * userData.Age + addOn) *
                              activityMultiplier * targetMultiplier);
        }

        private async Task<Recipe> GetMealDescriptionAsync(string mealName, double mealEnergy)
        {
            var filter = string.Empty;

            switch (userData.Target)
            {
                case "Spadek wagi":
                    filter = "Na redukcję";
                    break;
                case "Wzrost wagi":
                    filter = "Na masę";
                    break;
            }
            List<Recipe> mealList;
            if (string.IsNullOrEmpty(filter))
            {
                mealList =
                    await recipeTable.Where(p =>
                        p.Deleted == false && p.Meal == mealName).ToListAsync();
            }
            else
            {
                mealList =
                    await recipeTable.Where(p =>
                        p.Deleted == false && p.Meal == mealName && p.Category == filter).ToListAsync();
            }
            
            var rand = new Random();
            var meal = mealList[rand.Next(mealList.Count)];

            var mealIngredients = new Dictionary<string, string>();
            var IngTab = meal.Content.Split(';');
            for (int i = 0; i < IngTab.Length; i += 2)
            {
                mealIngredients.Add(IngTab[i].Trim(), IngTab[i + 1].Trim());
            }
            var counter = 1.0;
            var kcalCount = 0.0;
            var proteinCount = 0.0;
            var carbsCount = 0.0;
            var fatsCount = 0.0;
            string igredientsList = string.Empty;
            foreach (var item in mealIngredients)
            {
                var productList = await productTable.Where(p => p.Deleted == false && p.Name == item.Key).ToListAsync();
                var product = productList.FirstOrDefault();
                var multiplier = Int32.Parse(item.Value);
                kcalCount += product.Calories * multiplier;
                proteinCount += product.Proteins * multiplier;
                carbsCount += product.Carbohydrate * multiplier;
                fatsCount += product.Fats * multiplier;
                igredientsList += string.Format(" {0}x {1}", item.Value, item.Key);
            }
            counter = Math.Round((mealEnergy / kcalCount) * 2, MidpointRounding.AwayFromZero) / 2;
            meal.Content =
                string.Format(
                    "{0}kcal w tym {1}g białka {2}g węglowodanów i {3}g tłuszczy\n{4} składa się z {5} porcji, w skład każdej porcji wchodzi:{6}",
                    kcalCount * counter, proteinCount * counter, carbsCount * counter, fatsCount * counter, meal.Meal, counter, igredientsList);
            return meal;
        }
        //Refresh the list with the items in the local store.
        private async Task RefreshItemsFromTableAsync()
        {
            try
            {
                GetEnergy();
                var breakfastEnergy = userData.Energy * 0.35;
                var lunchEnergy = userData.Energy * 0.45;
                var dinnerEnergy = userData.Energy * 0.2;
                string toast = string.Format("Potrzebujesz {0} kcal", userData.Energy);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
                adapter.Clear();
                var breakfast = await GetMealDescriptionAsync("Śniadanie", breakfastEnergy);
                adapter.Add(breakfast);
                var lunch = await GetMealDescriptionAsync("Obiad", lunchEnergy);
                adapter.Add(lunch);
                var dinner = await GetMealDescriptionAsync("Kolacja", dinnerEnergy);
                adapter.Add(dinner);
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }
        
        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}