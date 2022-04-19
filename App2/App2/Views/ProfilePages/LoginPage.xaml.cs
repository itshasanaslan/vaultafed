using System;
using System.Threading.Tasks;
using App2.Models;
using App2.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Data.Concretes;
using App2.Views.SystemPages;
namespace App2.Views.ProfilePages
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
            
            if (App.session.KeepLoggedIn && App.session.CurrentUser != null)
            {
                //EntryUsername.Text = App.session.CurrentUser.GetUsername();
                ChxKeepLogged.IsChecked = true;
                EntryUsername.Text = App.session.CurrentUser.GetUsername();
                EntryPassword.Unfocus();
                EntryPassword.Focus();
                DependencyService.Get<IMessage>().ShortAlert("Latest session: " + App.session.LatestSession.ToString());
             
            }

            if (App.session.OfflineLogIn)
            {
               // EntryUsername.Text = App.session.CurrentUser.GetUsername();
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
                App.session.KeepLoggedIn = ChxKeepLogged.IsChecked;
                App.sessionManager.Update(App.session);
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
                App.session.OfflineLogIn = false;
                App.session._offlinePassword = "";
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
            ServerResponse basicCheck = this.IfBoxesAreFull();
            if (!basicCheck.OperationSuccessful)
            {
                await DisplayAlert(basicCheck.OperationType, basicCheck.ServerMessage, "OK");
                return;
            }

            try
            {
                /* offlinee login
                 * was
                 * here
                 * implement
                 * later
                 * saving under garbageofflinelogin
                 */
                ServerResponse logInResponse = new ServerResponse();
                BtnLogIn.IsEnabled = false;
                ActivitySpinner.IsRunning = true;
                //create a temporary user class to assign entry boxes.
                User tempUser = new User { username = EntryUsername.Text, password = EntryPassword.Text, eMail = EntryUsername.Text };
                //await DisplayAlert("Test", tempUser.Username + " " + tempUser.Password, "Ok");
                await Task.Factory.StartNew(() =>
                {
                    logInResponse = App.databaseController.LogIn(tempUser); //check log in info
                    tempUser = App.databaseController.GetUser(tempUser); //get user info from online or offline.
                });

                BtnLogIn.IsEnabled = true;
                ActivitySpinner.IsRunning = false;

                // İf a problem occurs.
                ServerResponse checkConnectionSuccess = this.GetOperationAndConnection(logInResponse);
                if (!logInResponse.OperationSuccessful)
                {
                    await DisplayAlert(checkConnectionSuccess.OperationType, checkConnectionSuccess.ServerMessage, "Ok");
                    return;
                }

                Session session = null;
                    
                // online class
                if (!App.databaseController.IsOfflineClass())
                {
                    session = await GetOnlineSession(tempUser);
                }
                //OFFLINE / else
                else 
                {
                    session = await GetOfflineSession(tempUser);
                }

                App.session = session;
                App.session.KeepLoggedIn = ChxKeepLogged.IsChecked;
                App.sessionManager.Save(App.session);

                //await DisplayAlert("test e", App.session.GetAllProp(), "ok");
                App.Current.MainPage = new MasterPages();
                return;
                }
               
            
            catch (Exception f)
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
            App.session.KeepLoggedIn = !App.session.KeepLoggedIn;
            

        }

        async Task<Session> GetOnlineSession(User tempUser)
        {
            Session session;
            // Check if user saved to local db before.
            User userExists = OnlineUserDatabaseController.GetUserFromLocal(tempUser);
            if (userExists == null)
            {
                User newUser;
                OnlineUserDatabaseController.localController.AddUser(tempUser);// Save it 
                newUser = OnlineUserDatabaseController.GetUserFromLocal(tempUser); // retrieve with id.
                session = App.sessionManager.GetNew(); // if user is null, session is also null
                session.CurrentUser = newUser;
                session.UserId = newUser.GetId();
                App.sessionManager.Save(session);
                //await DisplayAlert("User not found on local.", "Saved to local and created a session", "OK");
            }

            else
            { // kullanıcı null değilse, sessionu retrieve et.
                session = App.sessionManager.Get(userExists.GetId());
                if (session == null)
                {
                  //  await DisplayAlert("Code 44", "Sesssion is null. Close app", "ok");
                    return null;
                }
                OnlineUserDatabaseController.localController.UpdateUser(userExists);
                session.CurrentUser = userExists;
                session.UserId = userExists.GetId();
            }
            return session;
        }

        async Task<Session> GetOfflineSession(User tempUser)
        {
            Session session = App.sessionManager.Get(tempUser.GetId());
            if (session == null)
            {
                session = App.sessionManager.GetNew();
                session.CurrentUser = tempUser;
                session.UserId = tempUser.GetId();
                App.sessionManager.Save(session);
              // await DisplayAlert("Session is null for user with id: " + tempUser.GetId().ToString(), "Created new", "ok");
            }
            else
            {
                session.CurrentUser = tempUser;
                session.UserId = tempUser.GetId();
               //await DisplayAlert("Session is not null.", "Good", "OK");
            }
            return session;
        }

        public ServerResponse IfBoxesAreFull()
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = true,
                OperationType = "Log in"
            };
            if (EntryUsername.Text == "" || EntryPassword.Text == "")
            {
                response.ServerMessage = "Please provide a username and a password.";
                response.OperationType = "Error";
                response.OperationSuccessful = false;


            }

            if (EntryPassword.Text.Length < 7 || EntryPassword.Text.Length > 20)
            {
                if (!App.session.OfflineLogIn)

                {
                    response.ServerMessage = "A password length for this server must be between 7-20 characters. Please type a correct password.";
                    response.OperationSuccessful = false;
                }
            }
            return response;
        }

        private ServerResponse GetOperationAndConnection(ServerResponse logInResponse)
        {
            ServerResponse response = new ServerResponse();
            response.OperationSuccessful = false;

            if (!logInResponse.ConnectionSuccessful)
            {
                response.OperationType = "Server";
                response.ServerMessage = "Unable to connect to the server. Please try again later.";
            }

            else if (!logInResponse.OperationSuccessful)
            {
                response.OperationType = "Log in";
                response.ServerMessage = logInResponse.ServerMessage;     
            }
            else
            {
                response.OperationSuccessful = true;
            }

            return response;
        }

        void GarbageOfflineLogin()
        {
            /*
               if (App.session.OfflineLogIn)
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
               */
        }
    }
}

