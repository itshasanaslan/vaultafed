using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Abstracts
{
    public interface IGeneralDataAccess
    {
        AEF FindFileById(int id);
        ServerResponse AddFile(AEF file);
        ServerResponse DeleteFile(AEF file);
        ServerResponse DeleteFile(int id);
        AEF FindFileByPath(string path);
        ServerResponse UpdateFile(AEF file);
        List<AEF> GetAllFile(int userId);

        Notes FindNoteById(int id);
        ServerResponse AddNote(Notes note);
        ServerResponse DeleteNote(Notes note);
        ServerResponse DeleteNote(int id);
        ServerResponse UpdateNote(Notes note);
        List<Notes> GetAllNote(int userId);

        Passwords FindPasswordById(int id);
        ServerResponse AddPassword(Passwords password);
        ServerResponse DeletePassword(Passwords password);
        ServerResponse DeletePassword(int id);
        ServerResponse UpdatePassword(Passwords password);
        List<Passwords> GetAllPassword(int userId);
        string[] GetPasswordCategories(int userId);

        AEFFolder FindFolderById(int id);
        ServerResponse AddFolder(AEFFolder folder);
        ServerResponse DeleteFolder(AEFFolder folder);
        ServerResponse DeleteFolder(int id);
        AEFFolder FindFolderByTitle(string title);
        ServerResponse UpdateFolder(AEFFolder folder);
        List<AEFFolder> GetAllFolder(int userId);
        AEFFolder FindFolderByUserId(int userId, string title);



    }
}
