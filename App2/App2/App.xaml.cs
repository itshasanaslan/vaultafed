using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Views;
using App2.Data;
using App2.Models;
using System.Net.Http.Headers;
using App2.ViewModels;
using App2.Data.Abstracts;
using App2.Data.Concretes;
using App2.Views.ProfilePages;
using App2.Business.Concretes;
namespace App2
{
    public partial class App : Application
    {
        public static ConfigInformation config = ConfigInformation.Load(ConfigInformation.GetConfigPath());
        public static IDatabaseController databaseController = new OfflineDatabaseController(); //OnlineUserDatabaseController();


        public static ISessionManager sessionManager = new SessionManager(new OfflineDatabaseController());
        public static IGeneralDataAccess generalDataAccess = new GeneralDataManager();

        public static Session session = sessionManager.Start();


        public static AEFListViewModel MainDatabase = new AEFListViewModel();

        public static System.Collections.Generic.List<AEF> FilesOnCache = new System.Collections.Generic.List<AEF>();
        public static System.Collections.Generic.List<AEF> MissingFiles = new System.Collections.Generic.List<AEF>();

        public static ImageResizer imageResizer = new ImageResizer();
       

        public App()
        {

            InitializeComponent();   
            MainPage = new NavigationPage(new LoginPage());
            ConfigInformation.Save(config);
            
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
