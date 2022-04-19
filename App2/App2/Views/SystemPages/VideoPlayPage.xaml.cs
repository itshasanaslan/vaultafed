using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App2.Models;
using Octane.Xamarin.Forms.VideoPlayer;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views.SystemPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPlayPage : ContentPage
    {
        public static bool IsPortrait(Page p) { return p.Width < p.Height; }
        public VideoPlayPage(AEF file)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.BindingContext = file;
            var stream1 = new MemoryStream(file.OutputData.ToArray());
            VideoToShow.Source = VideoSource.FromStream( () => stream1, file.OriginalExtension);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            //DisplayAlert("Is portrait", IsPortrait(this).ToString(),"OK");
            if (IsPortrait(this))
            {
                VideoToShow.VerticalOptions = LayoutOptions.Center;
                VideoToShow.HorizontalOptions = LayoutOptions.Center;
                //NavigationPage.SetHasNavigationBar(this, true);
            }
            else
            {
                //NavigationPage.SetHasNavigationBar(this, false);
                VideoToShow.VerticalOptions = LayoutOptions.FillAndExpand;
                VideoToShow.HorizontalOptions = LayoutOptions.FillAndExpand;
            }

            
        }

    }
}