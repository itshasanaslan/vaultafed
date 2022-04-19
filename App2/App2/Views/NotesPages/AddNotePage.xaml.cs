using App2.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace App2.Views.NotesPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNotePage : ContentPage
    {
        public Notes note;
        public bool modifying = false;
        public AddNotePage()
        {
            InitializeComponent();
            Init();
            this.Title = "Add a Note";
        }
        public AddNotePage(Notes note)
        {
            InitializeComponent();
            this.note = note;
            this.Title = "Edit Note";
            Init();
            EntryTitle.Text = note.Title;
            EntryContent.Text = note.Content;
            modifying = true;
        }

        public void Init()
        {
          
            EntryTitle.Focus();
            EntryTitle.Completed += (s, e) => EntryContent.Focus();
            //EntryContent.Completed += (s, e) => HandleSave(s, e);
        }

        public async void HandleSave(object sender, EventArgs e)
        {
            if (EntryTitle.Text == null || EntryContent.Text == null)
            {
                await DisplayAlert("Error", "Please fill in the blanks!", "OK");
                return;
            }

            if (!this.modifying)
            {
               Notes temp = new Notes(EntryTitle.Text, EntryContent.Text);
                App.MainDatabase.AddNote(temp);
               ServerResponse response =  App.generalDataAccess.AddNote(temp);
                if (!response.OperationSuccessful)
                {
                    await DisplayAlert(response.OperationType, response.ServerMessage, "Ok");
                }
                
              
            }
            else
            {

                //App.MainDatabase.RemoveNote(this.note);

                try {
                    this.note.Content = EntryContent.Text;
                    this.note.Title = EntryTitle.Text;
                    App.MainDatabase.AddNote(this.note);
                    ServerResponse response = App.generalDataAccess.UpdateNote(this.note);
                    if (!response.OperationSuccessful)
                    {
                        await DisplayAlert(response.OperationType, response.ServerMessage, "OK");
                    }
               
                }
                catch(Exception f)
                {
                    await DisplayAlert("Error", f.Message, "ok");
                }
               
            }
            try
            {
               ServerResponse response = App.sessionManager.Save(App.session);
                //await DisplayAlert(response.OperationType, response.ServerMessage, "ok2");
                await Navigation.PopAsync(true);

            }
            catch (Exception f)
            {
                await DisplayAlert("Error", f.Message, "OK");
            }

        }

        private async void HandleCancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void EntryContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EntryContent.Text == null || EntryContent.Text.Length == 0)
            {
                EntryContent.BackgroundColor = Color.White;
            }
            else
            {
                EntryContent.BackgroundColor = Color.LightYellow;
            }
        }
    }
}