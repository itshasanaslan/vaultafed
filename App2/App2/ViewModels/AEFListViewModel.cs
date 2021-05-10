using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using App2.Models;

namespace App2.ViewModels
{
    public class AEFListViewModel
    {
        public ObservableCollection<AEF> FilesSource { get; set; }
        public ObservableCollection<Notes> NotesSource { get; set; }
        public ObservableCollection<Passwords> PasswordsSource { get; set; }
        public bool fileSourceInitialized { get; set; }
        public bool noteSourceInitialized { get; set; }
        public bool passwordSourceInitialized { get; set; }



        public AEFListViewModel()
        {
            FilesSource = new ObservableCollection<AEF>();
            NotesSource = new ObservableCollection<Notes>();
            PasswordsSource = new ObservableCollection<Passwords>();
           

            /*

            filesSource.Add(new AEF("/storage/emulated/0/file1.jpg"));
            filesSource.Add(new AEF("/storage/emulated/0/file2.jpg"));
            filesSource.Add(new AEF("/storage/emulated/0/file3.jpg"));
            */
        }
        public void AddFile(AEF file)
        {
            this.FilesSource.Add(file);
        }
        public void RemoveFile(AEF file)
        {
            this.FilesSource.Remove(file);
        }

        public void AddNote(Notes note)
        {
            this.NotesSource.Add(note);
        }
        public void RemoveNote(Notes note)
        {
            this.NotesSource.Remove(note);
        }

        public void AddPassword(Passwords password)
        {
            this.PasswordsSource.Add(password);
        }
        public void RemovePassword(Passwords password)
        {
            this.PasswordsSource.Remove(password);
        }



        public void Sort(string orderHow)
        {
            switch (orderHow)
            {
                case "File Name (A-Z)":
                    this.FilesSource = new ObservableCollection<AEF>(this.FilesSource.OrderBy(i => i));
                    break;
                case "File Name (Z-A)":
                    break;
                case "Added Time (Now-Past)":
                    break;
                case "Added Time (Past-Now)":
                    break;
                case "File Size(Bigger-Smaller)":
                    break;
                case "File Size(Smaller-Bigger)":
                    break;
                default:
                    break;
            }
        }

     
    }

    
}
