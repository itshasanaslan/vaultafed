using App2.Data;
using App2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using App2.ViewModels;
using Xamarin.Forms.Xaml;
using System.IO;
using App2.Views.SystemPages;


#pragma warning disable CS1998, CS4014, CS0414
namespace App2.Views.MixedDataPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilesPage : ContentPage
    {

        public bool MultiSelectionEnabled = false;
        ToolbarItem TlbSelectionMode = new ToolbarItem { Text = "SELECT", Order = ToolbarItemOrder.Primary };
        ToolbarItem TlbSortItems = new ToolbarItem { Text = "Sort", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbSelectAll = new ToolbarItem { Text = "Select All", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbAdPage = new ToolbarItem { Text = "Support", Order = ToolbarItemOrder.Secondary };

        public bool FilesAdded = false;
        public List<string> FilePaths = new List<string>();

        public FilesPage()
        {
            InitializeComponent();
            Init();
            

            Device.StartTimer(TimeSpan.FromSeconds(8), () =>
            {
                if (!FilesAdded) return true;
                FilesAdded = false;
                HandleEncryption();
                return true; // return true to repeat counting, false to stop timer
            });
        }

        private void Init()
        {

            InitializeFileList();
            //Add click events
            TlbSelectionMode.Clicked += ManageSelectionMode;
            TlbSortItems.Clicked += SortItems;
            //TlbSortItems.Clicked += SortItems; I realized I added it twice?

            TlbSelectAll.Clicked += (s, e) => {
                foreach (var i in App.MainDatabase.FilesSource)
                {
                    lstFiles.SelectedItems.Add(i);
                }
                TlbSelectAll.IsEnabled = false;
            };

            TlbSelectAll.IsEnabled = false;

            // Add toolbars.
            ToolbarItems.Add(TlbSelectionMode);
            ToolbarItems.Add(TlbSortItems);
            ToolbarItems.Add(TlbSelectAll);

            if (App.session.CurrentUser.GetHasPurchased() == 0)
            {
                TlbAdPage.Clicked += (s, e) => { Navigation.PushAsync(new AddTest()); };
                ToolbarItems.Add(TlbAdPage);
            }

            BtnDecryptActions(false);
            // set sort items function.
            this.searchBar.TextChanged += (s, e) => FilterItem(searchBar.Text);
            this.searchBar.SearchButtonPressed += (s, e) => FilterItem(searchBar.Text);         

        }

        private void InitializeFileList()
        {
            //update lstview.
            // data kept on App.cs class. If user leaves desktop page and re-opens the app, same files will be added to list twice.
            // To prevent it, check if its initialized.
            if (!App.MainDatabase.fileSourceInitialized)
            {
                foreach (AEF file in App.generalDataAccess.GetAllFile(App.session.CurrentUser.GetId()))
                {

                    if (System.IO.File.Exists(file.OutputName))
                    {
                        App.MainDatabase.AddFile(file);

                    }
                    else
                    {
                        App.MissingFiles.Add(file);
                    }
                }
                App.MainDatabase.fileSourceInitialized = true; // to prevent memory leaks, I need to control this.
                AlertMissingFiles();

            }

            lstFiles.ItemsSource = App.MainDatabase.FilesSource;
            this.SortItemsInner(App.session.FilesSortMethod); //sort items as in the way it was before.
        }

        private async void PickAFile(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != Xamarin.Essentials.PermissionStatus.Granted)
            {
                // get permission
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            string fileFormatToPick = await DisplayActionSheet("File Format", "Cancel", null, "Photo", "Video", "Other");
            IEnumerable<FileResult> pickResult = null; // store selected files.

            switch (fileFormatToPick)
            {
                case "Photo":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick a photo",
                        FileTypes = FilePickerFileType.Images

                    });
                    break;
                case "Video":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick a video",
                        FileTypes = FilePickerFileType.Videos
                    });
                    break;
                case "Other":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick a file",
                    });
                    break;
                default:
                    break;
            }

            ManageAd();
            if (pickResult == null && fileFormatToPick != "Cancel")
            {
                await DisplayAlert("File Error", "No file is chosen!", "OK");
                return;
            }
            if (fileFormatToPick != "Cancel")
            {
                FilesAdded = true;
                BtnFilePicker.IsEnabled = false;
                GridStatusNotifier.IsVisible = true;
                GridStatusNotifier.IsEnabled = true;
                ActivitySpinner.IsRunning = true;
                BtnFilePicker.IsEnabled = false;
                LblStatus.Text = "Generating Encryption methods";
                foreach(var i in pickResult)
                {
                    FilePaths.Add(i.FullPath);
                }
                WarnAboutEncryption();
               
                   
            }
        }

        private async void EncryptFile(string filename)
        {
            AEF file = new AEF(filename, App2.Business.Concretes.ThumbPathFinder.Get());
            try
            {
                LblStatus.Text = "Encrypting " + file.FullName;
                byte[] fileBytes = File.ReadAllBytes(file.OriginalPath);
                if (file.IsPhoto())
                {
                    //generate thumbnail
                   byte[] thumbnailBytes =  await App.imageResizer.ResizeImage(fileBytes);
                    App.imageResizer.SaveThumbnail(file.ThumbPath, thumbnailBytes);
                }
                if (file.Encrypt(fileBytes))
                {
                    file.Save();
                    App.MainDatabase.FilesSource.Add(file);
                    App.generalDataAccess.AddFile(file);
                   
                    file.OutputData.Clear(); // Clear memory.
                }
            }
            catch (Exception f)
            {
                DisplayAlert("Error", "Error logged for file: " + file.FullName + "\nError:" + f.Message, "OK");
            }
        }

        private async Task HandleEncryption()
        {
            foreach (string i in FilePaths)
            {
                await Task.Run(async () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        EncryptFile(i);
                    }
                    );
                });

            }

            //App.userOptions.SaveLocalDB()
            // re-enable the buttons.
            BtnFilePicker.IsEnabled = true;
            ActivitySpinner.IsRunning = false;
            GridStatusNotifier.IsVisible = false;
            GridStatusNotifier.IsEnabled = false;
            LblStatus.Text = "";
            BtnFilePicker.IsEnabled = true;
            FilePaths.Clear();
        }

        private async void AlertMissingFiles()
        {
            if (App.MissingFiles == null) App.MissingFiles = new System.Collections.Generic.List<AEF>();

            if (App.MissingFiles.Count < 1)
            {
                return;
            }

            string warningMessage = "";
            foreach (AEF tempFile in App.MissingFiles)
            {
                warningMessage += tempFile.OutputName + ",\n";
            }

            bool action = await DisplayAlert("Warning", "One or more files are missing. Encrypted files have the prefix and extension of 'afed'. If you moved or renamed those files, undo it. Otherwise you will lose your file. \nFiles Missing:\n" + warningMessage + "If you no longer have those files, I can delete their records.", "Delete records.", "No, I'll fix them.");
            if (action)
            {
                foreach (AEF tempFile in App.MissingFiles)
                {
                    App.generalDataAccess.DeleteFile(tempFile);
                }
                App.MissingFiles.Clear();
                //App.userOptions.SaveLocalDB();
            }
            else
            {
                await DisplayAlert("Missing Files", "You'll get this alert till files are found. If you are successful to relocate those files, you won't see this message.", "OK");
            }

        }

        private async void DecryptFile(object sender, EventArgs e)
        {
            GridStatusNotifier.IsVisible = true;
            GridStatusNotifier.IsEnabled = true;
            ActivitySpinner.IsRunning = true;
            BtnFilePicker.IsEnabled = false;
            BtnDecrypt.IsEnabled = false;
            ManageAd();
            var tempCollection = new ObservableCollection<AEF>();
            foreach (AEF i in lstFiles.SelectedItems)
            {
                tempCollection.Add(i);
            }
            foreach (AEF tempFile in tempCollection)
            {
                if (System.IO.File.Exists(tempFile.OutputName))
                {
                    try
                    {
                        await Task.Run(async () =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                LblStatus.Text = "Decrypting: " + tempFile.FullName;
                                if (tempFile.IsEncrypted)
                                {
                                    tempFile.Decrypt();
                                    tempFile.Save();
                                }

                                tempFile.OutputData.Clear();
                                App.generalDataAccess.DeleteFile(tempFile);
                                App.MainDatabase.RemoveFile(tempFile);
                            }
                            );
                        });
                    }
                    catch (Exception wtf)
                    {
                        await DisplayAlert("Error", "filename: " + tempFile.FullName + "\nLog: " + wtf.Message, "OK");
                        break;
                    }
                }
                else
                {
                    App.MissingFiles.Add(tempFile);
                }
            }

            AlertMissingFiles();


            GridStatusNotifier.IsVisible = false;
            GridStatusNotifier.IsEnabled = false;
            ActivitySpinner.IsRunning = false;
            BtnDecryptActions(false);
            BtnFilePicker.IsEnabled = true;
            LblStatus.Text = "";
            ManageSelectionMode(sender, e);
            //App.userOptions.SaveLocalDB();
            lstFiles.SelectedItems.Clear();
            tlbShowHowManySelected.Text = "";
            TlbSelectionMode.Text = "SELECT";
            lstFiles.SelectionMode = SelectionMode.Single;
        }

        private void BtnDecryptActions(bool value)
        {
            BtnDecrypt.IsEnabled = value;
            BtnDecrypt.IsVisible = value;
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tempFile = e.CurrentSelection.FirstOrDefault() as AEF;
            if (tempFile == null)
            {
                return;
            }
                 ((CollectionView)sender).SelectedItem = null;
            if (!MultiSelectionEnabled)
            {
                if (System.IO.File.Exists(tempFile.OutputName))
                {
                    await Navigation.PushAsync(new FileInfoPage(tempFile));
                }
                else
                {
                    App.MissingFiles.Add(tempFile);
                    await DisplayAlert("Warning", "This file is missing. You probably deleted, moved or renamed it.", "OK");
                }


            }
            else

            {
                tlbShowHowManySelected.Text = lstFiles.SelectedItems.Count.ToString() + " selected";
                BtnDecryptActions(true);

                if (lstFiles.SelectedItems == null)
                {
                    tlbShowHowManySelected.Text = "";
                    BtnDecryptActions(false);
                }
            }
        }

        private void ManageSelectionMode(object sender, EventArgs e)
        {
            MultiSelectionEnabled = !MultiSelectionEnabled;
            if (MultiSelectionEnabled)
            {
                TlbSelectionMode.Text = "CANCEL";
                lstFiles.SelectionMode = SelectionMode.Multiple;
                TlbSelectAll.IsEnabled = true;
            }
            else
            {
                BtnDecryptActions(false);
                TlbSelectionMode.Text = "SELECT";
                TlbSelectAll.IsEnabled = false;
                lstFiles.SelectionMode = SelectionMode.Single;
                tlbShowHowManySelected.Text = "";
                lstFiles.SelectedItems.Clear();

            }
        }

        private async void SortItems(object sender, EventArgs e)
        {
                string action = await DisplayActionSheet("Sort by", "Cancel", null,
                "Added Time (Now-Past)",
                "Added Time (Past-Now)",
                "File Name (A-Z)",
                "File Name (Z-A)",
                "File Size(Bigger-Smaller)",
                "File Size(Smaller-Bigger)",
                "File Format (A-Z)",
                "File Format (Z-A)");

            SortItemsInner(action);


        }

        private void SortItemsInner(string action)
        {

            switch (action)
            {
                case "Added Time (Now-Past)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.ActualAddedTime);
                    break;
                case "Added Time (Past-Now)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.ActualAddedTime);
                    break;
                case "File Name (A-Z)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.FullName);
                    break;
                case "File Name (Z-A)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.FullName);
                    break;
                case "File Size(Bigger-Smaller)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.ActualFileSize);
                    break;
                case "File Size(Smaller-Bigger)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.ActualFileSize);
                    break;
                case "File Format (A-Z)":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.OriginalExtension);
                    break;
                case "File Format (Z-A))":
                    lstFiles.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.OriginalExtension);
                    break;

                default:
                    break;
            }
            App.session.FilesSortMethod = action;
        }

        private void UpdateCollectionView(object sender, EventArgs e)
        {
            lstFiles.ItemsSource = App.MainDatabase.FilesSource;
            refreshView.IsRefreshing = false;
        }

        private void FilterItem(string text)
        {
            refreshView.IsRefreshing = true;
            if (string.IsNullOrWhiteSpace(text))
            {
                lstFiles.ItemsSource = App.MainDatabase.FilesSource;
            }
            else
            {
                lstFiles.ItemsSource = App.MainDatabase.FilesSource.Where(x => x.FullName.ToLower().Contains(text.ToLower()));
            }
            refreshView.IsRefreshing = false;
        }

        private async void WarnAboutEncryption()
        {
            if (!App.session.HasEncryptedBefore)
            {
                App.session.HasEncryptedBefore = true;
                bool agreed = false;

                do
                {
                    agreed = await DisplayAlert("Warning!", "Selected files have been renamed with the 'afed' prefix and suffix. If you see any file has 'afed' prefix or suffix, do not edit or delete it. Otherwise, you will lose the file.", "I'll be careful.", "No");
                } while (!agreed);

                do
                {
                    agreed = await DisplayAlert("Warning!", "Please do not leave the application when doing encryption/decryption, especially with the files have large sizes.. Otherwise, you will lose the file.", "I'll be careful.", "No");
                } while (!agreed);
            }
            App.sessionManager.Save(App.session);
        }

        private void ManageAd()
        {
            if (App.session.CurrentUser.GetHasPurchased() == 0)
            {
                DependencyService.Get<IAdInterstitial>().ShowAd();
            } 
        }

        


    }
}