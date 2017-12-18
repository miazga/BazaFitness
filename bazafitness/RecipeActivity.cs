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
    [Activity(Label = "Dodaj przepis")]
    public class RecipeActivity : Activity
    {
        // Client reference.
        private MobileServiceClient client;

        private IMobileServiceTable<Recipe> recipeTable;

        // Adapter to map the items list to the view
        private RecipeAdapter adapter;
        
        private EditText rec_icon;
        private EditText rec_name;
        private EditText rec_content;
        private Spinner spinnerMeal;
        private Spinner spinnerCategory;

        // URL of the mobile app backend.
        const string applicationURL = @"https://bazafitness.azurewebsites.net";

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Activity_Recipe);

            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            recipeTable = client.GetTable<Recipe>();

            rec_icon = FindViewById<EditText>(Resource.Id.rec_icon);
            rec_name = FindViewById<EditText>(Resource.Id.rec_name);
            rec_content = FindViewById<EditText>(Resource.Id.rec_content);

            // Create an adapter to bind the items with the view
            adapter = new RecipeAdapter(this, Resource.Layout.row_list_recipe);
            var listViewToDo = FindViewById<ListView>(Resource.Id.listViewRecipes);
            listViewToDo.Adapter = adapter;

            spinnerMeal = FindViewById<Spinner>(Resource.Id.spinnerMeal);
            spinnerCategory = FindViewById<Spinner>(Resource.Id.spinnerCategory);
            
            var spinnerMealAdapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.meal_array, Android.Resource.Layout.SimpleSpinnerItem);

            spinnerMealAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMeal.Adapter = spinnerMealAdapter;

            var spinnerCategoryAdapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.category_array, Android.Resource.Layout.SimpleSpinnerItem);

            spinnerCategoryAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerCategory.Adapter = spinnerCategoryAdapter;

            // Load the items from the mobile app backend.
            OnRefreshItemsSelected();
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
                // Get the items that weren't marked as completed and add them in the adapter
                var list = await recipeTable.Where(item => item.Deleted == false).ToListAsync();

                adapter.Clear();

                foreach (Recipe current in list.OrderBy(x => x.Name).ThenBy(x => x.Meal))
                    adapter.Add(current);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        public async Task CheckItem(Recipe item)
        {
            if (client == null)
            {
                return;
            }

            // Set the item as completed and update it in the table
            item.Deleted = true;
            try
            {
                // Update the new item in the local store.
                await recipeTable.UpdateAsync(item);
                if (item.Deleted)
                    adapter.Remove(item);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        [Java.Interop.Export()]
        public async void AddItem(View view)
        {
            if (client == null || string.IsNullOrWhiteSpace(rec_icon.Text) || string.IsNullOrWhiteSpace(rec_content.Text) || string.IsNullOrWhiteSpace(rec_name.Text) || string.IsNullOrWhiteSpace(spinnerMeal.SelectedItem.ToString()))
            {
                return;
            }

            // Create a new item
            var item = new Recipe()
            {
                Name = rec_name.Text,
                Icon = rec_icon.Text,
                Content = rec_content.Text,
                Meal = spinnerMeal.SelectedItem.ToString(),
                Category = spinnerCategory.SelectedItem.ToString(),
                Deleted = false
            };

            try
            {
                // Insert the new item into the local store.
                await recipeTable.InsertAsync(item);

                if (!item.Deleted)
                {
                    adapter.Add(item);
                }
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }

            rec_name.Text = "";
            rec_icon.Text = "";
            rec_content.Text = "";
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