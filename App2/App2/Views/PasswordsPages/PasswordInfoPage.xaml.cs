using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Models;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;

namespace App2.Views.PasswordPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordInfoPage : ContentPage
    {
        public Passwords Password;
        public PasswordInfoPage(Passwords Password)
        {
            InitializeComponent();
            this.Password = Password;

            LblTitle.Text = Password.Title;
            EntryUserName.Text =  Password.UserName;
            EntryPassword.Text =  Password.Password;
            LblAddedTime.Text = "Added at:" + Password.AddedTime;
            LblCategory.Text = "Category:" + Password.Category;
            this.Title = this.Password.Title;


            //init show password icon
            var iconTap = new TapGestureRecognizer();
            iconTap.Tapped += (object sender, EventArgs e) =>
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

            ImageShowPassword.GestureRecognizers.Add(iconTap);
            //refresh
            /*
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                LblTitle.Text = Password.Title;
                EntryUserName.Text = Password.UserName;
                EntryPassword.Text = Password.Password;
                LblAddedTime.Text = "Added at:" + Password.AddedTime;
                LblCategory.Text = "Category:" + Password.Category;
                this.Title = this.Password.Title;
                return true;
            });
            */


        }

        void HandleEdit(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPasswordPage(this.Password));
        }
        async void HandleDelete(object sender, EventArgs e)
        {
            App.MainDatabase.RemovePassword(this.Password);
            App.generalDataAccess.DeletePassword(this.Password.Id);
            //App.generalDataAccess.HandlePasswordCategory(this.Password.Category, "Remove");
            await Navigation.PopAsync();
        }

        async void HandleCopy(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Copy which", "Cancel", null, "Username", "Password");
            if (action == "Username")
            {
                await Clipboard.SetTextAsync(this.Password.UserName);
            }
            else if(action == "Password")
            {
                await Clipboard.SetTextAsync(this.Password.Password);

            }
            else
            {
                return;
            }
            //await DisplayAlert(action, "Copied to clipboard.", "Ok");
            DependencyService.Get<IMessage>().LongAlert("Copid to the clipboard.");

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LblTitle.Text = Password.Title;
            EntryUserName.Text = Password.UserName;
            EntryPassword.Text = Password.Password;
            LblAddedTime.Text = "Added at:" + Password.AddedTime;
            LblCategory.Text = "Category:" + Password.Category;
            this.Title = this.Password.Title;
        }



    }
}