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
    public class MainActivity : Activity
    {
        // Client reference.
        private MobileServiceClient client;

        private IMobileServiceTable<Products> productTable;

        // Adapter to map the items list to the view
        private ProductAdapter adapter;

        // EditText containing the "New Products" text
        private EditText textNewProduct;

        // URL of the mobile app backend.
        const string applicationURL = @"https://bazafitness.azurewebsites.net";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Activity_Product);

            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            productTable = client.GetTable<Products>();

            textNewProduct = FindViewById<EditText>(Resource.Id.textNewProducts);

            // Create an adapter to bind the items with the view
            adapter = new ProductAdapter(this, Resource.Layout.Row_List_Product);
            var listViewProdct = FindViewById<ListView>(Resource.Id.listViewProducts);
            listViewProdct.Adapter = adapter;

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
                var list = await productTable.Where(p => p.Deleted == false).ToListAsync();

                adapter.Clear();

                foreach (Products current in list)
                    adapter.Add(current);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        public async Task CheckItem(Products item)
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
                await productTable.UpdateAsync(item);
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
            if (client == null || string.IsNullOrWhiteSpace(textNewProduct.Text))
            {
                return;
            }

            // Create a new item
            var item = new Products()
            {
                Name = textNewProduct.Text,
                Deleted = false
            };

            try
            {
                // Insert the new item into the local store.
                await productTable.InsertAsync(item);
                if (!item.Deleted)
                {
                    adapter.Add(item);
                }
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }

            textNewProduct.Text = "";
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