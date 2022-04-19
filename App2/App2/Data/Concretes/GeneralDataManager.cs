using App2.Data.Abstracts;
using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Concretes
{
    public class GeneralDataManager : IGeneralDataAccess
    {
        IFileManager fileManager;
        IPasswordManager passwordManager;
        INoteManager noteManager;
        IFolderManager folderManager;

        public GeneralDataManager() 
        {
            this.fileManager = new FileManager();
            this.passwordManager = new PasswordManager();
            this.noteManager = new NoteManager();
            this.folderManager = new FolderManager();
        }

        public ServerResponse AddFile(AEF file)
        {
            file.UserId = App.session.UserId;
            return this.fileManager.Add(file);
        }

        public ServerResponse AddNote(Notes note)
        {
            note.UserId = App.session.UserId;
            return this.noteManager.Add(note);

        }

        public ServerResponse AddPassword(Passwords password)
        {
            password.UserId = App.session.UserId;

            return this.passwordManager.Add(password);
        }

        public ServerResponse DeleteFile(AEF file)
        {
            return this.fileManager.Delete(file);
        }

        public ServerResponse DeleteFile(int id)
        {
            return this.fileManager.Delete(id);
        }

        public ServerResponse DeleteNote(Notes note)
        {
            return this.noteManager.Delete(note);
        }

        public ServerResponse DeleteNote(int id)
        {
            return this.noteManager.Delete(id);
        }

        public ServerResponse DeletePassword(Passwords password)
        {
            return this.passwordManager.Delete(password);
        }

        public ServerResponse DeletePassword(int id)
        {
            return this.passwordManager.Delete(id);
        }

        public AEF FindFileById(int id)
        {
            return this.fileManager.FindById(id);
        }

        public AEF FindFileByPath(string path)
        {
            return this.fileManager.FindByPath(path);
        }

        public Notes FindNoteById(int id)
        {
            return this.noteManager.FindById(id);
        }

        public Passwords FindPasswordById(int id)
        {
            return this.passwordManager.FindById(id);
        }

        public List<AEF> GetAllFile(int userId)
        {
            return this.fileManager.GetAll(userId);
        }

        public List<Notes> GetAllNote(int userId)
        {
            return this.noteManager.GetAll(userId);
        }

        public List<Passwords> GetAllPassword(int userId)
        {
            return this.passwordManager.GetAll(userId);
        }

        public ServerResponse UpdateFile(AEF file)
        {
            return this.fileManager.Update(file);
        }

        public ServerResponse UpdateNote(Notes note)
        {
            return this.noteManager.Update(note);
        }

        public ServerResponse UpdatePassword(Passwords password)
        {
            return this.passwordManager.Update(password);
        }

        public string[] GetPasswordCategories(int userId)
        {
            return this.passwordManager.GetCategories(userId);
        }

        public AEFFolder FindFolderById(int id)
        {
            return this.folderManager.FindById(id);
        }

        public ServerResponse AddFolder(AEFFolder folder)
        {
            return this.folderManager.Add(folder);
        }

        public ServerResponse DeleteFolder(AEFFolder folder)
        {
            return this.folderManager.Delete(folder);
        }

        public ServerResponse DeleteFolder(int id)
        {
            return this.folderManager.Delete(id);
        }

        public AEFFolder FindFolderByTitle(string title)
        {
            return this.folderManager.FindByTitle(title);
        }

        public ServerResponse UpdateFolder(AEFFolder folder)
        {
            return this.folderManager.Update(folder);
        }

        public List<AEFFolder> GetAllFolder(int userId)
        {
            return this.folderManager.GetAll(userId);
        }
        
        public AEFFolder FindFolderByUserId(int userId, string title)
        {
            return this.folderManager.FindByUserId(userId, title);
        }

    }
}
