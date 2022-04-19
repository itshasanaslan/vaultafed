using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Models;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using App2.Data.Concretes;

namespace App2.Views.PasswordPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPasswordPage : ContentPage
    {
        public Passwords Password;
        public bool modifying = false;
        public string firstCategory;
        public AddPasswordPage()
        {
            InitializeComponent();
            this.Password = new Passwords();
            this.Title = "Add a Password";
            Init();
        }
        public AddPasswordPage(Passwords password)
        {
            InitializeComponent();
            this.Password = password;
            modifying = true;
            EntryTitle.Text = this.Password.Title;
            EntryUserName.Text = this.Password.UserName;
            EntryPassword.Text = this.Password.Password;
            btnCategory.Text = this.Password.Category;
            this.Title = "Edit Password";
            this.firstCategory = password.Category;

            Init();
        }


        void Init()
        {
            EntryTitle.Completed += (s, e) => EntryUserName.Focus();
            EntryUserName.Completed += (s, e) => EntryPassword.Focus();   

            // add show password icon touch recognizer
            var showPasswordTap = new TapGestureRecognizer();
            
            showPasswordTap.Tapped += (object sender, EventArgs e) =>
            {
                if (EntryPassword.IsPassword)
                {
                    EntryPassword.IsPassword = false;
                    ImageShowPassword.Source = "dont_show_password.png";
                }
                else
                {
                    EntryPassword.IsPassword = true;
                    ImageShowPassword.Source = "show_password.png";

                }

            };
            ImageShowPassword.GestureRecognizers.Add(showPasswordTap);



        }

        async void HandleSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntryUserName.Text) || string.IsNullOrEmpty(EntryPassword.Text))
            {
                await DisplayAlert("Error","Please provide a title, username and a password.", "OK");
                return;
            }
            this.Password.Title = EntryTitle.Text;
            this.Password.UserName = EntryUserName.Text;
            this.Password.Password = EntryPassword.Text;
            this.Password.Category = btnCategory.Text;
            
            if (!this.modifying) // then this is new.
            {
                this.Password.ActualDateTime = DateTime.Now;
                this.Password.AddedTime = this.Password.ActualDateTime.ToString();
                App.MainDatabase.AddPassword(this.Password);
                App.generalDataAccess.AddPassword(this.Password);
            }
            else
            {
                App.generalDataAccess.UpdatePassword(this.Password);
            }

            /*
            if(this.modifying)
            {
                App.sessionManager.HandlePasswordCategory(this.firstCategory, "Remove");

            }

            App.sessionManager.HandlePasswordCategory(this.Password.Category, "Add");
            */
            
            App.sessionManager.Save(App.session);
            await Navigation.PopAsync();
        }

        private void EntryTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EntryTitle.Text.Length > 15)
            {
                EntryTitle.WidthRequest = (EntryTitle.Text.Length * 10) + 50;
            }
            else
            {
                EntryTitle.WidthRequest = 200;
            }
        }

        private async void HandleCategory(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Category", "Cancel", null, App.generalDataAccess.GetPasswordCategories(App.session.CurrentUser.GetId()));
            //await DisplayAlert("est44", App.generalDataAccess.GetPasswordCategories(App.session.CurrentUser.GetId()).Length.ToString(), "OK");
            if (action == "Cancel" || action == "" || string.IsNullOrWhiteSpace(action))
            {
                btnCategory.Text = "Undefined";
            }
            else if(action == "Add Category")
            {
                string result = await DisplayPromptAsync("Add Category", "");
                if (string.IsNullOrEmpty(result) ||string.IsNullOrWhiteSpace(result))
                {
                    btnCategory.Text = "Undefined";
                }
                else
                {
                    btnCategory.Text = OnlineUserDatabaseController.FormatName(result);
                }
            }
            else
            {
                btnCategory.Text = OnlineUserDatabaseController.FormatName(action);
            }
        }

        private async void HandleCancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}