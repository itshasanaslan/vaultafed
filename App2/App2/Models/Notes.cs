using App2.Data;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App2.Models
{
    [Table("Notes")]
    public class Notes
    {
        [PrimaryKey, AutoIncrement, Unique]
        [Column("note_id")]
        public int Id { get; set; }

        [Column("note_user_id")]
        public int UserId { get; set; }

        [Column("note_title")]
        public string Title { get; set; }

        [Column("note_content")]
        public string Content { get; set; }
   
        [Column("note_added_time")]
        public string AddedTime { get; set; }

        [Column("note_actual_added_time")]
        public DateTime ActualDateTime { get; set; }

        public Notes(string title, string content)
        {
            this.ActualDateTime = DateTime.Now;
            this.AddedTime = this.ActualDateTime.ToString();
            this.Content = content;
            this.Title = title;
        }
        public Notes()
        {
            
        }
    }
}
