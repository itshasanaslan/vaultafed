using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using App2.Data;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Bson;


namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            App.userOptions = UserOptions.ReadLocalDB();
            if (App.userOptions.KeepLoggedIn)
            {
                EntryUsername.Text = App.userOptions.Username;
                ChxKeepLogged.IsChecked = true;
                EntryPassword.Unfocus();
                EntryPassword.Focus();
            }

            if (App.userOptions.OfflineLogIn)
            {
                EntryUsername.Text = App.userOptions.Username;
                ChxKeepLogged.IsChecked = true;
                EntryPassword.Keyboard = Keyboard.Numeric;
                LblSwitchToOnline.IsVisible = true;
                EntryUsername.IsReadOnly = true;
            }

            InitializeGestureRecognizers();
            EntryUsername.Completed += (s, e) => EntryPassword.Focus();
            EntryPassword.Completed += (s, e) => LogInProcedure(s, e);
            ActivitySpinner.IsRunning = false;

        }

        void InitializeGestureRecognizers()
        {

            // Declare recognizers.
            var tempAct = new TapGestureRecognizer();
            var showPasswordTap = new TapGestureRecognizer();
            var LblKeepSigned = new TapGestureRecognizer();
            var ActionLblSign = new TapGestureRecognizer();
            var ActionLblForgotPassword = new TapGestureRecognizer();

            LblKeepSigned.Tapped += (s, e) =>
            {
                ChangeLoggedInState(s, e);
                ChxKeepLogged.IsChecked = !ChxKeepLogged.IsChecked;
            };

            ActionLblSign.Tapped += (s, e) =>
            {
                NavigateToSignUp(s, e);
            };


            ActionLblForgotPassword.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new ForgotPasswordPage());
            };

            tempAct.Tapped += (s, e) =>
            {
                App.userOptions.OfflineLogIn = false;
                App.userOptions._offlinePassword = "";
                App.userOptions._password = "";
                LblSwitchToOnline.IsVisible = false;
                EntryUsername.IsReadOnly = false;
                EntryPassword.Keyboard = Keyboard.Text;
                EntryPassword.Text = "";
                EntryPassword.Focus();
            };

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

            
            LblKeep.GestureRecognizers.Add(LblKeepSigned);
            LblSignUp.GestureRecognizers.Add(ActionLblSign);
            LblForgotPassword.GestureRecognizers.Add(ActionLblForgotPassword);
            LblSwitchToOnline.GestureRecognizers.Add(tempAct);
            ImageShowPassword.GestureRecognizers.Add(showPasswordTap);
        }

        async void LogInProcedure(object sender, EventArgs e)
        {
            if (EntryUsername.Text == "" || EntryPassword.Text == "")
            {
                await DisplayAlert("Error", "Please provide a username and a password.", "OK");
                return;
            }

            if (EntryPassword.Text.Length < 7 || EntryPassword.Text.Length > 20)
            {
                if (!App.userOptions.OfflineLogIn)

                {
                    await DisplayAlert("Log in", "A password length for this server must be between 7-20 characters. Please type a correct password.", "OK");
                    return;
                }
            }

            try
            {
                if (App.userOptions.OfflineLogIn)
                {
                    ActivitySpinner.IsRunning = true;
                    if (EntryUsername.Text != App.userOptions._username || EntryPassword.Text != App.userOptions._offlinePassword)
                    {
                        EntryPassword.Text = "";
                        await DisplayAlert("Error", "Invalid username or password. Offline log in activated on this device. Remember to use offline password you set in the application", "OK");
                        EntryPassword.Focus();
                        ActivitySpinner.IsRunning = false;
                        return;
                    }
                    else
                    {
                        App.userOptions.MakeOfflineLogin();
                        App.Current.MainPage = new MasterPages();
                        return;
                    }
                }

                ServerResponse logInResponse = new ServerResponse();

                BtnLogIn.IsEnabled = false;
                ActivitySpinner.IsRunning = true;

                User tempUser = new User { username = EntryUsername.Text, password = EntryPassword.Text, eMail = EntryUsername.Text };
                await Task.Factory.StartNew(() =>
                {
                    logInResponse = App.databaseController.Operate("LogIn", tempUser);
                    tempUser = App.databaseController.GetUser(tempUser);
                });


                BtnLogIn.IsEnabled = true;
                ActivitySpinner.IsRunning = false;


                if (!logInResponse.ConnectionSuccessful )
                {
                    await DisplayAlert("Server", "Unable to connect to the server. Please try again later.", "OK");
                    return;
                }

                if (!logInResponse.OperationSuccessful)
                {
                    await DisplayAlert("Log in", logInResponse.ServerMessage, "OK");
                    return;
                }


                if (logInResponse.OperationSuccessful)
                {
                    if (!App.userOptions.CheckSession(EntryUsername.Text))
                    {
                        await DisplayAlert("Error", "Another account is logged in on this device.", "OK");
                        return;
                    }

                    //Log in success 
                    UserDatabaseController.CurrentUser = tempUser;
                    App.userOptions.Username = UserDatabaseController.CurrentUser.username; // to remember username next login   
                    App.userOptions.AuthCode = logInResponse.SplittedMessage(1);
                    UserDatabaseController.CurrentUser.AuthCode = App.userOptions.AuthCode;
                    App.userOptions.SaveLocalDB();
                    App.Current.MainPage = new MasterPages();
                  
                    UserDatabaseController.CurrentUser.password = null;
                    return;
                }
                else
                {
                    //await DisplayAlert("Login", "Login Denied", "Invalid Credentials!");
                    await DisplayAlert("Log in", logInResponse.ServerMessage, "OK");
                    EntryPassword.Text = "";
                }
            }
            catch (InvalidUsernameException f)
            {
                await DisplayAlert("Error", f.Message, "OK");
            }
            ActivitySpinner.IsRunning = false;

        }

        void NavigateToSignUp(object sender, EventArgs e)
        {
            //App.Current.MainPage = new SignUpPage();   
            Navigation.PushAsync(new SignUpPage());
        }

        void ChangeLoggedInState(object sender, EventArgs e)
        {
            App.userOptions.KeepLoggedIn = !App.userOptions.KeepLoggedIn;
            //ChxKeepLogged.IsChecked = App.userOptions.KeepLoggedIn;
        }



    }
}

