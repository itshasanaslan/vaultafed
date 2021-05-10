using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using App2.Models;
using Xamarin.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Diagnostics;

namespace App2.Data
{
    public class UserOptions
    {
        public static string LocalDataPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), ".3mNdgs");
        public static byte[] dataSignature = Encoding.UTF8.GetBytes("This file is encrypted by AFED. Do not interfere.");
        
        public int AvailablePasswordId;
        public int AvailableNoteId;
        public long MaxCacheUsage;
        public string Username;
        public bool KeepLoggedIn;
        public bool ExecuteOrder;
        public bool OfflineLogIn;
        public bool VeryNewUser; // to say things.
        public bool HasEncryptedBefore; // to say hey do not delete afed files
        public string _username; // offline
        public string _password;
        public string _offlinePassword;
        public string _name;
        public string _lastName;
        public string _email;
        public string AuthCode;
        public string FilesSortMethod;
        public string NotesSortMethod;
        public string PasswordsSortMethod;
        public int _hasPurchased;
        public int LastServerMessageId;

        public Dictionary<string, List<string>> EncryptedFiles =
            new Dictionary<string, List<string>>(); // {filename: [filepath, thumbnailpath, encryptedpath, size]}

        public Dictionary<int, List<string>> EncryptedNotes = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> EncryptedPasswords = new Dictionary<int, List<string>>();
        public Dictionary<string, int> PasswordCategories = new Dictionary<string, int>();
        

        public static UserOptions ReadLocalDB()
        {
            if (File.Exists(UserOptions.LocalDataPath))
            {

                byte[] temp = File.ReadAllBytes(UserOptions.LocalDataPath);
                List<byte> retrieved = new List<byte>();

                //check signature
                for (int i = 0; i < UserOptions.dataSignature.Length; i++)
                {
                    if (temp[i] != UserOptions.dataSignature[i])
                    {
                        return new UserOptions();
                    }
                }

                //decrypt
                for (int i = temp.Length - 1; i > UserOptions.dataSignature.Length - 1; i--)
                {
                    int x = temp[i] - 1;
                    if (x == -1) x = 255;

                    retrieved.Add((byte)x);
                }
                
                UserOptions options =  JsonConvert.DeserializeObject<UserOptions>(Encoding.UTF8.GetString(retrieved.ToArray()));
                return options;
            }
            return new UserOptions {
                AvailableNoteId = 0,
                AvailablePasswordId = 0,
                MaxCacheUsage = 250 * 1000000,
                Username = "",
                KeepLoggedIn = true,
                ExecuteOrder = false,
                VeryNewUser = true,
                HasEncryptedBefore = false,
                _hasPurchased = 0,
                _username = "",
                _password = "",
                _offlinePassword = "",
                _name = "",
                _lastName = "",
                _email = "",
                AuthCode = "",
                FilesSortMethod = "Added Time (Now-Past)",
                NotesSortMethod = "Added Time(Now - Past)",
                PasswordsSortMethod = "",
                LastServerMessageId = 0
            };
        }

        public bool SaveLocalDB()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            byte[] temp = Encoding.UTF8.GetBytes(json);
            List<byte> toSave = new List<byte>();

            //sign file
            foreach (byte b in UserOptions.dataSignature)
            {
                toSave.Add(b);
            }

            for (int i = temp.Length - 1; i >= 0; i--)
            {
                int x = temp[i] + 1;
                if (x == 256) x = 0;
                toSave.Add((byte)x);
            }
            try
            {
                File.WriteAllBytes(UserOptions.LocalDataPath, toSave.ToArray());
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteDatabase()
        {
            if (File.Exists(UserOptions.LocalDataPath))
            {
                File.Delete(UserOptions.LocalDataPath);
            }
            
        }

        public bool CheckSession(string username)
        {
            if (this.Username == "") return true;
            return this.Username == username;
        }

        public void SetAllInfoForOffline()
        {
            this._username = UserDatabaseController.CurrentUser.username;
            this._password = UserDatabaseController.CurrentUser.password;
            this._name = UserDatabaseController.CurrentUser.name;
            this._lastName = UserDatabaseController.CurrentUser.lastName;
            this._email = UserDatabaseController.CurrentUser.eMail;
            this._hasPurchased = UserDatabaseController.CurrentUser.hasPurchased;
        }

        public bool CheckOfflineLogIn(string username, string password)
        {
            return (username == this._username && password == this._password);
        }

        public void MakeOfflineLogin()
        {
            UserDatabaseController.CurrentUser.name = this._name;
            UserDatabaseController.CurrentUser.lastName = this._lastName;
            UserDatabaseController.CurrentUser.eMail = this._email;
            UserDatabaseController.CurrentUser.hasPurchased = this._hasPurchased;
            this.Username = this._username;
        }

        public bool AddFile(AEF file)
        {
            List<string> fileToList = new List<string> {
                file.ParentDirectory,
                file.Name,
                file.OriginalExtension,
                file.CurrentExtension,
                file.FullName,
                file.EncryptedFileName,
                file.OutputName,
                file.IsEncrypted.ToString(),
                file.ThumbPath,
                file.AddedTime.ToString(),
                file.FileSize,
                file.EncryptionMethod
            };
            try
            {
                this.EncryptedFiles.Add(file.OriginalPath, fileToList);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public AEF GetFile(string filePath)
        {
            AEF file = new AEF();
            List<string> fileAttributes = this.EncryptedFiles[filePath];
            file.OriginalPath = filePath;
            file.ParentDirectory = fileAttributes[0];
            file.Name = fileAttributes[1];
            file.OriginalExtension = fileAttributes[2];
            file.CurrentExtension = fileAttributes[3];
            file.FullName = fileAttributes[4];
            file.EncryptedFileName = fileAttributes[5];
            file.OutputName = fileAttributes[6];
            file.IsEncrypted = bool.Parse(fileAttributes[7]);
            file.ThumbPath = fileAttributes[8];
            file.AddedTime = fileAttributes[9];
            file.FileSize = fileAttributes[10];
            file.EncryptionMethod = fileAttributes[11];
            file.ActualAddedTime = DateTime.Parse(file.AddedTime);
            file.ActualFileSize = file.ParseFileSize();
            return file;
        }

        public bool RemoveFile(string filepath)
        {
            return this.EncryptedFiles.Remove(filepath);
        }

        public void InitializeOfflineLogIn(User user)
        {
            this.OfflineLogIn = true;
            this._username = user.username;
            this._password = user.password;
            // offline icin ayrı bir password oluştur.
        }

        public void AddNote(Notes note, bool update = false)
        {
            List<string> NoteToList = new List<string> {
                note.Id.ToString(),
                note.Title,
                note.Content,
                note.AddedTime
            };
            if (this.EncryptedNotes ==  null)
            {
                this.EncryptedNotes = new Dictionary<int, List<string>>();
            }
            this.EncryptedNotes.Add(note.Id, NoteToList);
            if (!update) this.AvailableNoteId++;

        }

        public void RemoveNote(int id)
        {
            this.EncryptedNotes.Remove(id);
        }

        public void UpdateNote(int id, string title, string content)
        {
            this.EncryptedNotes[id][1] = title;
            this.EncryptedNotes[id][2] = content;
            
        }

        public Notes GetNote(int id)
        {
            Notes note = new Notes();
            List<string> noteString = this.EncryptedNotes[id];
            note.Id = id;
            note.Title = noteString[1];
            note.Content = noteString[2];
            note.AddedTime = noteString[3];
            note.ActualDateTime = DateTime.Parse(note.AddedTime);
            return note;
        }

        public void AddPassword(Passwords password)
        {
            if (this.EncryptedPasswords == null)
            {
                this.EncryptedPasswords = new Dictionary<int, List<string>>();
            }
            List<string> temp = new List<string> {password.Id.ToString(), password.Title, password.UserName, password.Password, password.AddedTime, password.Category };

            this.EncryptedPasswords.Add(password.Id, temp);
            this.AvailablePasswordId++;

        }

        public Passwords GetPassword(int id)
        {
            Passwords passwords = new Passwords();
            List<string> passString = this.EncryptedPasswords[id];
            passwords.Id = id;
            passwords.Title = passString[1];
            passwords.UserName = passString[2];
            passwords.Password = passString[3];
            passwords.AddedTime = passString[4];
            passwords.Category = passString[5];
            passwords.ActualDateTime = DateTime.Parse(passwords.AddedTime);
            return passwords;

        }

        public void RemovePassword(int id)
        {
            this.EncryptedPasswords.Remove(id);
        }

        public void UpdatePassword(int id, Passwords newPassword)
        {
            this.EncryptedPasswords[id][1] = newPassword.Title;    // 2,3,5
            this.EncryptedPasswords[id][2] = newPassword.UserName;
            this.EncryptedPasswords[id][3] = newPassword.Password;
            this.EncryptedPasswords[id][5] = newPassword.Category;

        }

        public void HandlePasswordCategory(string category, string operation)
        {
            if (category == "Undefined" || string.IsNullOrEmpty(category)) return;

            if (operation == "Add")
            {
                if (this.PasswordCategories.ContainsKey(category))
                {
                    this.PasswordCategories[category] += 1;
                }
                else
                {
                    this.PasswordCategories.Add(category, 1);
                }
            }
            else
            {
                if (!this.PasswordCategories.ContainsKey(category))
                {
                    return;
                }
                this.PasswordCategories[category] -= 1;
                if (this.PasswordCategories[category] <= 0)
                {
                    this.PasswordCategories.Remove(category);
                }
                }

        }

        public string[] GetPasswordCategories()
        {
           List <string> temp = new List<string>(this.PasswordCategories.Keys);
            temp.Add("Add a category");
            return temp.ToArray();
        }
    }

}
