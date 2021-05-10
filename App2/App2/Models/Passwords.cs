using App2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Models
{
    public class Passwords
    {
        public int Id;
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AddedTime { get; set; }
        public string Category { get; set; }
        public DateTime ActualDateTime { get; set; }

        public Passwords(string title, string username, string password, string category)
        {
            this.ActualDateTime = DateTime.Now;
            this.AddedTime = this.ActualDateTime.ToString();
            this.Title = title;
            this.UserName = username;
            this.Password = password;
            this.Category = category;
            this.Id = App.userOptions.AvailablePasswordId;
        }
        public Passwords() 
        {
            this.Id = App.userOptions.AvailablePasswordId;    
        }
    }
}
