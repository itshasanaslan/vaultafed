using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#pragma warning disable CS0618, CS1998
namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPages : MasterDetailPage
    {
        public MasterPages()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
           
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPagesMasterMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;
            Detail = new NavigationPage(page);

            /*
            switch(item.Id)
            {
                case 0:
                    await Detail.Navigation.PopToRootAsync();
                    
                    break;
                case 2:
                    await Detail.Navigation.PushAsync(new ProfilePage());
                    //Detail = new NavigationPage(new ProfilePage());
                    break;
                case 3:
                    await Detail.Navigation.PushAsync(new AboutPage());
                    //Detail = new NavigationPage(new AboutPage());
                    break;
                case 4:
                    await Detail.Navigation.PushAsync(new RemoveAdsPage());
                    break;
                default:
                    await DisplayAlert("error 15", "switch statement", "OK");
                    break;
            }
            */
            IsPresented = false;
            MasterPage.ListView.SelectedItem = null;
            
        }

     
    

    }
}