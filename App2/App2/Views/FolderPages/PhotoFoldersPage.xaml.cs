using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Models;
using App2.Views.MixedDataPages;

namespace App2.Views.FolderPages
{
   

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoFoldersPage : ContentPage
    {
        ToolbarItem TlbAddfolder = new ToolbarItem { Text = "ADD FOLDER", Order = ToolbarItemOrder.Primary };
        public static bool MultiSelectionEnabled = false;
        public string fileType;
        public PhotoFoldersPage(string fileType)
        {
            InitializeComponent();

            TlbAddfolder.Clicked += AddFolder;
            ToolbarItems.Add(TlbAddfolder);
            this.fileType = fileType;
            Title = fileType + " Folders";

           
            
           
            myCollection.ItemsSource = App.MainDatabase.FoldersSource;
            BindingContext = this;
            myCollection.SelectionChanged += (s, e) =>
            {
                CollectionView_SelectionChanged(s, e);

            };
            


        }
     
        private string GetInfo(AEFFolder folder)
        {
            string temp = "";
            foreach (var prop in folder.GetType().GetProperties())
            {
                AEFFolder item = myCollection.SelectedItem as AEFFolder;
                temp += prop.Name + " : " + prop.GetValue(folder, null) + "\n";
                
            }
            return temp;
        }


        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tempFile = e.CurrentSelection.FirstOrDefault() as AEFFolder;
            if (tempFile == null)
            {
                return;
            }
          ((CollectionView)sender).SelectedItem = null;
            if (!MultiSelectionEnabled)
            {
                 await Navigation.PushAsync(new DataListPage(tempFile));

            }
            else
            {
                //tlbShowHowManySelected.Text = myCollection.SelectedItems.Count.ToString() + " selected";
                //BtnDecryptActions(true);

                if (myCollection.SelectedItems == null)
                {
                    //tlbShowHowManySelected.Text = "";
                    //BtnDecryptActions(false);
                }
            }

        }

        private async void AddFolder(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Create Folder", "Enter a title: ");
            if (result == null || result == "" ||result == " ")
            {
                return;
            }
            AEFFolder newFolder = new AEFFolder
            {
                Title = result,
                DataType = this.fileType, //"Photo",
                ThumbnailPath = "folders_icon.jpg",
                UserId = App.session.UserId,
                Visible = true
            };

            try
            {
                App.generalDataAccess.AddFolder(newFolder);
                AEFFolder retrieved = App.generalDataAccess.FindFolderByUserId(App.session.UserId, result); // MISTAKE.
                //await DisplayAlert("Folder info", GetInfo(retrieved), "ok");
                App.MainDatabase.FoldersSource.Add(retrieved);
                UpdateCollectionView(null, null);
            }
            catch (Exception f)
            {
                await DisplayAlert("Error", f.Message, "Ok");
            }
            
           
        }

        private void UpdateCollectionView(object sender, EventArgs e)
        {
            refreshView.IsRefreshing = true;
            if (fileType != "Generic")
                myCollection.ItemsSource = App.MainDatabase.FoldersSource.Where<AEFFolder>(x => x.DataType == fileType);
            else myCollection.ItemsSource = App.MainDatabase.FoldersSource;
            refreshView.IsRefreshing = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateCollectionView(null, null);
        }
    }
}