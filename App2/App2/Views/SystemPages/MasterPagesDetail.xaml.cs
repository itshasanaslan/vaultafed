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
using App2.Views.NotesPages;
using App2.Views.MixedDataPages;
using App2.Views.PasswordPages;
using App2.Views.ProfilePages;
using App2.Views.FolderPages;
using App2.Models;

namespace App2.Views.SystemPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPagesDetail : ContentPage
    {
        public static bool isBusy = true;
        public MasterPagesDetail()
        {
            InitializeComponent();
            InitializeGestureRecognizers();
            // On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
          
            if (App.session.VeryNewUser)
            {
                IntroduceOffLineMode();
            }

            activitySpinner.IsRunning = true;
            InitializeFileList();
            InitializeFolders();
            DependencyService.Get<IMessage>().LongAlert("Welcome " + App.session.CurrentUser.name);



        }

        private void InitializeGestureRecognizers()
        {
            var imgClickPhoto = new TapGestureRecognizer();
            var imgClickVideo = new TapGestureRecognizer();
            var imgClickAudio = new TapGestureRecognizer();

            var imgClickPassword = new TapGestureRecognizer();
            var imgClickVideoGesture = new TapGestureRecognizer();
            var imgClickAudioGesture = new TapGestureRecognizer();
            var imgClickNotes = new TapGestureRecognizer();
            var imgMixedData = new TapGestureRecognizer();


            imgClickPhoto.Tapped += (s, e) => SwitchPage(0);
            imgClickPassword.Tapped += (s, e) => SwitchPage(4);
            imgClickNotes.Tapped += (s, e) => SwitchPage(5);
            imgMixedData.Tapped += (s, e) => SwitchPage(3);
            imgClickAudioGesture.Tapped += (s, e) => SwitchPage(2);
            imgClickVideoGesture.Tapped += (s, e) => SwitchPage(1);

            ImgPhotos.GestureRecognizers.Add(imgClickPhoto);
            ImgNotes.GestureRecognizers.Add(imgClickNotes);
            ImgPasswords.GestureRecognizers.Add(imgClickPassword);
            ImgGenericData.GestureRecognizers.Add(imgMixedData);
            ImgAudios.GestureRecognizers.Add(imgClickAudioGesture);
            ImgVideos.GestureRecognizers.Add(imgClickVideoGesture);
            
        }

        private async void SwitchPage(int id)
        {
            if (isBusy)
            {
                DisplayAlert("New Launch", "Please wait while handling data...", "Ok");
                return;
            }
            activitySpinner.IsRunning = true;
            switch (id)
            {
                case 0:
                    await Navigation.PushAsync(new PhotoFoldersPage("Photo"));
                    break;
                case 1:
                    await Navigation.PushAsync(new PhotoFoldersPage("Video"));
                    break;
                case 2:
                    await Navigation.PushAsync(new PhotoFoldersPage("Audio"));
                    break;
                case 3:
                    await Navigation.PushAsync(new PhotoFoldersPage("Generic"));
                    break;
                case 4:
                    await Navigation.PushAsync(new PasswordsPage());
                    break;
                case 5:
                    await Navigation.PushAsync(new NotesPage());
                    break;
                default:
                    throw new Exception("HASAN, BE CAREFUL");
            }
                    activitySpinner.IsRunning = false;


            }
        

        private async void IntroduceOffLineMode()
        {
            App.session.VeryNewUser = false;
            App.sessionManager.Update(App.session);
            bool action = await DisplayAlert("Welcome!", "You don't have to connect to server every time you want to log in. You can set an offline password. Would you like to be navigated to set offline password page?", "Yes", "No");
            if (action)
            {
                await Navigation.PushAsync(new ProfilePage());
            }

        }

        


        public async void PromptBackButton()
        {
            App.sessionManager.Update(App.session);
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


        private async void InitializeFileList()
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
                activitySpinner.IsRunning = false;
                isBusy = false;
            }

            
        }

        private async void InitializeFolders()
        {
            int userId = App.session.UserId;
            if (!App.MainDatabase.folderSourceInitialized)
            {
                foreach(var i in App.generalDataAccess.GetAllFolder(userId))
                {
                    App.MainDatabase.FoldersSource.Add(i);
                }
                App.MainDatabase.folderSourceInitialized = true;
            }
        }

        private void Calculate()
        {
            DependencyService.Get<IMessage>().ShortAlert("Calculating...");
            activitySpinner.IsRunning = true;
            int photoFolders = App.MainDatabase.FoldersSource.Where<AEFFolder>(x => x.DataType == "Photo").Count();
            int photosCount = App.MainDatabase.FilesSource.Where<AEF>(x => x.IsPhoto()).Count();
            lblPhotosInfo.Text = photoFolders.ToString() + " folders, " + photosCount.ToString() + " photos";

            int videoFolders = App.MainDatabase.FoldersSource.Where<AEFFolder>(x => x.DataType == "Video").Count();
            int videoCount = App.MainDatabase.FilesSource.Where<AEF>(x => x.IsVideo()).Count();
            lblVideosInfo.Text = videoFolders.ToString() + " folders, " + videoCount.ToString() + " videos";


            int audioFolders = App.MainDatabase.FoldersSource.Where<AEFFolder>(x => x.DataType == "Audio").Count();
            int audioCount = App.MainDatabase.FilesSource.Where<AEF>(x => x.IsAudio()).Count();
            lblAudiosInfo.Text = audioFolders.ToString() + " folders, " + audioCount.ToString() + " audios";

            int genericFolders = photoFolders + videoFolders + audioFolders;
            int genericCount = photosCount + videoCount + audioCount;
            lblGenericInfo.Text = genericFolders.ToString() + " folders, " + genericCount.ToString() + " files";


            int notesCount = App.MainDatabase.NotesSource.Count();
            lblNotesInfo.Text = notesCount.ToString() + " notes";

            int passwordsCount = App.MainDatabase.PasswordsSource.Count();
            lblPasswordsInfo.Text = passwordsCount.ToString() + " passwords";

            activitySpinner.IsRunning = false;
            DependencyService.Get<IMessage>().ShortAlert("Calculated...");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Calculate();
        }

    }
}