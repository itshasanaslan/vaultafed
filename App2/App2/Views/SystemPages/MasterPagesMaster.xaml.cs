using App2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using App2.Views.ProfilePages;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;

namespace App2.Views.SystemPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPagesMaster : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public ListView ListView;

        public MasterPagesMaster()
        {
            InitializeComponent();
          
            string userName = App.session.CurrentUser.GetName();
            BindingContext = new MasterPagesMasterViewModel(Greet(userName));
            ListView = MenuItemsListView;

            if (App.session.CurrentUser.GetHasPurchased() == 1) adBannerToShow.IsVisible = false;
            MainThread.BeginInvokeOnMainThread(() => this.ListenAdmin());

            //DisplayAlert("TEsting on masterpagesmaster", App.session.GetAllProp() + "\n\n" + App.session.CurrentUser.GetAllProp(), "ok");
        }

        class MasterPagesMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterPagesMasterMenuItem> MenuItems { get; set; }
            public string WelcomeText { get; set; }

            public MasterPagesMasterViewModel(string text)
            {
                MenuItems = new ObservableCollection<MasterPagesMasterMenuItem>(new[]
                {
                    new MasterPagesMasterMenuItem { Id = 0, Title = "Home", IconSource ="icons_home.png" , TargetType = typeof(MasterPagesDetail)},
                    new MasterPagesMasterMenuItem { Id = 2, Title = "Profile", IconSource ="icons_username.png", TargetType = typeof(ProfilePage)},
                    new MasterPagesMasterMenuItem { Id = 3, Title = "About", IconSource ="icons_about.png", TargetType = typeof(AboutPage) },

                });
                this.WelcomeText = text;
                if (App.session.CurrentUser.GetHasPurchased() == 0)
                {
                    //MenuItems.Add(new MasterPagesMasterMenuItem { Id = 4, Title = "Remove Ads", IconSource = "icons_about.png" });


                }
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        private string Greet(string user)
        {
            if (DateTime.Now.Hour <= 12)
            {
                return "Good morning, " + user;
            }
            else if (DateTime.Now.Hour <= 16)
            {
                return "Good afternoon, " + user;
            }
            else if (DateTime.Now.Hour <= 20)
            {
                return "Good evening, " + user;
            }
            return "Good night, " + user;
        }

        public async void ListenAdmin()
        {
            //App.userOptions.LastServerMessageId = 0; // to test messages
            ServerResponse response = App.databaseController.AdminWantsToSay();
            if (response.Id == 0)
            {
                App.session.LastServerMessageId = 0;
                
            }

            else if (response.Id == -1 || response.Id <= App.session.LastServerMessageId) return;
            // if -1, it means an error occured. -1 is my number for bad connections.
            // if Id is equal or smaller, it means that they already have seen it.
            bool agreed = false;
            App.session.LastServerMessageId = response.Id;
            
            do
            {
                agreed = await DisplayAlert("IMPORTANT Message From Admin", response.ServerMessage, "Got it!", "Skip");
            } while (!agreed);
            
                App.sessionManager.Update(App.session);
            //await DisplayAlert(response.OperationType, response.ServerMessage, "OK");

        }
    }
}