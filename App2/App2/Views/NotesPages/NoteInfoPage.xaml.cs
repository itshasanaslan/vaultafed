using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App2.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace App2.Views.NotesPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteInfoPage : ContentPage
    {
        public Notes note;
        public NoteInfoPage(Notes note)
        {
            InitializeComponent();
            this.note = note;
            LblTitle.Text = note.Title;
            LblContent.Text = note.Content;
            LblAddedTime.Text = note.AddedTime;
            this.Title = note.Title;
         
            }

        public void HandleDelete(object sender, EventArgs e)
        {
            App.MainDatabase.RemoveNote(this.note);
            App.generalDataAccess.DeleteNote(this.note.Id);
            Navigation.PopAsync();
        }

        public async void HandleEdit(object sender, EventArgs e)
        {
           
            await Navigation.PushAsync(new AddNotePage(this.note));
        }
        async void HandleCopy(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(LblContent.Text);
            DependencyService.Get<IMessage>().LongAlert("Content has been copied to clipboard.");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LblTitle.Text = note.Title;
            LblContent.Text = note.Content;
            LblAddedTime.Text = note.AddedTime;
            this.Title = note.Title;
        }
    }
}


