using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Models
{
    [Table("Folders")]
    public class AEFFolder
    {
        // implement later
        [PrimaryKey, AutoIncrement, Unique]
        [Column("folder_id")]
        public int FolderId { get; set; }

        [Column("folder_user_id")]
        public int UserId { get; set; }

        [NotNull]
        [Column("folder_title")]
        public string Title { get; set; }

        [Column("folder_thumbnail_path")]
        public string ThumbnailPath { get; set; }

        [Column("folder_holding_data_type")]
        public string DataType { get; set; } // Photo, Video, Audio, Other

        [Column("folder_visible")]
        public bool Visible { get; set; }
        public AEFFolder() { }
    }
}
