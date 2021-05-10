using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App2.Models;
using Xamarin.Forms;
using Android.Gms.Ads;
using App2.Droid.Models;
using App2.ViewModels;

[assembly: Dependency(typeof(AdInterstitial_Droid))]

namespace App2.Droid.Models
{
    class AdInterstitial_Droid : IAdInterstitial
    {
        InterstitialAd interstitialAd;



        public AdInterstitial_Droid()
        {
            interstitialAd = new InterstitialAd(Android.App.Application.Context);

            // TODO: change this id to your admob id  
            interstitialAd.AdUnitId = "ca-app-pub-4368476552163448/4593517797"; //test "ca-app-pub-3940256099942544/1033173712"; 
            LoadAd();

        }

        void LoadAd()
        {
            var requestbuilder = new AdRequest.Builder();
            interstitialAd.LoadAd(requestbuilder.Build());
        }

        public void ShowAd()
        {
            if (interstitialAd.IsLoaded)
                interstitialAd.Show();

            LoadAd();
        }
    }
}