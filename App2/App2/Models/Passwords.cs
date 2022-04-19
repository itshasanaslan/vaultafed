using App2.Data;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App2.Models
{
    [Table("Passwords")]
    public class Passwords
    {
        [PrimaryKey, AutoIncrement, Unique]
        [Column("password_id")]
        public int Id { get; set; }

        [Column("password_user_id")]
        public int UserId { get; set; }

        [Column("password_category")]
        public string Category { get; set; }

        [Column("password_title")]
        public string Title { get; set; }

        [Column("password_username")]
        public string UserName { get; set; }

        [Column("password_password")]
        public string Password { get; set; }

        [Column("password_added_time")]
        public string AddedTime { get; set; }

     

        [Column("password_actual_added_time")]
        public DateTime ActualDateTime { get; set; }

        public Passwords(string title, string username, string password, string category)
        {
            this.ActualDateTime = DateTime.Now;
            this.AddedTime = this.ActualDateTime.ToString();
            this.Title = title;
            this.UserName = username;
            this.Password = password;
            this.Category = category;
            
        }
        public Passwords() 
        {   
        }
    }
}
