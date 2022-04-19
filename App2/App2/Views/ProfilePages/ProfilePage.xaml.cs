using App2.Data;
using App2.Data.Abstracts;
using App2.Data.Concretes;
using App2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views.ProfilePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public bool ChxInıt = false;
        public ProfilePage()
        {
            InitializeComponent();
            
            LblNameLastName.Text = App.session.CurrentUser.GetName() + " " + App.session.CurrentUser.GetLastName();
            LblMail.Text = App.session.CurrentUser.GetEmail();
            LblUsername.Text = App.session.CurrentUser.GetUsername();
            
            if(App.session.OfflineLogIn)
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
            LblCache.Text = (App.session.MaxCacheUsage / 1000000).ToString() + " MB";
            SliderMain.Value = App.session.MaxCacheUsage / 1000000;

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
                        response = App.databaseController.DeleteUser(App.session.CurrentUser); //SUSPICIOUS CODE
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
                        App.sessionManager.Delete(App.session);
                        //UserOptions.DeleteDatabase();
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
            if (App.session.OfflineLogIn)
            {
                await DisplayAlert("Warning", "To change the account's online password, you need to turn off the offline log in method" +
                    " and restart the application.\nTo change offline password, just re-check the setting.", "OK");
                return;
            }
            await Navigation.PushAsync(new NewPasswordPage(App.session.CurrentUser, false));
        }

        void HandleToggle(object sender, EventArgs e)
        {
            if (!ChxInıt) return;
            if (!App.session.OfflineLogIn)
            {
                BtnChangePassword.IsVisible = false;
                BtnDeleteAccount.IsVisible = false;
                StkOffline.IsVisible = true;
                StkOffline.IsEnabled = true;
            }
            else
            {
                App.session.OfflineLogIn = false;
                App.sessionManager.Update(App.session);
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
            

            App.session.OfflineLogIn = true;
            App.session.KeepLoggedIn = true;
           
            App.session._offlinePassword = EntrySetPassword.Text;
            StkOffline.IsVisible = false;
            StkOffline.IsEnabled = false;
            BtnChangePassword.IsVisible = true;
            BtnDeleteAccount.IsVisible = true;
            EntrySetPassword.Text = "";
            EntrySetPassword2.Text = "";
            ChxInıt = false;
            ChxOffline.IsChecked = true;
            ChxInıt = true;
         
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
            if (SliderMain.Value < 10)App.session.MaxCacheUsage = 10000000;
            else
            {
                App.session.MaxCacheUsage = (long)SliderMain.Value * 1000000;
            }
          
        }

        public async void PromptBackButton()
        {
            
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

        private async void SaveDatabaseFile(object sender, EventArgs e)
        {
            string path = "/storage/emulated/0/Download/afed_file.db3";
            try
            {

                byte[] file = System.IO.File.ReadAllBytes(
                    DependencyService.Get<ISQLite>().GetDatabaseName());
                System.IO.File.WriteAllBytes(path, file);
                await DisplayAlert("Save db", "Successfully saved to: " + path, "Ok");
                DependencyService.Get<IMessage>().LongAlert("Successfully saved to: " + path);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void btnRestore_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Restore database","Enter the path.\nExample: /storage/emulated/0/file.db3");
            if (result == null || result == "" || result == " ")
            {
                DependencyService.Get<IMessage>().LongAlert("Path is null!");
                return;
            }

            if (!System.IO.File.Exists(result))
            {
                DependencyService.Get<IMessage>().LongAlert("No such file has been found: " + result );
                return;
            }

            string o = await DisplayPromptAsync("Restore database", "New database file will be replaced. It means that you will lose your current profile. To proceed, type 'ok'");
            if (o == "ok" || o == "Ok")
            {
                try
                {
                    System.IO.File.WriteAllBytes(DependencyService.Get<ISQLite>().GetDatabaseName(), System.IO.File.ReadAllBytes(result));
                    await DisplayAlert("Restore", "Database has been restored successfully. App will be terminated.", "Ok");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch (Exception fck)
                {
                    DependencyService.Get<IMessage>().LongAlert(fck.Message);
                    return;
                }
            }


           
        }
    }
}