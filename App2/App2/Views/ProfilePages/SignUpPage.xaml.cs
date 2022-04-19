using App2.Data;
using App2.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views.ProfilePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
       
        public SignUpPage()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
           
      
            EntryUsername.Completed += (s, e) => EntryPassword.Focus();
            EntryPassword.Completed += (s, e) => EntryName.Focus();
            EntryName.Completed += (s, e) => EntryLastName.Focus();
            EntryLastName.Completed += (s, e) => EntryMail.Focus();
            EntryMail.Completed += (s, e) => SignUpProcedure(s, e);

            // initialize this page
            // check info
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
            ActivitySpinner.IsRunning = false;
        }

       async  void SignUpProcedure(object sender, EventArgs e)
        {
            if (CheckBoxes())
            {
                User user = new User() {
                    name = EntryName.Text,
                    lastName = EntryLastName.Text,
                    username = EntryUsername.Text,
                    password = EntryPassword.Text,
                    eMail = EntryMail.Text,
                    AuthCode = AEF.GenerateRandomName(25).Substring(4)
                };
                try 
                {

                    ServerResponse logInResponse = new ServerResponse();

                    BtnLogIn.IsEnabled = false;
                    ActivitySpinner.IsRunning = true;

                    User tempUser = new User {
                        username = EntryUsername.Text,
                        password = EntryPassword.Text,
                        eMail = EntryMail.Text,
                        name = EntryName.Text,
                        lastName = EntryLastName.Text,
                        hasPurchased = 0,
                        AuthCode = AEF.GenerateRandomName(25).Substring(4)

                    };


                    await Task.Factory.StartNew(() => {
                        logInResponse = App.databaseController.AddUser( tempUser);
                    });

                    BtnLogIn.IsEnabled = true;
                    ActivitySpinner.IsRunning = false;


                    if (!logInResponse.ConnectionSuccessful)
                    {
                        await DisplayAlert("Server", logInResponse.ServerMessage, "OK");
                        return;
                    }
                   
                    if (logInResponse.ServerMessage.Contains("users.eMail"))
                    {
                        await DisplayAlert("Sign up", EntryMail.Text + " is already taken.", "OK");
                        return;
                    }
                    if (logInResponse.ServerMessage.Contains("users.username"))
                    {
                        await DisplayAlert("Sign up", EntryUsername.Text + " is already taken.", "OK");
                        return;
                    }

                    await DisplayAlert("Sign up", logInResponse.ServerMessage, "OK");
                    if (logInResponse.OperationSuccessful)
                    {

                        NavigateToLogIn(sender, e);
                    }



                }
                catch (Exception wtf)
                {
                    await DisplayAlert("SignUpProcedureFunction", wtf.Message, "OK");
                }

              
            }
            else
            {
                await DisplayAlert("Invalid Info", "You need to complete all boxes.", "OK");
            }
        }

        bool CheckBoxes()
        {
            Entry[] entries = { EntryUsername, EntryPassword, EntryName, EntryLastName, EntryMail };

            if (EntryMail.Text != null && !App.sessionManager.ValidateMail(EntryMail.Text))
            {
                DisplayAlert("Mail", "Please enter a valid e-mail address.", "OK");
                return false;
            }
            foreach (Entry entry in entries)
            {
                if (entry.Text == "" || entry.Text == null)
                {
                    return false;
                }

                
            }

            return true;
        }

        private  void NavigateToLogIn(object sender, EventArgs e)
        {
            //App.Current.MainPage = ScreenManager.loginPage;
            Navigation.PopAsync();
        }

        

    }
}