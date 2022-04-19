using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


#pragma warning disable CS1998, CS4014, CS0414
namespace App2.Views.NotesPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : ContentPage
    {

        public bool MultiSelectionEnabled = false;
        ToolbarItem TlbSelectionMode = new ToolbarItem { Text = "SELECT", Order = ToolbarItemOrder.Primary };
        ToolbarItem TlbSortItems = new ToolbarItem { Text = "Sort", Order = ToolbarItemOrder.Secondary };
        ToolbarItem TlbSelectAll = new ToolbarItem { Text = "Select All", Order = ToolbarItemOrder.Secondary };
       

        public NotesPage()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            //update lstview.
            if (!App.MainDatabase.noteSourceInitialized) // Since the data kept on App.cs, 
            {
                App.MainDatabase.NotesSource = new System.Collections.ObjectModel.ObservableCollection<Notes>();
               
                foreach(var i in App.generalDataAccess.GetAllNote(App.session.CurrentUser.GetId()))
                {
                    App.MainDatabase.AddNote(i);
                }

               
                App.MainDatabase.noteSourceInitialized = true; // to prevent memory leaks, I need to control this.
            }
            lstNotes.ItemsSource = App.MainDatabase.NotesSource;



            //Add click events
            TlbSelectionMode.Clicked += ManageSelectionMode;
            TlbSortItems.Clicked += SortItems;
            TlbSelectAll.Clicked += (s, e) => {
                foreach (var i in App.MainDatabase.NotesSource)
                {
                    lstNotes.SelectedItems.Add(i);
                }
                this.SortInner(App.session.NotesSortMethod);
                TlbSelectAll.IsEnabled = false;
            };
            TlbSelectAll.IsEnabled = false;


            ToolbarItems.Add(TlbSelectionMode);
            ToolbarItems.Add(TlbSortItems);
            ToolbarItems.Add(TlbSelectAll);
            BtnRemoveActions(false);

            searchBar.TextChanged += (s, e) => FilterItem(searchBar.Text);
            searchBar.SearchButtonPressed += (s, e) => FilterItem(searchBar.Text);
        }

        private void AddNote(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddNotePage());
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tempNote = e.CurrentSelection.FirstOrDefault() as Notes;
            if (tempNote == null)
            {
                return;
            }
                ((CollectionView)sender).SelectedItem = null;
            if (!MultiSelectionEnabled)
            {
                //await Navigation.PushAsync(new FileInfoPage(tempFile));
                await Navigation.PushAsync(new NoteInfoPage(tempNote));
            }
            else

            {
                tlbShowHowManySelected.Text = lstNotes.SelectedItems.Count.ToString() + " selected";
                BtnRemoveActions(true);
            }
        }

        private void ManageSelectionMode(object sender, EventArgs e)
        {
            MultiSelectionEnabled = !MultiSelectionEnabled;
            if (MultiSelectionEnabled)
            {  
                TlbSelectionMode.Text = "CANCEL";
                lstNotes.SelectionMode = SelectionMode.Multiple;
                TlbSelectAll.IsEnabled = true;
            }
            else
            {
                BtnRemoveActions(false);
                TlbSelectionMode.Text = "SELECT";
                TlbSelectAll.IsEnabled = false;
                lstNotes.SelectionMode = SelectionMode.Single;
                lstNotes.SelectedItems.Clear();
                tlbShowHowManySelected.Text = "";
            }
        }
        
        private void BtnRemoveActions(bool value)
        {
            BtnRemove.IsEnabled = value;
            BtnRemove.IsVisible = value;
        }

        public async void HandleRemoveButton(object sender, EventArgs e)
        {
            
            if (lstNotes.SelectedItems.Count <= 0)
            {
                await DisplayAlert("Error", "You have to select at least one note.", "OK");
                return;
            }
            var tempCollection = new ObservableCollection<Notes>();

        
                    foreach(Notes i in lstNotes.SelectedItems)
                    {
                        tempCollection.Add(i);
                    }
            foreach (var i in tempCollection)
            {

                await Task.Run(async () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RemoveNote(i);
                    }
                    );
                });
            }
            App.sessionManager.Save(App.session);
            BtnRemoveActions(false);
            ManageSelectionMode(sender, e);
            }

        private async void SortItems(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Sort by", "Cancel", null,
                 "Added Time (Now-Past)",
                 "Added Time (Past-Now)",
                 "Title (A-Z)",
                 "Title (Z-A)",
                 "Content (A-Z)",
                 "Content (Z-A)"
                 );
            this.SortInner(action);
        }

        private void SortInner(string action)
        {
            switch (action)
            {
                case "Added Time (Now-Past)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderByDescending(x => x.ActualDateTime);
                    break;
                case "Added Time (Past-Now)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderBy(x => x.ActualDateTime);
                    break;
                case "Title (A-Z)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderBy(x => x.Title);
                    break;
                case "Title (Z-A)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderByDescending(x => x.Title);
                    break;
                case "Content (A-Z)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderBy(x => x.Content);
                    break;
                case "Content (Z-A)":
                    lstNotes.ItemsSource = App.MainDatabase.NotesSource.OrderByDescending(x => x.Content);
                    break;
                default:
                    break;
            }
            App.session.NotesSortMethod = action;
        }

        private void RemoveNote(Notes note)
        {
            App.MainDatabase.RemoveNote(note);
            App.generalDataAccess.DeleteNote(note.Id);
        }

        private void UpdateCollectionView(object sender, EventArgs e)
        {
            lstNotes.ItemsSource = App.MainDatabase.NotesSource;
            refreshView.IsRefreshing = false;

        }

        private void FilterItem(string text)
        {
            refreshView.IsRefreshing = true;
            if (string.IsNullOrWhiteSpace(text))
            {
                lstNotes.ItemsSource = App.MainDatabase.NotesSource;
            }
            else
            {
                lstNotes.ItemsSource = App.MainDatabase.NotesSource.Where(x => x.Content.ToLower().Contains(text.ToLower()));
            }
            refreshView.IsRefreshing = false;
        }

      
    }
}