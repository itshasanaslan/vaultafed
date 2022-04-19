using App2.Data;
using App2.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views.ProfilePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        public static User user;
        public ForgotPasswordPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            LblLOGO.HeightRequest = Constants.LoginIconHeight;
            EntryUsername.Completed += (s, e) => BtnSend.Focus();
            ActivitySpinner.IsRunning = false;
            

        }

       private async void SendCode(object sender, EventArgs e)
        {
            user = new User();
            // check mail format
            // check if user exists
            // if not, return
            // send post request to send a mail.
            // if request is successful, receive code.
            // if not display alert and return
            // send post request to control if code is correct.
            // 


            if (!App.sessionManager.ValidateMail(EntryUsername.Text) || EntryUsername.Text == null)
            {
                await DisplayAlert("Error!", "Mail format is not valid!", "OK");
                return;
            }

            ServerResponse checkUserResponse = new ServerResponse();
            user.SetEmail (EntryUsername.Text.ToLower());

            BtnSend.IsEnabled = false;
            ActivitySpinner.IsRunning = true;

            await Task.Factory.StartNew(() => {
                checkUserResponse = App.databaseController.CheckUserExists(user);
            });


            BtnSend.IsEnabled = true;
            ActivitySpinner.IsRunning = false;


            if (!checkUserResponse.ConnectionSuccessful || !checkUserResponse.OperationSuccessful)
            {
                await DisplayAlert("Connection", checkUserResponse.ServerMessage, "OK");
                return;
            }
            
           

            else
            {
                ActivitySpinner.IsRunning = true;
                BtnSend.IsEnabled = false;
                LblInfo.Text = "Sending a mail from server..";

                try
                {
                    await Task.Factory.StartNew(() => {
                        checkUserResponse = App.databaseController.SendMailCode(user);
                        user = App.databaseController.GetUser(user);
                        user.SetPassword(null);

                    });

                    ActivitySpinner.IsRunning = false;
                    if (!checkUserResponse.ConnectionSuccessful || !checkUserResponse.OperationSuccessful)
                    {
                        await DisplayAlert("Server Connection", checkUserResponse.ServerMessage, "OK");
                        await Navigation.PopAsync();
                        return;
                    }

                    // if success.
                    

                            
                    LblInfo.Text = "A mail has been  sent to " + EntryUsername.Text + "!";
                    LblInfo.IsEnabled = true;
                    LblInfo.IsVisible = true;


                    BtnSend.IsVisible = false;
                    BtnVerify.IsEnabled = true;
                    BtnVerify.IsVisible = true;
                    EntryUsername.Text = "";
                    EntryUsername.Placeholder = "Reset code..";
                    DependencyService.Get<IMessage>().LongAlert("Check your mailbox!");

                }
                catch (Exception f)
                {
                    BtnSend.IsEnabled = true;
                    await DisplayAlert("Mail Error", f.Message, "Ok");
                }


                ActivitySpinner.IsRunning = false;
            }
        }

       private async void VerifyCode(object sender, EventArgs e)
        {
            user.SetPassword(EntryUsername.Text); // sending code to server as a password.
            ServerResponse codeResponse = new ServerResponse();

            ActivitySpinner.IsRunning = true;
            BtnVerify.IsEnabled = false;
            await Task.Factory.StartNew(
                () => {
                    codeResponse = App.databaseController.VerifyMailCode(user);
                });
            ActivitySpinner.IsEnabled = false;
            BtnVerify.IsEnabled = true;

            if (!codeResponse.ConnectionSuccessful ||!codeResponse.OperationSuccessful)
            {
                await DisplayAlert("Server Error", codeResponse.ServerMessage, "OK");
                return;
            }

         
              await Navigation.PushAsync(new NewPasswordPage(user, true));
         

        }

       

        
    }
}