using App2.Data;
using App2.Models;
using App2.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("Lobster-Regular.ttf", Alias = "Lobster")]
#pragma warning disable CS1998, CS4014
namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileInfoPage : ContentPage
    {
        public static bool IsPortrait(Page p) { return p.Width < p.Height; }
        bool IsImage = false;
        AEF file;
        ActivityIndicator spinner = new ActivityIndicator { IsRunning = true };
        Button btnPlayVideo;


        public FileInfoPage(AEF file)
        {
            this.file = file; // make it global
            InitializeComponent();
            this.BackgroundColor = Constants.BackgroundColor;
            this.BindingContext = file;
            
            if (file.IsPhoto())
            {
                // inner function automatically checks if it is on stream or not.
                file.CopyToStream();
                var stream1 = new MemoryStream(file.OutputData.ToArray());
                ImgMain.Source = ImageSource.FromStream(() => stream1);
                // if i want  to add performance mode in the future, to this if the mode is enabled.
                // It will clean the output once the photo is streamed.
                //file.OutputData.Clear();
                IsImage = true;
                HandleCache();
            }

            else if(file.IsVideo())
            {
                btnPlayVideo =  new Button
                {
                    Text = "Play",
                    BackgroundColor = Color.Red,
                    TextColor = Color.White,
                };

                btnPlayVideo.Clicked += (s, e) => NavigateToVideoPage(s, e);
                VideoDecryption();
            }

            // Nondisplayable media
            else
            {
                DisplayStatus($"\nSorry, the '{file.OriginalExtension}' file cannot be displayed.\n", false); 
            }

        }


        private void HandleInfoClicked(object sender, EventArgs e)
        {
            DisplayAlert("Info", file.GetFileInfo() ,"OK");
        }

        private async void HandleDecrypt(object sender, EventArgs e)
        {
            tlbDecrypt.IsEnabled = false;
            await DisplayStatus("Decrypting file.", true);

            ManageAds();
            await Task.Run(async () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    file.Decrypt();
                    file.Save();
                    file.OutputData.Clear();
                    App.userOptions.RemoveFile(file.OriginalPath);
                    App.MainDatabase.RemoveFile(file);
                    App.userOptions.SaveLocalDB();
                    DisplayAlert("File", "File is decrypted and saved to " + file.OriginalPath, "OK");
                    Navigation.PopAsync();
                }
                );
            });

        }

        private async void VideoDecryption()
        {
            tlbDecrypt.IsEnabled = false;
            await DisplayStatus("Decrypting video to show", true);
            await Task.Factory.StartNew(() => { file.CopyToStream(); });
            //await DisplayAlert("Video", "Decrypted to stream", "OK");
            DependencyService.Get<IMessage>().ShortAlert("Decrypted!");
            tlbDecrypt.IsEnabled = true;
            await InitializeVideoScreen();
            HandleCache();
            //NavigateToVideoPage2();
            spinner.IsRunning = false;
           
            
            
        }

        async Task DisplayStatus(string message, bool activity)
        {
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                Children = {
                new BoxView { Color = Color.Red },
                new BoxView { Color = Color.Yellow },
                new BoxView { Color = Color.Blue },

                new Label {
                    Text = message,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    FontFamily="Lobster-Regular",
                    TextColor = Color.Black
            },
                new ActivityIndicator{IsRunning = activity, Color = Color.Red},
                new BoxView { Color = Color.Green },
                new BoxView { Color = Color.Orange },
                new BoxView { Color = Color.Purple }
            }
            };
        }

        async Task InitializeVideoScreen()
        {
           
            Content = new StackLayout
            {
                Padding = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                Children = {
                    spinner,
                      new Label {
                    Text = "Video is ready to display.",
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    FontFamily="Lobster-Regular",
                    TextColor = Color.Red
            },
                      btnPlayVideo
            }
            };
        }

        private void NavigateToVideoPage(object sender, EventArgs e)
        {
            NavigateToVideoPage2();
        }

        private async void NavigateToVideoPage2()
        {
            btnPlayVideo.IsEnabled = false;
            spinner.IsRunning = true;
            await Navigation.PushAsync(new VideoPlayPage(this.file));
            btnPlayVideo.IsEnabled = true;
            spinner.IsRunning = false;
        }

        async void HandleCache()
        {
            
            if (AEF.CacheUsageAtTheMoment > App.userOptions.MaxCacheUsage)
            {
                bool action = await DisplayAlert("Performance", "Memory usage has reached to: " + (AEF.CacheUsageAtTheMoment / 1000000).ToString() + " megabytes. Program will clean the cache. Would you like to clean this file's too?.", "Yes", "No");

                AEF.CacheUsageAtTheMoment = 0;
                foreach (AEF i in App.FilesOnCache)
                {
                    if (i == this.file && !action)
                    {
                        continue;
                    }
                    i.OutputData.Clear();
                    i.OnStream = false;
                }
                if(action)
                {
                    await Navigation.PopToRootAsync();
                }

            }
            if (!App.FilesOnCache.Contains(this.file))
            {
                AEF.CacheUsageAtTheMoment += this.file.OutputData.Count();
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
           
            base.OnSizeAllocated(width, height);
            //DisplayAlert("Is portrait", IsPortrait(this).ToString(),"OK");
            if (IsPortrait(this))
            {

               // NavigationPage.SetHasNavigationBar(this, true);
              
            }
            else
            {
                //NavigationPage.SetHasNavigationBar(this, false);
    

            }


        }

        private void ManageAds()
        {
            if (UserDatabaseController.CurrentUser.hasPurchased == 0)
            {
                Random r = new Random();
                int x = r.Next(0, 2);
                if (x == 1)
                DependencyService.Get<IAdInterstitial>().ShowAd();
            }
        }
       
    }
}