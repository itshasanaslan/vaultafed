using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using App2.Data;
using App2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPasswordPage : ContentPage
    {
        public User user;
        public bool ComeFromLogInPage;
        public NewPasswordPage(User user, bool fromLoginPage)
        {
            InitializeComponent();
            this.user = user;
            this.ComeFromLogInPage = fromLoginPage;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            //LblLOGO.HeightRequest = Constants.LoginIconHeight;
            


        }

        private async void ChangePassword(object sender, EventArgs e)
        {

        if (EntryPassword.Text != EntryPasswordAgain.Text)
            {
            await DisplayAlert("Error", "Passwords does not match!", "OK");
            EntryPasswordAgain.Text = "";
            EntryPassword.Text = "";
        }
           
            if (EntryPassword.Text.Length < 7 || EntryPassword.Text.Length > 20)
            {
                await DisplayAlert("Error", "Password length must be between 7-20", "OK");
                return;
            }

            ServerResponse updateResponse = new ServerResponse();

            ManageLoadingAnimation(true);

            await Task.Factory.StartNew(
                () =>
                {
                    user = App.databaseController.GetUser(this.user);
                }
                );

            if (user.username == "null" && user.password == "null")
            {
                await DisplayAlert("Database ", "It seems, there is no such user.", "OK");
                ManageLoadingAnimation(false);
                return;
            }

            if (user.username == "?" && user.password == "?")
            {
                await DisplayAlert("Connection", "Internet connection error: " + user.name, "OK");
                ManageLoadingAnimation(false);
                return;
            }

            user.password = EntryPassword.Text;
            user.AuthCode = UserDatabaseController.AuthorizationCode;

            await Task.Factory.StartNew(
            () =>
            {
               
                updateResponse =  App.databaseController.Operate("UpdateUser", user);
            }
            );

            if (!updateResponse.ConnectionSuccessful || !updateResponse.OperationSuccessful)
            {
                await DisplayAlert("Connection", updateResponse.ServerMessage + user.AuthCode, "OK");
                ManageLoadingAnimation(false);
                return;
            }

            await DisplayAlert("Success", "Password changed!", "OK");
            if (this.ComeFromLogInPage)
            {
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                App.userOptions._password = EntryPassword.Text;
                await Navigation.PopAsync();
            }
                       
            
         
                


            }
          

        public void ManageLoadingAnimation(bool value)
        {
            BtnChangePassword.IsEnabled = !value;
            ActivitySpinner.IsRunning = value;

            
        }
    }
}