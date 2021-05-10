using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Views;
using App2.Data;
using App2.Models;
using System.Net.Http.Headers;
using App2.ViewModels;

namespace App2
{
    public partial class App : Application
    {
        public static UserDatabaseController databaseController = new UserDatabaseController();
        public static UserOptions userOptions = UserOptions.ReadLocalDB();
        public static AEFListViewModel MainDatabase = new AEFListViewModel();

        public static System.Collections.Generic.List<AEF> FilesOnCache = new System.Collections.Generic.List<AEF>();
        public static System.Collections.Generic.List<AEF> MissingFiles = new System.Collections.Generic.List<AEF>();
      

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

       

    }
}
