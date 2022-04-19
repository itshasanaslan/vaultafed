using App2.Models;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#pragma warning disable CS1998
namespace App2.Views.PasswordPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordsPage : ContentPage
    {

        public bool MultiSelectionEnabled = false;
        ToolbarItem TlbSelectionMode = new ToolbarItem { Text = "SELECT", Order = ToolbarItemOrder.Primary };
        ToolbarItem TlbSortItems = new ToolbarItem { Text = "Sort", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbSelectAll = new ToolbarItem { Text = "Select All", Order = ToolbarItemOrder.Secondary };

        public PasswordsPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {

            //update lstview.
            if (!App.MainDatabase.passwordSourceInitialized)
            {
                App.MainDatabase.PasswordsSource = new System.Collections.ObjectModel.ObservableCollection<Passwords>();
                foreach (Passwords i in App.generalDataAccess.GetAllPassword(App.session.CurrentUser.GetId()))
                { 
                    App.MainDatabase.AddPassword(i);
                }
                App.MainDatabase.passwordSourceInitialized = true; // to prevent memory leaks, I need to control this.
                this.SortInner(App.session.PasswordsSortMethod);
            }


            lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource;

            //Add click events
            TlbSelectionMode.Clicked += ManageSelectionMode;
            TlbSortItems.Clicked += SortItems;

            TlbSelectAll.Clicked += (s, e) => {
                foreach (var i in App.MainDatabase.PasswordsSource)
                {
                    lstPasswords.SelectedItems.Add(i);
                }
                TlbSelectAll.IsEnabled = false;
            };
            TlbSelectAll.IsEnabled = false;

            ToolbarItems.Add(TlbSelectionMode);
            ToolbarItems.Add(TlbSortItems);
            ToolbarItems.Add(TlbSelectAll);
            BtnRemoveActions(false);

            searchBar.TextChanged += (s, e) => FilterItem(searchBar.Text);
            searchBar.SearchButtonPressed += (s, e) => FilterItem(searchBar.Text);
        }

        private void AddPassword(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPasswordPage());
        }
       
        private async void HandleRemoveButton(object sender, EventArgs e)
        {
            if (lstPasswords.SelectedItems.Count <= 0)
            {
                await DisplayAlert("Error", "You have to select at least one note.", "OK");
                return;
            }
       
            var tempCollection = new ObservableCollection<Passwords>();

         
            foreach (Passwords i in lstPasswords.SelectedItems)
            {
                tempCollection.Add(i);
            }
                    

            foreach (var i in tempCollection)
            {

                await Task.Run(async () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RemovePassword(i);
                        
                    }
                    );
                });
            }
            App.sessionManager.Save(App.session);

            BtnRemoveActions(false);
            ManageSelectionMode(sender, e);
        }
        
        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tempPassword = e.CurrentSelection.FirstOrDefault() as Passwords;
            if (tempPassword == null)
            {
                return;
            }
               ((CollectionView)sender).SelectedItem = null;
            if (!MultiSelectionEnabled)
            {
                await Navigation.PushAsync(new PasswordInfoPage(tempPassword));
            }
            else

            {
                tlbShowHowManySelected.Text = lstPasswords.SelectedItems.Count.ToString() + " selected";
                BtnRemoveActions(true);
            }
        }

        private void ManageSelectionMode(object sender, EventArgs e)
        {
            MultiSelectionEnabled = !MultiSelectionEnabled;
            if (MultiSelectionEnabled)
            {
                TlbSelectionMode.Text = "CANCEL";
                lstPasswords.SelectionMode = SelectionMode.Multiple;
                TlbSelectAll.IsEnabled = true;
            }
            else
            {
                BtnRemoveActions(false);
                TlbSelectionMode.Text = "SELECT";
                TlbSelectAll.IsEnabled = false;
                lstPasswords.SelectionMode = SelectionMode.Single;
                lstPasswords.SelectedItems.Clear();
                tlbShowHowManySelected.Text = "";
            }
        }

        private async void SortItems(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by", "Cancel", null,
                 "Added Time (Now-Past)",
                 "Added Time (Past-Now)",
                 "Category (A-Z)",
                 "Category (Z-A)",
                 "Title (A-Z)",
                 "Title (Z-A)",
                 "Username (A-Z)",
                 "Username (Z-A)"
                 );

            this.SortInner(action);
        }


        private void SortInner(string action)
        {
            switch (action)
            {
                case "Added Time (Now-Past)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderByDescending(x => x.ActualDateTime);
                    break;
                case "Added Time (Past-Now)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderBy(x => x.ActualDateTime);
                    break;
                case "Category (A-Z)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderBy(x => x.Category);
                    break;
                case "Category (Z-A)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderByDescending(x => x.Category);
                    break;
                case "Title (A-Z)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderBy(x => x.Title);
                    break;
                case "Title (Z-A)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderByDescending(x => x.Title);
                    break;
                case "Username (A-Z)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderBy(x => x.UserName);
                    break;
                case "Username (Z-A)":
                    lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.OrderByDescending(x => x.UserName);
                    break;
                default:
                    break;
            }
            App.session.PasswordsSortMethod = action;
        }

        private void BtnRemoveActions(bool value)
        {
            BtnRemove.IsEnabled = value;
            BtnRemove.IsVisible = value;
        }

        private void RemovePassword(Passwords password)
        {
            App.MainDatabase.RemovePassword(password);
            App.generalDataAccess.DeletePassword(password.Id);
        }

        private void UpdateCollectionView(object sender, EventArgs e)
        {
            lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource;
            refreshView.IsRefreshing = false;

        }

        private void FilterItem(string text)
        {
            refreshView.IsRefreshing = true;
            if (string.IsNullOrWhiteSpace(text))
            {
                lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource;
            }
            else
            {
                lstPasswords.ItemsSource = App.MainDatabase.PasswordsSource.Where(x => x.UserName.ToLower().Contains(text.ToLower()));
            }
            refreshView.IsRefreshing = false;
        }
    }
}