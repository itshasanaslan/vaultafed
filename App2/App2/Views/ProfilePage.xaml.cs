using App2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public bool ChxInıt = false;
        public ProfilePage()
        {
            InitializeComponent();
            if (App.userOptions.OfflineLogIn)
            {
                LblNameLastName.Text = App.userOptions._name + " " + App.userOptions._lastName;
                LblMail.Text = App.userOptions._email;
                LblUsername.Text = App.userOptions._username;
            }
            else
            {
                LblNameLastName.Text = UserDatabaseController.CurrentUser.name + " " + UserDatabaseController.CurrentUser.lastName;
                LblMail.Text = UserDatabaseController.CurrentUser.eMail;
                LblUsername.Text = UserDatabaseController.CurrentUser.username;
            }
           
            
            if(App.userOptions.OfflineLogIn)
            {
                ChxOffline.IsChecked = true;                
            }
            ChxInıt = true;
            EntrySetPassword.Completed += (s, e) => EntrySetPassword2.Focus();
            EntrySetPassword2.Completed += (s, e) => HandleOfflineLogin(s, e);

            var LblHelpAction = new TapGestureRecognizer();
            LblHelpAction.Tapped += (s, e) =>
            {
                DisplayAlert("Help", "There are 2 types of log in methods for this application.\n1) Online log in: This requires internet connection and this is the " +
                    "safest.\n2)Offline log in: This method does not require internet connection and it is faster to log in, yet, the least secure. To use this, " +
                    "you need to set a local password apart from account password.", "OK");
            };
            LblHelp.GestureRecognizers.Add(LblHelpAction);
            LblCache.Text = (App.userOptions.MaxCacheUsage / 1000000).ToString() + " MB";
            SliderMain.Value = App.userOptions.MaxCacheUsage / 1000000;

        }

        private async void HandleDeleteAccount(object sender, EventArgs e)
        {
            if (App.MainDatabase.FilesSource.Count <= 0 && App.MainDatabase.NotesSource.Count <= 0 && App.MainDatabase.PasswordsSource.Count <= 0)
            {
                bool action = await DisplayAlert("Alert!", "Your account will be deleted from our database. Are you sure?", "Yes", "No");
                if (action)
                {
                    ServerResponse response = new ServerResponse();
                    BtnDeleteAccount.IsEnabled = false;
                    await Task.Factory.StartNew(() =>
                    {
                        response = App.databaseController.Operate("DeleteUser", UserDatabaseController.CurrentUser);
                    });

                    BtnDeleteAccount.IsEnabled = true;

                    if (!response.ConnectionSuccessful)
                    {
                        await DisplayAlert("Server", response.ServerMessage, "OK");
                        return;
                    }

                    if (!response.OperationSuccessful)
                    {
                        await DisplayAlert("Delete Operation", response.ServerMessage, "OK");
                        return;
                    }
                    
                    if (response.OperationSuccessful)
                    {
                        await DisplayAlert("Delete", response.ServerMessage, "OK");
                        UserOptions.DeleteDatabase();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        return;
                    }

                }
            }
            else
            {
                await DisplayAlert("Delete account", "You need to decrypt all data first.", "OK");
            }
        }
        async void HandleChangePassword(object sender, EventArgs e)
        {
            if (App.userOptions.OfflineLogIn)
            {
                await DisplayAlert("Warning", "To change the account's online password, you need to turn off the offline log in method" +
                    " and restart the application.\nTo change offline password, just re-check the setting.", "OK");
                return;
            }
            await Navigation.PushAsync(new NewPasswordPage(UserDatabaseController.CurrentUser, false));
        }

        void HandleToggle(object sender, EventArgs e)
        {
            if (!ChxInıt) return;
            if (!App.userOptions.OfflineLogIn)
            {
                BtnChangePassword.IsVisible = false;
                BtnDeleteAccount.IsVisible = false;
                StkOffline.IsVisible = true;
                StkOffline.IsEnabled = true;
            }
            else
            {
                App.userOptions.OfflineLogIn = false;
                App.userOptions._password = "";
                App.userOptions._offlinePassword = "";
                App.userOptions.SaveLocalDB();
            }


        }

        async void HandleOfflineLogin(object sender, EventArgs e)
        {
            if (EntrySetPassword.Text == "" || EntrySetPassword2.Text == "")
            {
              await DisplayAlert("Error", "Passwords cannot be empty.", "OK");
                return;
            }
            if (EntrySetPassword.Text.Length < 4)
            {
                await DisplayAlert("Error", "Passwords must be at least 4 characters length.", "OK");
                return;
            }
            if (EntrySetPassword.Text != EntrySetPassword2.Text)
            {
                await DisplayAlert("Error", "Passwords does not match.", "OK");
                return;
            }

            foreach(char i in EntrySetPassword.Text)
            {
                if (!Char.IsDigit(i))
                {
                    await DisplayAlert("Error", "Local password must only contain numbers.", "OK");
                    return;
                }
            }
            

            App.userOptions.OfflineLogIn = true;
            App.userOptions.KeepLoggedIn = true;
            App.userOptions._username = UserDatabaseController.CurrentUser.username;
            App.userOptions._offlinePassword = EntrySetPassword.Text;
            StkOffline.IsVisible = false;
            StkOffline.IsEnabled = false;
            BtnChangePassword.IsVisible = true;
            BtnDeleteAccount.IsVisible = true;
            EntrySetPassword.Text = "";
            EntrySetPassword2.Text = "";
            ChxInıt = false;
            ChxOffline.IsChecked = true;
            ChxInıt = true;
            App.userOptions.SetAllInfoForOffline();
            App.userOptions.SaveLocalDB();
            await DisplayAlert("Offline", "Offline login set successfully. You can now log in with your local password.", "OK");
        }

        private void SliderMain_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double value = SliderMain.Value;
            LblCache.Text = value.ToString() + " MB";
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (SliderMain.Value < 10)App.userOptions.MaxCacheUsage = 10000000;
            else
            {
                App.userOptions.MaxCacheUsage = (long)SliderMain.Value * 1000000;
            }
            App.userOptions.SaveLocalDB();
        }

        public async void PromptBackButton()
        {
            App.userOptions.SaveLocalDB();
            string ok = await DisplayActionSheet("Warning", "No", "Yes", "Do you want to close the application?");
            if (ok == "Yes")
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        protected override bool OnBackButtonPressed()
        {

            PromptBackButton();
            return true;
        }

    }
}