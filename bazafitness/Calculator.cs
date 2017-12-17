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
using Microsoft.WindowsAzure.MobileServices;

namespace bazafitness
{
    
    [Activity(MainLauncher = true,
        Icon = "@drawable/ic_launcher", Label = "@string/app_name",
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
        private EditText pro_name;
        private EditText pro_calories;
        private EditText pro_proteins;
        private EditText pro_carbs;
        private EditText pro_fats;

        private Button btnAddProduct;
        private Button btnAddRecipe;
        private Button btnSubmit;

        private Spinner sexSpinner;
        private Spinner activitySpinner;
        private Spinner targetSpinner;
        const string applicationURL = @"https://bazafitness.azurewebsites.net";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.calculator_layout);

            CurrentPlatform.Init();
            
            client = new MobileServiceClient(applicationURL);

            productTable = client.GetTable<Products>();
            recipeTable = client.GetTable<Recipe>();

            btnAddProduct = FindViewById<Button>(Resource.Id.btnAddProduct);
            btnAddRecipe = FindViewById<Button>(Resource.Id.btnAddRecipe);
            //btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);

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
            // refresh view using local store.
            await RefreshItemsFromTableAsync();
        }

        //Refresh the list with the items in the local store.
        private async Task RefreshItemsFromTableAsync()
        {
            try
            {
                // Get the items that weren't marked as checked and add them in the adapter
                var list = await recipeTable.Where(p => p.Deleted == false).ToListAsync();

                adapter.Clear();

                foreach (Recipe current in list)
                    adapter.Add(current);

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