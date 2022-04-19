using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Diagnostics;

namespace App2.Data
{
    [Table("Sessions")]
    public class Session
    {
        [PrimaryKey, AutoIncrement, Unique]
        [Column("session_id")]
        public int id { get; set; }
        public User CurrentUser;

        [Column("session_user_id")]
        public int UserId { get; set; }

        [Column("sessionauthorization_code")]
        public string AuthorizationCode { get; set; }

        [Column("session_max_cache_usage")]
        public long MaxCacheUsage { get; set; }

        [Column("session_keep_logged_in")]
        public bool KeepLoggedIn { get; set; }

        [Column("session_execute_order")]
        public bool ExecuteOrder { get; set; }


        [Column("session_offline_login")]
        public bool OfflineLogIn { get; set; }


        [Column("session_very_new_user")]
        public bool VeryNewUser { get; set; }// to say things.


        [Column("session_has_encrypted_before")]
        public bool HasEncryptedBefore { get; set; } // to say hey do not delete afed files

        [Column("session_offline_password")]
        public string _offlinePassword { get; set; }

        [Column("session_files_sort_method")]
        public string FilesSortMethod { get; set; }


        [Column("session_photos_sort_method")]
        public string PhotosSortMethod { get; set; }


        [Column("session_videos_sort_method")]
        public string VideosSortMethod { get; set; }


        [Column("session_audios_sort_method")]
        public string AudiosSortMethod { get; set; }

        [Column("session_notes_sort_method")]
        public string NotesSortMethod { get; set; }


        [Column("session_password_sort_method")]
        public string PasswordsSortMethod { get; set; }

        [Column("session_last_server_message_id")]
        public int LastServerMessageId { get; set; }

        [Column("session_last_session")]
        public DateTime LatestSession { get; set; }


        public Dictionary<string, List<string>> EncryptedFiles =
         new Dictionary<string, List<string>>(); // {filename: [filepath, thumbnailpath, encryptedpath, size]}

        public Dictionary<int, List<string>> EncryptedNotes = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> EncryptedPasswords = new Dictionary<int, List<string>>();
        public Dictionary<string, int> PasswordCategories = new Dictionary<string, int>();

        public Session() {
        this.AuthorizationCode = "0xHEXthisisfromVaultafed";
        }

        public string GetAllProp()
        {
            string o = "";
            foreach (var prop in this.GetType().GetProperties())
            {
                 
                    o += prop.Name + " : " + prop.GetValue(this, null) + "\n";
                
              
            }

            return o;
        }
    }
}
