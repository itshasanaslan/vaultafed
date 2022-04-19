using UIKit;
using Xamarin.Forms;
using Google.MobileAds;
using App2.ViewModels;
using App2.iOS.ViewModels;


[assembly: Dependency(typeof(AdInterstitial_iOS))]

#pragma warning disable CS0618
namespace App2.iOS.ViewModels
{
    public class AdInterstitial_iOS : IAdInterstitial
    {
        Interstitial interstitial;
        public AdInterstitial_iOS()
        {
            LoadAd();
            interstitial.ScreenDismissed += (s, e) => LoadAd();
        }

        void LoadAd()
        {
            // TODO: change this id to your admob id    
            interstitial = new Interstitial("ca-app-pub-4368476552163448/8914351850");

            var request = Request.GetDefaultRequest();
            request.TestDevices = new string[] { "Your Test Device ID", "GADSimulator" };
            interstitial.LoadRequest(request);
        }

        public void ShowAd()
        {
            if (interstitial.IsReady)
            {
                var viewController = GetVisibleViewController();
                interstitial.PresentFromRootViewController(viewController);
            }
        }
        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;
            }

            if (rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }

    }
}