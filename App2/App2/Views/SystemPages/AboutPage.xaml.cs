using App2.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#pragma warning disable CS4014

namespace App2.Views.SystemPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public IList<UpdateCalendarLVModel> Updates { get; private set; }

        public AboutPage()
        {
            InitializeComponent();
            BindingContext = this;
            InitializeListElements();
            lstUpdates.ItemsSource = Updates;
            Init();
            

        }
        private  void Init()
        {
            var twitterRecognizer = new TapGestureRecognizer();
            var instagramRecgnizer = new TapGestureRecognizer();
            var mailRecognizer = new TapGestureRecognizer();
            var siteRecognizer = new TapGestureRecognizer();


            twitterRecognizer.Tapped += (s, e) => 
            {
                Uri twitUri = new Uri("http://www.twitter.com/itshasanaslan", UriKind.Absolute);
                OpenBrowser(twitUri);
            };
            instagramRecgnizer.Tapped += (s, e) => {
                Uri instagramUri = new Uri("http://www.instagram.com/itshasanaslan", UriKind.Absolute);
                OpenBrowser(instagramUri);
            };
            mailRecognizer.Tapped += (s, e) => 
            {
                Uri mailUri = new Uri("mailto:aslanhassan98@gmail.com", UriKind.Absolute);
                 OpenBrowser(mailUri);
            };
            siteRecognizer.Tapped += (s, e) =>
            {
                Uri siteUri = new Uri("http://www.itshasanaslan.com", UriKind.Absolute);
                OpenBrowser(siteUri);
            };



            LblTwitter.GestureRecognizers.Add(twitterRecognizer);
            LblInstagram.GestureRecognizers.Add(instagramRecgnizer);
            LblMail.GestureRecognizers.Add(mailRecognizer);
            LblSite.GestureRecognizers.Add(siteRecognizer);

            ImgTwitter.GestureRecognizers.Add(twitterRecognizer);
            ImgInstagram.GestureRecognizers.Add(instagramRecgnizer);
            ImgMail.GestureRecognizers.Add(mailRecognizer);

        }
        private async Task OpenBrowser(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception)
            {
                // An unexpected error occured. No browser may be installed on the device.
            }
        }

    private void InitializeListElements()
        {
            Updates = new List<UpdateCalendarLVModel>();
            Updates.Add(new UpdateCalendarLVModel { Description = "Better image display and thumbnail support." });
            Updates.Add(new UpdateCalendarLVModel { Description = "Performance optimizations." });
            Updates.Add(new UpdateCalendarLVModel { Description = "Gallery view for encrypted media files." });
            Updates.Add(new UpdateCalendarLVModel { Description = "For now, do not encrypt files with their size above 200 mb if your device is not powerful. In future, this will be fixed." });
            }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private void HandleSupport(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddTest());
        }

        public async void PromptBackButton()
        {
            App.sessionManager.Save(App.session);
            string ok = await DisplayActionSheet("Warning", "No", "Yes", "Do you want to close the application?");
            if (ok == "Yes")
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        protected override bool OnBackButtonPressed()
        {

            PromptBackButton();
            return true;
        }
    }
}