using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using App2.Models;
using Xamarin.Forms.Xaml;
using App2.Views.MixedDataPages;
using Xamarin.Essentials;
using System.IO;
using System.Collections.ObjectModel;
using App2.ViewModels;

namespace App2.Views.MixedDataPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataListPage : ContentPage
    {
        public static AEFFolder rootFolder;
        public bool MultiSelectionEnabled = false;
        ToolbarItem TlbPickAFile = new ToolbarItem { Text = "Add", Order = ToolbarItemOrder.Primary };
        ToolbarItem TlbSelectionMode = new ToolbarItem { Text = "SELECT", Order = ToolbarItemOrder.Primary };
        ToolbarItem TlbSortItems = new ToolbarItem { Text = "Sort", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbSelectAll = new ToolbarItem { Text = "Select All", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbDeleteFolder = new ToolbarItem { Text = "Delete Folder", Order = ToolbarItemOrder.Secondary };
        public bool FilesAdded = false;
        public List<string> FilePaths = new List<string>();


        public DataListPage(AEFFolder rootFolder)
        {
            InitializeComponent();
            Title = rootFolder.Title;
            DataListPage.rootFolder = rootFolder;
            BtnDecryptActions(false);
           
            BindingContext = this;
            myCollection.SelectionChanged += (s, e) => {
                CollectionView_SelectionChanged(s, e);
            };
            TlbSelectionMode.Clicked += ManageSelectionMode;
            TlbSortItems.Clicked += SortItems;
            TlbPickAFile.Clicked += PickAFile;
            TlbDeleteFolder.Clicked += DeleteFolder;
            //TlbSortItems.Clicked += SortItems; I realized I added it twice?

            TlbSelectAll.Clicked += (s, e) => {
                foreach (var i in App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()))
                {
                    myCollection.SelectedItems.Add(i);
                }
                TlbSelectAll.IsEnabled = false;
            };

            TlbSelectAll.IsEnabled = false;

            // Add toolbars.
            ToolbarItems.Add(TlbPickAFile);
            ToolbarItems.Add(TlbSelectionMode);
            ToolbarItems.Add(TlbSortItems);
            ToolbarItems.Add(TlbSelectAll);
            ToolbarItems.Add(TlbDeleteFolder);

            UpdateCollectionView(null, null);
            Device.StartTimer(TimeSpan.FromSeconds(8), () =>
            {
                if (!FilesAdded) return true;
                FilesAdded = false;
                HandleEncryption();
                return true; // return true to repeat counting, false to stop timer
            });

        }

        private void GetFiles()
        {
            switch (rootFolder.DataType)
            {
                case "Photo":
                    if (App.session.PhotosSortMethod == null || App.session.PhotosSortMethod == "" || App.session.PhotosSortMethod == string.Empty)
                    {
                        myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto());
                        return;
                    }
                    SortItemsInner(App.session.PhotosSortMethod);
                    break;
                case "Video":
                    if (App.session.VideosSortMethod == null || App.session.VideosSortMethod == "" || App.session.VideosSortMethod == string.Empty)
                    {
                        myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo());
                        return;
                    }
                    SortItemsInner(App.session.VideosSortMethod);
                    break;
                case "Audio":
                    if (App.session.AudiosSortMethod == null || App.session.AudiosSortMethod == "" || App.session.AudiosSortMethod == string.Empty)
                    {
                        myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio());
                        return;
                    }
                    SortItemsInner(App.session.AudiosSortMethod);
                    break;
                case "Generic":
                    if (App.session.FilesSortMethod == null || App.session.FilesSortMethod == "" || App.session.FilesSortMethod == string.Empty)
                    {
                        myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId);
                        return;
                    }
                    SortItemsInner(App.session.FilesSortMethod);
                    break;
                default:
                    DisplayAlert("33", "Not set yet", "ok");
                    return;
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
            "File Size(Smaller-Bigger)");

            SortItemsInner(action);


        }

        private void SortItemsInner(string action)
        {
            switch (rootFolder.DataType)
            {
                case "Photo":
                    switch (action)
                    {
                        case "Added Time (Now-Past)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderByDescending(x => x.ActualAddedTime);
                            break;
                        case "Added Time (Past-Now)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderBy(x => x.ActualAddedTime);
                            break;
                        case "File Name (A-Z)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderBy(x => x.FullName);
                            break;
                        case "File Name (Z-A)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderByDescending(x => x.FullName);
                            break;
                        case "File Size(Bigger-Smaller)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderByDescending(x => x.ActualFileSize);
                            break;
                        case "File Size(Smaller-Bigger)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsPhoto()).OrderBy(x => x.ActualFileSize);
                            break;
                        default:
                            DisplayAlert("a", "", "aa");
                            break;
                    }
                    App.session.PhotosSortMethod = action;
                    break;
                case "Video":
                    switch (action)
                    {
                        case "Added Time (Now-Past)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderByDescending(x => x.ActualAddedTime);
                            break;
                        case "Added Time (Past-Now)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderBy(x => x.ActualAddedTime);
                            break;
                        case "File Name (A-Z)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderBy(x => x.FullName);
                            break;
                        case "File Name (Z-A)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderByDescending(x => x.FullName);
                            break;
                        case "File Size(Bigger-Smaller)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderByDescending(x => x.ActualFileSize);
                            break;
                        case "File Size(Smaller-Bigger)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsVideo()).OrderBy(x => x.ActualFileSize);
                            break;
                        default:
                            DisplayAlert("a", "", "aa");
                            break;
                    }

                    App.session.VideosSortMethod = action;
                    break;
                case "Audio":

                    switch (action)
                    {
                        case "Added Time (Now-Past)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderByDescending(x => x.ActualAddedTime);
                            break;
                        case "Added Time (Past-Now)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderBy(x => x.ActualAddedTime);
                            break;
                        case "File Name (A-Z)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderBy(x => x.FullName);
                            break;
                        case "File Name (Z-A)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderByDescending(x => x.FullName);
                            break;
                        case "File Size(Bigger-Smaller)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderByDescending(x => x.ActualFileSize);
                            break;
                        case "File Size(Smaller-Bigger)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.Where<AEF>(x => x.FolderId == rootFolder.FolderId).Where<AEF>(x => x.IsAudio()).OrderBy(x => x.ActualFileSize);
                            break;
                        default:
                            DisplayAlert("a", "", "aa");
                            break;
                    }
                    App.session.AudiosSortMethod = action;
                    break;
                case "Generic":
                    switch (action)
                    {
                        case "Added Time (Now-Past)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.ActualAddedTime);
                            break;
                        case "Added Time (Past-Now)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.ActualAddedTime);
                            break;
                        case "File Name (A-Z)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.FullName);
                            break;
                        case "File Name (Z-A)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.FullName);
                            break;
                        case "File Size(Bigger-Smaller)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.ActualFileSize);
                            break;
                        case "File Size(Smaller-Bigger)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.ActualFileSize);
                            break;
                        case "File Format (A-Z)":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderBy(x => x.OriginalExtension);
                            break;
                        case "File Format (Z-A))":
                            myCollection.ItemsSource = App.MainDatabase.FilesSource.OrderByDescending(x => x.OriginalExtension);
                            break;

                        default:
                            break;
                    }
                    App.session.FilesSortMethod = action;
                    break;
                default:
                     DisplayAlert("34", "Not set", "Ok");
                    break;
            }
            App.sessionManager.Save(App.session);

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
                    
                    //DisplayAlert("Thumb test on datalistpage", tempFile.ThumbPath, "ok");
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
                tlbShowHowManySelected.Text = myCollection.SelectedItems.Count.ToString() + " selected";
                BtnDecryptActions(true);

                if (myCollection.SelectedItems == null)
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
                myCollection.SelectionMode = SelectionMode.Multiple;
                TlbSelectAll.IsEnabled = true;
            }
            else
            {
                BtnDecryptActions(false);
                TlbSelectionMode.Text = "SELECT";
                TlbSelectAll.IsEnabled = false;
                myCollection.SelectionMode = SelectionMode.Single;
                tlbShowHowManySelected.Text = "";
                myCollection.SelectedItems.Clear();

            }
        }

        private void BtnDecryptActions(bool value)
        {
            BtnDecrypt.IsEnabled = value;
            BtnDecrypt.IsVisible = value;
        }

        private async void PickAFile(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != Xamarin.Essentials.PermissionStatus.Granted)
            {
                // get permission
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            string fileFormatToPick = "Photo";
            IEnumerable<FileResult> pickResult;
            //await DisplayAlert("filetype", rootFolder.DataType, "ok");

            switch (rootFolder.DataType)
            {
                case "Photo":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick photos",
                        FileTypes = FilePickerFileType.Images
                    });
                    break;
                case "Video":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick videos",
                        FileTypes = FilePickerFileType.Videos
                    });
                    break;
                case "Audio":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick audios",
                    }) ; ;
                    break;
                case "Generic":
                    pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        PickerTitle = "Pick files"
                    }); ;
                    break;
                default:
                    await DisplayAlert("35", "Wrong folder type", "Ok");
                    return;
            }

     
            ManageAd();
            if (pickResult == null)
            {
                DependencyService.Get<IMessage>().LongAlert("No file chosen! ");

                return;
            }
            if (fileFormatToPick != "Cancel")
            {
                FilesAdded = true;
                TlbPickAFile.IsEnabled = false;
                GridStatusNotifier.IsVisible = true;
                GridStatusNotifier.IsEnabled = true;
                ActivitySpinner.IsRunning = true;
                LblStatus.Text = "Generating Encryption methods";
                foreach (var i in pickResult)
                {
                    if (rootFolder.DataType == "Audio" && !ValidateAudioFormat(Path.GetExtension(i.FullPath))) 
                    {
                        await DisplayAlert("File type error", "Selected file is not an audio file: \n" + i.FullPath, "ok");
                        continue; 
                    }
                    FilePaths.Add(i.FullPath);
                }
                WarnAboutEncryption();


            }
        }
        private bool ValidateAudioFormat(string extension)
        {
            string[] audioTypes = { ".m4a", ".ogg", ".mp3" };
            foreach (string i in audioTypes)
            {
                if (i == extension) return true;
            }
            return false;
        }

        private void ManageAd()
        {
            if (App.session.CurrentUser.GetHasPurchased() == 0)
            {
                DependencyService.Get<IAdInterstitial>().ShowAd();
            }

        }

        private void WarnAboutEncryption(){ }

        private async void EncryptFile(string filename)
        {
            AEF file = new AEF(filename, App2.Business.Concretes.ThumbPathFinder.Get());
            try
            {
                file.FolderId = DataListPage.rootFolder.FolderId;
                LblStatus.Text = "Encrypting " + file.FullName;
                byte[] fileBytes = File.ReadAllBytes(file.OriginalPath);
                if (file.IsPhoto())
                {
                    //generate thumbnail
                    byte[] thumbnailBytes = await App.imageResizer.ResizeImage(fileBytes);
                    App.imageResizer.SaveThumbnail(file.ThumbPath, thumbnailBytes);
                }
                else
                {
                    if (file.IsVideo()) file.ThumbPath = "icon_video.png";
                    else if (file.IsAudio()) file.ThumbPath = "icon_audio.png";
                    else
                    {
                        file.ThumbPath = "icon_all_files.png";
                    }

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
            ActivitySpinner.IsRunning = false;
            GridStatusNotifier.IsVisible = false;
            GridStatusNotifier.IsEnabled = false;
            LblStatus.Text = "";
            TlbPickAFile.IsEnabled = true;
            UpdateCollectionView(null, null);
            FilePaths.Clear();
            
        }

        private void UpdateCollectionView(object sender, EventArgs e)
        {
            refreshView.IsRefreshing = true;
            GetFiles();
            refreshView.IsRefreshing = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateCollectionView(null, null);
        }

        private async void DecryptFile(object sender, EventArgs e)
        {
            GridStatusNotifier.IsVisible = true;
            GridStatusNotifier.IsEnabled = true;
            ActivitySpinner.IsRunning = true;
            TlbPickAFile.IsEnabled = false;
            BtnDecrypt.IsEnabled = false;
            ManageAd();
            var tempCollection = new ObservableCollection<AEF>();
            foreach (AEF i in myCollection.SelectedItems)
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
            TlbPickAFile.IsEnabled = true;
            LblStatus.Text = "";
            ManageSelectionMode(sender, e);
            //App.userOptions.SaveLocalDB();
            myCollection.SelectedItems.Clear();
            tlbShowHowManySelected.Text = "";
            TlbSelectionMode.Text = "SELECT";
            myCollection.SelectionMode = SelectionMode.Single;
            UpdateCollectionView(null, null);
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

        private async void DeleteFolder(object sender, EventArgs e)
        {
            string msg = "This will delete all files in this folder. You will no longer access them. \n\n\tPlease write this code to confirm:\n\n";

            string code = AEF.GenerateRandomName(8).Substring(4);
            msg += code;
            string result = await DisplayPromptAsync("Delete Folder- Warning", msg);
            if (result == null || result == "" || result == " ")
            {
                return;
            }

            if (result != code)
            {
                await DisplayAlert("Code", "Code is not correct. Operation cancelled.", "OK");
                return;
            }
            DependencyService.Get<IMessage>().LongAlert("This may take a while. Please wait....");
            ActivitySpinner.IsRunning = true;
            TlbPickAFile.IsEnabled = false;
            List<AEF> temp = new List<AEF>();
            
            foreach(AEF file in myCollection.ItemsSource)
            {
                temp.Add(file);
            }
            
            foreach(AEF file in temp)
            {
                App.generalDataAccess.DeleteFile(file);
                try
                {
                    System.IO.File.Delete(file.OutputName);
                    File.Delete(file.ThumbPath);
                    App.generalDataAccess.DeleteFile(file);
                    App.MainDatabase.FilesSource.Remove(file);

                }
                catch (FileNotFoundException) { }
                catch (Exception f)
                {
                    DisplayAlert("Error", file.OutputName + "\n" + f.Message, "ok");
                }
            }

            ActivitySpinner.IsRunning = false;
            App.generalDataAccess.DeleteFolder(rootFolder);
            App.MainDatabase.FoldersSource.Remove(rootFolder);
            DependencyService.Get<IMessage>().LongAlert("Deleted successfully....");
            await Navigation.PopAsync();
        }

    }
}