using App2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTest : ContentPage
    {
        public AddTest()
        {
            InitializeComponent();
            DependencyService.Get<AdBanner>();
        }
        void InterstitialAdShowClick(object sender, EventArgs e)
        {
            DependencyService.Get<IAdInterstitial>().ShowAd();
            DependencyService.Get<AdBanner>();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DisplayAlert("System", "Thank you for your support.", "You're welcome!");
        }
    }
}