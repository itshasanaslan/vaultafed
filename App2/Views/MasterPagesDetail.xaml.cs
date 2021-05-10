using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPagesDetail : Xamarin.Forms.TabbedPage
    {
        public MasterPagesDetail()
        {
            InitializeComponent();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            

            if (App.userOptions.VeryNewUser)
            {
                IntroduceOffLineMode();
            }
          
        }

        private async void IntroduceOffLineMode()
        {
            App.userOptions.VeryNewUser = false;
            bool action = await DisplayAlert("Welcome!", "You don't have to connect to server every time you want to log in. You can set an offline password. Would you like to be navigated to set offline password page?", "Yes", "No");
            if (action)
            {
                await Navigation.PushAsync(new ProfilePage());
            }

        }

        


        public async void PromptBackButton()
        {
            App.userOptions.SaveLocalDB();
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